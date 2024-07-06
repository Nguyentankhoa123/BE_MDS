using MDS.Model.Entity;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MDS.Repository.Implement
{
    public class DiscountRepository : IBaseRepository<Discount>
    {
        private AppDbContext _context;
        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Discount entity)
        {
            await _context.Discounts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Discount>> FindAll()
        {
            return _context.Discounts.ToListAsync();
        }

        public IQueryable<Discount> FindByCondition(Expression<Func<Discount, bool>>? filter = null)
        {
            IQueryable<Discount> query = _context.Discounts;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public async Task<Discount> FirstOrDefaultAsync(Expression<Func<Discount, bool>>? filter = null)
        {
            IQueryable<Discount> query = _context.Discounts;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(Discount entity)
        {
            _context.Discounts.Remove(entity);
        }

        public void Update(Discount entity)
        {
            _context.Discounts.Update(entity);
        }
    }
}
