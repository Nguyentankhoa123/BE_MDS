using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Cart;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class CartService : ICartService
    {
        private IMapper _mapper;
        private AppDbContext _context;
        public CartService(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<CartObjectResponse> CreateAsync(string userId, int productId, int quantity)
        {
            CartObjectResponse response = new();

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new NotFoundException("Product not found!");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = productId,
                            Quantity = quantity
                        }
                    }
                };
                _context.Carts.Add(cart);
            }
            else
            {
                var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });
                }
            }

            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Add to cart";
            response.Data = _mapper.Map<CartResponse>(cart);

            return response;
        }

        public async Task<CartObjectResponse> DecreaseAsync(string userId, int productId, int quantity)
        {
            CartObjectResponse response = new();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found!");
            }

            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem == null)
            {
                throw new NotFoundException("CartItem not found!");
            }

            if (quantity > existingItem.Quantity)
            {
                throw new BadRequestException("Quantity to remove is greater than existing quantity");
            }

            existingItem.Quantity -= quantity;

            if (existingItem.Quantity < 1)
            {
                throw new BadRequestException("Quantity cannot be less than 1");
            }

            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Decrease success";
            response.Data = _mapper.Map<CartResponse>(cart);

            return response;
        }

        public async Task<CartObjectResponse> GetAsync(string userId)
        {
            CartObjectResponse response = new();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Drugstore)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found!");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Cart retrieved!";
            response.Data = _mapper.Map<CartResponse>(cart);

            return response;
        }

        public async Task<CartObjectResponse> RemoveAllAsync(string userId)
        {
            CartObjectResponse response = new();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found");
            }

            cart.CartItems.Clear();

            if (!cart.CartItems.Any())
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Remove all success";
            response.Data = _mapper.Map<CartResponse>(cart);

            return response;
        }

        public async Task<CartObjectResponse> RemoveAsync(string userId, int productId)
        {
            CartObjectResponse response = new();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found!");
            }

            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem == null)
            {
                throw new NotFoundException("CartItem not found");
            }

            cart.CartItems.Remove(existingItem);

            if (!cart.CartItems.Any())
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Remove success";
            response.Data = _mapper.Map<CartResponse>(cart);

            return response;
        }
    }
}
