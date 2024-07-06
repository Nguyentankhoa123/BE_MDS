using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Discount;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Core.Helper;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class DiscountService : IDiscountService
    {
        private IMapper _mapper;
        private AppDbContext _context;
        public DiscountService(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<DiscountObjectResponse> CreateAsync(DiscountRequest request)
        {
            DiscountObjectResponse response = new();

            if (DateTime.Now > request.StartDate || DateTime.Now > request.EndDate)
            {
                throw new BadRequestException("Mã giảm giá đã hết hạn!");
            }

            if (request.StartDate >= request.EndDate)
            {
                throw new BadRequestException("Ngày bắt đầu phải trước ngày kết thúc");
            }

            var foundDiscount = await _context.Discounts.FirstOrDefaultAsync(x => x.Code == request.Code);


            if (foundDiscount != null)
            {
                throw new BadRequestException("Discount exists!");
            }

            var discount = _mapper.Map<Discount>(request);

            if (request.DrugstoreId != null)
            {
                discount.ApplyTo = DiscountApply.Drugstore;
                discount.DrugstoreId = request.DrugstoreId;
            }
            else
            {
                discount.ApplyTo = DiscountApply.System;
            }

            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();


            response.StatusCode = ResponseCode.CREATED;
            response.Message = "Discount created!";
            response.Data = _mapper.Map<DiscountResponse>(discount);

            return response;
        }

        public async Task<DiscountObjectResponse> DeleteAsync(int discountId)
        {
            DiscountObjectResponse response = new();

            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Id == discountId);

            if (discount == null)
            {
                throw new NotFoundException("Discount not found!");
            }

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Discount deleted!";

            return response;
        }

        public async Task<DiscountListObjectResponse> GetAllAsync(string? drugstoreId, int pageNumber, int pageSize)
        {
            DiscountListObjectResponse response = new();

            var skipResults = (pageNumber - 1) * pageSize;

            var totalDiscounts = await _context.Discounts
                .Where(d => drugstoreId == null || (d.ApplyTo == DiscountApply.Drugstore && d.DrugstoreId == drugstoreId))
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalDiscounts / pageSize);

            var discounts = await _context.Discounts
                .Where(d => drugstoreId == null || (d.ApplyTo == DiscountApply.Drugstore && d.DrugstoreId == drugstoreId))
                .Include(d => d.DiscountUsers)
                .Skip(skipResults)
                .Take(pageSize)
                .ToListAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Discount retrieved!";
            response.Data = _mapper.Map<List<DiscountResponse>>(discounts);
            response.TotalPages = totalPages;

            return response;
        }
    }
}
