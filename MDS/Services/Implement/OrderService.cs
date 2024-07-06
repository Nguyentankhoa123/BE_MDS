using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Order;
using MDS.Services.DTO.VnPay;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Core.Helper;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using Scriban;
using System.Globalization;

namespace MDS.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        public OrderService(AppDbContext context, IMapper mapper, IRedisService redisService, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _redisService = redisService;
            _config = config;
            _emailService = emailService;
        }

        public async Task<OrderListObjectResponse> TestOrderDrugstore(OrderRequest request, string userId, string? code, int addressId, HttpContext context)
        {
            OrderListObjectResponse response = new();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var address = await _context.Addresss.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == addressId);
            if (user == null)
            {
                throw new NotFoundException("User not found!");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new NotFoundException("The cart does not exist or is empty!.");
            }

            var groupCartItems = cart.CartItems.GroupBy(ci => ci.Product.DrugstoreId);
            List<Order> orders = new List<Order>();

            foreach (var group in groupCartItems)
            {
                var order = _mapper.Map<Order>(request);
                order.User = user;
                order.OrderDetails = group.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    Total = ci.Quantity * ci.Product.Price,
                    Name = ci.Product.Name,
                    PictureUrls = ci.Product.PictureUrls.FirstOrDefault(),
                    DrugstoreId = ci.Product.DrugstoreId,
                    OrderStatus = "Đang xử lý",
                }).ToList();

                order.TotalPrice = order.OrderDetails.Sum(od => od.Total);


                string paymentUrl = null;
                if (request.PaymentType == "VnPay")
                {
                    var vnPayRequest = new VnPayRequest
                    {
                        OrderId = order.Id,
                        Amount = order.TotalPrice,
                        CreatedDate = DateTime.Now
                    };

                    var vnPayRepo = new VnPayService(_config);
                    paymentUrl = vnPayRepo.CreatePaymentUrl(context, vnPayRequest);
                }
                else if (request.PaymentType == "cod")
                {
                }

                _context.Orders.Add(order);
                orders.Add(order);
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Order success";
            response.Data = _mapper.Map<List<OrderResponse>>(orders);


            return response;

        }


        public async Task<OrderObjectResponse> CreateAsync(OrderRequest request, string userId, string? code, int addressId, HttpContext context)
        {
            OrderObjectResponse response = new();

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var address = await _context.Addresss.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == addressId);

            if (user == null)
            {
                throw new NotFoundException("User not found!");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new NotFoundException("The cart does not exist or is empty!.");
            }

            var order = _mapper.Map<Order>(request);

            order.User = user;



            order.OrderDetails = cart.CartItems.Select(ci => new OrderDetail
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Price = ci.Product.Price,
                Total = ci.Quantity * ci.Product.Price,
                Name = ci.Product.Name,
                PictureUrls = ci.Product.PictureUrls.FirstOrDefault(),
                DrugstoreId = ci.Product.DrugstoreId,
                OrderStatus = "Đang xử lý",
            }).ToList();


            // Có lỗi nếu như có 2 sản phẩm order tồn kho thì chỉ hiển thị ra thông báo sản phẩm đầu tiên
            foreach (var detail in order.OrderDetails)
            {
                string lockKey = await _redisService.AcquireLockAsync(detail.ProductId, detail.Quantity, cart.Id);
                var stock = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == detail.ProductId);
                if (lockKey == null)
                {
                    int overQuantity = detail.Quantity - stock.Stock;
                    throw new BadRequestException("Một số sản phẩm đã hết hàng hoặc số lượng không đủ. Vui lòng kiểm tra lại giỏ hàng.");
                }
            }

            double totalDiscountAmount = 0;
            if (!string.IsNullOrEmpty(code))
            {
                var discount = await _context.Discounts.FirstOrDefaultAsync(x => x.Code == code);

                var discountUser = await _context.DiscountsUser.FirstOrDefaultAsync(x => x.DiscountId == discount.Id && x.UserId == userId);

                if (discount == null)
                {
                    throw new NotFoundException("Discount not found!");
                }

                if (discountUser != null && !discountUser.IsActive)
                {
                    throw new NotFoundException("Discount expried for this user");
                }


                if (discount.MaxUse == 0 || discount.UseCount >= discount.MaxUse)
                {
                    throw new NotFoundException("Discount are out");
                }

                if (discount != null)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        if (discount.ApplyTo == DiscountApply.System || discount.ApplyTo == DiscountApply.Drugstore)
                        {
                            var discountAmount = detail.Total * discount.Percent / 100;
                            discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount.Value);

                            totalDiscountAmount += discountAmount;
                        }
                    }

                    discount.UseCount += 1;
                    if (discount.UseCount >= discount.MaxUse)
                    {
                        _context.Discounts.Remove(discount);
                    }

                    await _context.SaveChangesAsync();

                    if (discountUser == null)
                    {
                        discountUser = new DiscountUser
                        {
                            DiscountId = discount.Id,
                            UserId = userId,
                            IsActive = false,
                            UsedAt = DateTime.Now,
                        };
                        _context.DiscountsUser.Add(discountUser);
                    }
                    else
                    {
                        discountUser.IsActive = false;
                        discountUser.UsedAt = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            order.DiscountAmount = totalDiscountAmount;
            order.TotalPrice = order.OrderDetails.Sum(x => x.Total) - totalDiscountAmount;

            if (user == null || order == null)
            {
                throw new NotFoundException("User or Order is null!");
            }


            string paymentUrl = null;
            if (request.PaymentType == "VnPay")
            {
                var vnPayRequest = new VnPayRequest
                {
                    OrderId = order.Id,
                    Amount = order.TotalPrice,
                    CreatedDate = DateTime.Now
                };

                var vnPayRepo = new VnPayService(_config);
                paymentUrl = vnPayRepo.CreatePaymentUrl(context, vnPayRequest);
            }
            else if (request.PaymentType == "cod")
            {
            }

            _context.Orders.Add(order);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(order.CreateOn, vietnamTimeZone);

            var totalPrice = order.TotalPrice.ToString("C", new CultureInfo("vi-VN"));
            var discountPrice = totalDiscountAmount.ToString("C", new CultureInfo("vi-VN"));
            var price = "";

            foreach (var item in order.OrderDetails)
            {
                price = item.Product.Price.ToString("C", new CultureInfo("vi-VN"));
            };
            var template = Template.Parse(@"
                            <!DOCTYPE html>
                            <html lang=""en"">
                              <head>
                                <meta charset=""UTF-8"" />
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                              </head>
                              <body>
                                <div style=""line-height: 18pt"">
                                  <p>Xin chào {{user.first_name}} {{user.last_name}}</p>
                                  <p>
                                    Sản phẩm trong đơn hàng của Anh/chị tại cửa hàng
                                    <strong>Pharma MDS</strong>
                                  </p>
                                  <div style=""font-size: 18px"">Thông tin giao hàng</div>
                                  <table>
                                    <tbody>
                                      <tr>
                                        <td>Mã đơn hàng: <strong>{{order.tracking_number}}</strong></td>
                                        <td style=""padding-left: 40px"">Ngày tạo giao hàng: {{time | date.to_string '%d/%m/%Y %I:%M:%S %p'}}</td>
                                      </tr>
                                    </tbody>
                                  </table>
                                  <table style=""width: 100%; border-collapse: collapse"">
                                    <thead>
                                      <tr>
                                        <th
                                          style=""
                                            border-left: 1px solid #d7d7d7;
                                            border-right: 1px solid #d7d7d7;
                                            border-top: 1px solid #d7d7d7;
                                            padding: 5px 10px;
                                            text-align: left;
                                          ""
                                        >
                                          Thông tin người nhận
                                        </th>
                                        <th
                                          style=""
                                            border-left: 1px solid #d7d7d7;
                                            border-right: 1px solid #d7d7d7;
                                            border-top: 1px solid #d7d7d7;
                                            padding: 5px 10px;
                                            text-align: left;
                                          ""
                                        >
                                          Thông tin vận chuyển
                                        </th>
                                      </tr>
                                    </thead>
                                    <tbody>
                                      <tr>
                                        <td
                                          style=""
                                            border-left: 1px solid #d7d7d7;
                                            border-right: 1px solid #d7d7d7;
                                            border-bottom: 1px solid #d7d7d7;
                                            padding: 5px 10px;
                                          ""
                                        >
                                          <table style=""width: 100%"">
                                            <tbody>
                                              <tr>
                                                <td>Họ tên:</td>
                                                <td>{{address.name}}</td>
                                              </tr>
                                              <tr>
                                                <td>Điện thoại:</td>
                                                <td>{{address.contact}}</td>
                                              </tr>
                                              <tr>
                                                <td>Địa chỉ:</td>
                                                <td>{{address.street}}</td>
                                              </tr>
                                              <tr>
                                                <td>Phường xã:</td>
                                                <td>{{address.ward}}</td>
                                              </tr>
                                              <tr>
                                                <td>Quận huyện:</td>
                                                <td>{{address.district}}</td>
                                              </tr>
                                              <tr>
                                                <td>Tỉnh thành:</td>
                                                <td>{{address.province}}</td>
                                              </tr>
                                            </tbody>
                                          </table>
                                        </td>
                                        <td
                                          style=""
                                            border-left: 1px solid #d7d7d7;
                                            border-right: 1px solid #d7d7d7;
                                            border-bottom: 1px solid #d7d7d7;
                                            padding: 5px 10px;
                                          ""
                                        >
                                          <table style=""width: 100%"">
                                            <tbody>
                                              <tr>
                                                <td><strong>Đơn vị: </strong></td>
                                                <td>{{order.carrier}}</td>
                                              </tr>
                                              <tr>
                                                <td><strong>Phương thức:</strong></td>
                                                <td>{{order.payment_type}}</td>
                                              </tr>
            <tr>
                                                <td><strong>Tổng giá tiền Voucher giảm giá:</strong></td>
                                                <td>{{discount_price}}</td>
                                              </tr>
                                            </tbody>
                                          </table>
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>
                                  <table style=""width: 100%; border-collapse: collapse"">
                                    <thead>
                                      <tr
                                        style=""
                                          border: 1px solid #d7d7d7;
                                          border-top: none;
                                          background-color: #f8f8f8;
                                        ""
                                      >
                                        <th style=""text-align: left; padding: 5px 10px"">
                                          <strong>Sản phẩm được giao</strong>
                                        </th>
                                        <th style=""text-align: left""><strong>Giá tiền</strong></th>   
                                        <th style=""text-align: center; padding: 5px 10px"">
                                          <strong>Số lượng</strong>
                                        </th>
                                      </tr>
                                    </thead>
                                    <tbody>
                                    {{- for item in order.order_details }}
                                      <tr style=""border: 1px solid #d7d7d7"">
                                        <td style=""padding: 5px 10px"">
                                             {{item.product.name}}
                                        </td>
                                        <td>{{price}}</td>
                                        <td style=""text-align: center; padding: 5px 10px"">{{item.quantity}}</td>
                                      </tr>
                                    {{- end }}     
                                      <tr style=""border: 1px solid #d7d7d7"">
                                        <td colspan=""2""></td>
                                        <td colspan=""2"">
                                          <table style=""width: 100%"">
                                            <tbody>
                                              <tr>
                                                <td>
                                                  <strong>Giá trị đơn hàng</strong>
                                                </td>
                                                <td style=""text-align: right"">{{total_price}}</td>
                                              </tr>
                                            </tbody>
                                          </table>
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>
                                </div>
                              </body>
                            </html>");

            response.StatusCode = ResponseCode.OK;
            response.Message = "Order success";
            response.Data = _mapper.Map<OrderResponse>(order);

            response.Data.PaymentUrl = paymentUrl;

            if (response.StatusCode == ResponseCode.OK)
            {
                var emailContent = await template.RenderAsync(new { user = user, order = order, address = address, total_price = totalPrice, price = price, time = localCreatedAt, discount_price = discountPrice });
                var message = new Message(new[] { user.Email }, "Thông tin đơn hàng", emailContent);
                _emailService.SendEmail(message);
            }

            return response;
        }



        public async Task<OrderListObjectResponse> GetAsync(string userId)
        {
            OrderListObjectResponse response = new();

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Drugstore)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (!orders.Any())
            {
                throw new BadRequestException("Order not found!");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Order retrieved";
            response.Data = _mapper.Map<List<OrderResponse>>(orders);

            return response;
        }

        public async Task<OrderDetailListObjectResponse> GetOrderDetailsByDrugstoreAsync(string drugstoreId, int pageNumber = 1, int pageSize = 5)
        {
            OrderDetailListObjectResponse response = new();

            var skipResults = (pageNumber - 1) * pageSize;

            var totalOrderDetails = await _context.OrderDetails.CountAsync(od => od.DrugstoreId == drugstoreId);

            var totalPages = (int)Math.Ceiling((double)totalOrderDetails / pageSize);

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .Where(od => od.DrugstoreId == drugstoreId)
                .ToListAsync();

            var pagedOrderDetails = orderDetails.Skip(skipResults).Take(pageSize).ToList();

            var orderDetailResponses = _mapper.Map<List<OrderDetailResponse>>(pagedOrderDetails);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Order details retrieved!";
            response.Data = orderDetailResponses;
            response.TotalPages = totalPages;

            return response;
        }


        public async Task<JmetterObjectResponse> TestOrderAsync(int productId, int quantity)
        {
            JmetterObjectResponse response = new();

            string lockKey = await _redisService.AcquireLockAsync(productId, quantity, 10);
            var stock = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (lockKey == null)
            {
                throw new BadRequestException("Một số sản phẩm đã hết hàng hoặc số lượng không đủ. Vui lòng kiểm tra lại giỏ hàng.");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Mua hàng thành công";

            return response;
        }



        public async Task<JmetterObjectResponse> TestOrderNoReisAsync(int productId, int quantity)
        {
            JmetterObjectResponse response = new();

            string userId = "User_" + Guid.NewGuid().ToString();


            var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventory.Stock < quantity)
            {
                throw new BadRequestException("Sản phẩm trong kho không đủ");
            }
            inventory.Stock -= quantity;

            await _context.SaveChangesAsync();



            response.StatusCode = ResponseCode.OK;
            response.Message = $"Mua hàng thành công cho người dùng {userId}";


            return response;
        }

    }
}
