using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.FeedBack;
using MDS.Shared.Core.Enums;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class FeedBackService : IFeedBackService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FeedBackService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<FeedBackObjectResponse> CreateAsync(FeedBackRequest request)
        {
            FeedBackObjectResponse response = new();

            var feedback = _mapper.Map<FeedBack>(request);

            _context.FeedBacks.Add(feedback);
            await _context.SaveChangesAsync();

            var orderDetail = await _context.OrderDetails
                .Include(x => x.Order)
                .FirstOrDefaultAsync(od => od.DrugstoreId == request.DrugstoreId && od.ProductId == request.ProductId && od.Order.UserId == request.UserId);

            if (orderDetail != null)
            {
                orderDetail.isReviewed = true;

                await _context.SaveChangesAsync();
            }

            response.StatusCode = ResponseCode.CREATED;
            response.Message = "Created feedback";
            response.Data = _mapper.Map<FeedBackResponse>(feedback);

            return response;

        }

        //public async Task<AllDrugstoreListObjectResponse> GetAllDrugstores()
        //{
        //    AllDrugstoreListObjectResponse response = new();

        //    var drugstoreRoleId = await _context.Roles
        //        .Where(r => r.Name == "Drugstore")
        //        .Select(r => r.Id)
        //        .FirstOrDefaultAsync();

        //    if (drugstoreRoleId == null)
        //    {
        //        // Xử lý khi không tìm thấy role "Drugstore"
        //        throw new BadRequestException("Lỗi");
        //    }

        //    var drugstoreUserIds = await _context.UserRoles
        //      .Where(ur => ur.RoleId == drugstoreRoleId)
        //      .Select(ur => ur.UserId)
        //      .ToListAsync();

        //    var drugstoreUsers = await _context.Users
        //        .Where(u => drugstoreUserIds.Contains(u.Id))
        //        .Include(u => u.Address)
        //        .ToListAsync();

        //    response.StatusCode = ResponseCode.OK;
        //    response.Message = "Success";
        //    response.Data = _mapper.Map<List<AllDrugstoreResponse>>(drugstoreUsers);


        //    return response;
        //}

        public async Task<FeedBackListObjectResponse> GetFeedbacksByDrugstore(string drugstoreId)
        {
            FeedBackListObjectResponse response = new();

            var feedbacks = await _context.FeedBacks
                .Where(f => f.DrugstoreId == drugstoreId)
                .Include(x => x.Drugstore)
                .Include(x => x.User)
                .ToListAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Feedbacks retrieved!";
            response.Data = _mapper.Map<List<FeedBackResponse>>(feedbacks);

            return response;
        }
    }
}
