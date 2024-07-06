using MDS.Model.Entity;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MDS.Repository.Implement
{
    public class CategoryRepository : IBaseRepository<Category>
    {
        private AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Category>> FindAll()
        {
            return _context.Categories.ToListAsync();
        }

        public IQueryable<Category> FindByCondition(Expression<Func<Category, bool>>? filter = null)
        {
            IQueryable<Category> query = _context.Categories;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public async Task<Category> FirstOrDefaultAsync(Expression<Func<Category, bool>>? filter = null)
        {
            IQueryable<Category> query = _context.Categories;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(Category entity)
        {
            _context.Categories.Remove(entity);
        }

        public void Update(Category entity)
        {
            _context.Categories.Update(entity);
        }
    }
}
