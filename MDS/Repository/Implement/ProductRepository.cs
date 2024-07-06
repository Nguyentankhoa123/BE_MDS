using MDS.Model.Entity;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MDS.Repository.Implement
{
    public class ProductRepository : IBaseRepository<Product>
    {
        private AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Product>> FindAll()
        {
            return _context.Products.ToListAsync();
        }

        public IQueryable<Product> FindByCondition(Expression<Func<Product, bool>>? filter = null)
        {
            IQueryable<Product> query = _context.Products;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public async Task<Product> FirstOrDefaultAsync(Expression<Func<Product, bool>>? filter = null)
        {
            IQueryable<Product> query = _context.Products;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(Product entity)
        {
            _context.Products.Remove(entity);
        }

        public void Update(Product entity)
        {
            _context.Products.Update(entity);
        }
    }

}
