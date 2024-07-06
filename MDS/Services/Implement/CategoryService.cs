using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Category;
using MDS.Services.DTO.Product;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private IMapper _mapper;
        private AppDbContext _context;
        public CategoryService(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<CategoryObjectResponse> CreateAsync(CategoryRequest request)
        {
            CategoryObjectResponse response = new();

            var category = _mapper.Map<Category>(request);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.CREATED;
            response.Message = "Category created!";
            response.Data = _mapper.Map<CategoryResponse>(category);

            return response;
        }

        public async Task<CategoryObjectResponse> DeleteAsync(int id)
        {
            CategoryObjectResponse response = new();

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                throw new NotFoundException("Category not found!");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Category deleted!";
            response.Data = _mapper.Map<CategoryResponse>(category);

            return response;
        }

        public async Task<CategoryListObjectResponse> GetAllAsync(int pageNumber = 1, int pageSize = 5)
        {
            CategoryListObjectResponse response = new();

            var skipResults = (pageNumber - 1) * pageSize;

            var categories = await _context.Categories.ToListAsync();

            var pagedCategories = categories.Skip(skipResults).Take(pageSize).ToList();

            var categoryResponses = _mapper.Map<List<CategoryResponse>>(pagedCategories);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Categories retrieved!";
            response.Data = categoryResponses;

            return response;
        }

        public async Task<CategoryObjectResponse> GetByIdAsync(int id)
        {
            CategoryObjectResponse response = new();

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                throw new NotFoundException("Category not found!");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Get by id!";
            response.Data = _mapper.Map<CategoryResponse>(category);

            return response;
        }

        public async Task<CategoryWithProductsObjectResponse> GetProductsByCategoryId(int id, int pageNumber = 1, int pageSize = 5)
        {
            CategoryWithProductsObjectResponse response = new();

            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new NotFoundException("Category not found!");
            }

            var skipResults = (pageNumber - 1) * pageSize;
            var pagedProducts = category.Products.Skip(skipResults).Take(pageSize).ToList();

            var categoryResponse = _mapper.Map<CategoryWithProductsResponse>(category);
            categoryResponse.Products = _mapper.Map<List<ProductResponse>>(pagedProducts);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Get products by category!";
            response.Data = categoryResponse;

            return response;
        }
    }
}
