using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Brand;
using MDS.Services.DTO.Product;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class BrandService : IBrandService
    {
        private IMapper _mapper;
        private AppDbContext _context;
        public BrandService(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<BrandObjectResponse> CreateAsync(BrandRequest request)
        {
            BrandObjectResponse response = new();

            var brand = _mapper.Map<Brand>(request);

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Brand created!";
            response.Data = _mapper.Map<BrandResponse>(brand);

            return response;
        }

        public async Task<BrandObjectResponse> DeleteAsync(int id)
        {
            BrandObjectResponse response = new();

            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);

            if (brand == null)
            {
                throw new NotFoundException("Brand not found!");
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Brand deleted!";
            response.Data = _mapper.Map<BrandResponse>(brand);

            return response;
        }

        public async Task<BrandListObjectResponse> GetAllAsync(int pageNumber, int pageSize)
        {
            BrandListObjectResponse response = new();

            var skipResults = (pageNumber - 1) * pageSize;

            var brands = await _context.Brands.ToListAsync();

            var pagedBrands = brands.Skip(skipResults).Take(pageSize).ToList();

            var brandResponses = _mapper.Map<List<BrandResponse>>(pagedBrands);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Brands retrieved!";
            response.Data = brandResponses;

            return response;
        }

        public async Task<BrandObjectResponse> GetByIdAsync(int id)
        {
            BrandObjectResponse response = new();

            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);

            if (brand == null)
            {
                throw new NotFoundException("Brand not found!");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Get by id!";
            response.Data = _mapper.Map<BrandResponse>(brand);

            return response;
        }

        public async Task<BrandWithProductsObjectResponse> GetProductsByBrandId(int id, int pageNumber, int pageSize)
        {
            BrandWithProductsObjectResponse response = new();

            var brand = await _context.Brands.Include(b => b.Products).FirstOrDefaultAsync(c => c.Id == id);

            if (brand == null)
            {
                throw new NotFoundException("Brand not found!");
            }

            var skipResults = (pageNumber - 1) * pageSize;
            var pagedProducts = brand.Products.Skip(skipResults).Take(pageSize).ToList();

            var brandResponse = _mapper.Map<BrandWithProductsResponse>(brand);
            brandResponse.Products = _mapper.Map<List<ProductResponse>>(pagedProducts);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Get products by brand!";
            response.Data = brandResponse;

            return response;
        }
    }
}
