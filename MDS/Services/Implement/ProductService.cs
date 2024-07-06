using AutoMapper;
using MDS.Model.Entity;
using MDS.Services.DTO.Product;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Database.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MDS.Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IInventoryService _inventoryService;
        public ProductService(UserManager<ApplicationUser> userManager, IMapper mapper, AppDbContext context, IInventoryService inventoryService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _inventoryService = inventoryService;
        }
        public async Task<MedicineObjectResponse> CreateMedicineAsync(string userId, MedicineRequest request)
        {
            MedicineObjectResponse response = new();

            var product = _mapper.Map<Product>(request);
            product.DrugstoreId = userId;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();


            await _inventoryService.InsertInventory(product.Id, product.Quantity, userId);

            response.StatusCode = ResponseCode.CREATED;
            response.Message = "Product created and indexed successfully";
            response.Data = _mapper.Map<MedicineResponse>(product);

            return response;
        }

        public async Task<NotMedicineObjectResponse> CreateNotMedicineAsync(string userId, NotMedicineRequest request)
        {
            NotMedicineObjectResponse response = new();

            var product = _mapper.Map<Product>(request);
            product.DrugstoreId = userId;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();


            await _inventoryService.InsertInventory(product.Id, product.Quantity, userId);


            response.StatusCode = ResponseCode.CREATED;
            response.Message = "Product created";
            response.Data = _mapper.Map<NotMedicineResponse>(product);

            return response;
        }

        public async Task<ProductObjectResponse> DeleteAsync(int id)
        {
            ProductObjectResponse response = new();

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                throw new NotFoundException("Product not found!");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Product deleted!";
            response.Data = _mapper.Map<ProductResponse>(product);

            return response;
        }

        public async Task<ProductListObjectResponse> GetAllAsync(int pageNumber = 1, int pageSize = 5)
        {
            ProductListObjectResponse response = new();

            var skipResults = (pageNumber - 1) * pageSize;

            var totalProducts = await _context.Products.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);


            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.Reservations)
                .ToListAsync();


            var pagedProducts = products.Skip(skipResults).Take(pageSize).ToList();

            var productResponses = _mapper.Map<List<ProductResponse>>(pagedProducts);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Products retrieved!";
            response.Data = productResponses;
            response.TotalPages = totalPages;

            return response;
        }

        public async Task<ProductObjectResponse> GetByIdAsync(int id)
        {
            ProductObjectResponse response = new();

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Drugstore)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.Reservations)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                throw new NotFoundException("Product not found!");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Get by id!";
            response.Data = _mapper.Map<ProductResponse>(product);

            return response;
        }

        public async Task<ProductListObjectResponse> GetDrugstoresForProduct(string id, int pageNumber = 1, int pageSize = 5)
        {
            ProductListObjectResponse response = new();

            var skipResults = (pageNumber - 1) * pageSize;

            // Tính tổng số sản phẩm
            var totalProducts = await _context.Products
                .Where(p => p.DrugstoreId == id)
                .CountAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await _context.Products
                .Where(p => p.DrugstoreId == id)
                .Include(p => p.Drugstore)
                .Include(p => p.Inventory)
                .Skip(skipResults)
                .Take(pageSize)
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                throw new NotFoundException("No products found for the specified drugstore.");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Products found for the specified drugstore.";
            response.Data = _mapper.Map<List<ProductResponse>>(products);
            response.TotalPages = totalPages;

            return response;
        }




        public async Task<ProductListObjectResponse> SearchAsync(string? nameQuery, string? activeIngredientQuery, string? useQuery, string? brandQuery, string? dosageFormQuery, string? filterQuery, string? priceSortOrder, int pageNumber, int pageSize)
        {
            ProductListObjectResponse response = new();

            try
            {
                var products = _context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nameQuery))
                {
                    products = products.Where(x => EF.Functions.ILike(x.Name, $"%{nameQuery}%"));
                }

                if (!string.IsNullOrWhiteSpace(activeIngredientQuery))
                {
                    products = products.Where(x => x.ActiveIngredient != null && x.ActiveIngredient.Contains(activeIngredientQuery));
                }

                if (!string.IsNullOrWhiteSpace(useQuery))
                {
                    products = products.Where(x => x.Use.Contains(useQuery));
                }

                if (!string.IsNullOrWhiteSpace(brandQuery))
                {
                    products = products.Include(p => p.Brand).Where(x => x.Brand != null && x.Brand.Name.Contains(brandQuery));
                }

                if (!string.IsNullOrWhiteSpace(dosageFormQuery))
                {
                    products = products.Where(x => x.DosageForm != null && x.DosageForm.Contains(dosageFormQuery));
                }

                if (!string.IsNullOrWhiteSpace(filterQuery))
                {
                    products = products.Where(x =>
                        (!string.IsNullOrWhiteSpace(x.ProductType) && x.ProductType.Contains(filterQuery)) ||
                        (x.Category != null && x.Category.Name.Contains(filterQuery)) ||
                        (!string.IsNullOrWhiteSpace(x.DetailCategory) && x.DetailCategory.Contains(filterQuery))
                    );
                }



                var totalProducts = await products.CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                var skipResults = (pageNumber - 1) * pageSize;

                var pagedProductsQuery = products
                    .Skip(skipResults)
                    .Take(pageSize);



                if (!string.IsNullOrWhiteSpace(priceSortOrder))
                {
                    if (priceSortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        pagedProductsQuery = pagedProductsQuery.OrderByDescending(x => x.Price);
                    }
                    else if (priceSortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                    {
                        pagedProductsQuery = pagedProductsQuery.OrderBy(x => x.Price);
                    }
                }


                var result = await pagedProductsQuery
                    .Include(p => p.Category)
                    .Include(p => p.Drugstore)
                    .Include(p => p.Brand)
                    .ToListAsync();



                if (result == null || result.Count == 0)
                {
                    throw new NotFoundException("No product found");
                }

                response.StatusCode = ResponseCode.OK;
                response.Message = "Medicines  retrieved";
                response.Data = _mapper.Map<List<ProductResponse>>(result);
                response.TotalPages = totalPages;
            }
            catch (Exception ex)
            {
                response.StatusCode = ResponseCode.BADREQUEST;
                response.Message = "Search failed: " + ex.Message;

            }

            return response;
        }

        public async Task<ProductObjectResponse> UpdateAsync(int id, ProductRequest request)
        {
            ProductObjectResponse response = new();

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                throw new NotFoundException("Product not exists!");
            }

            _mapper.Map(request, product);

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Product updated!";

            return response;
        }
    }
}
