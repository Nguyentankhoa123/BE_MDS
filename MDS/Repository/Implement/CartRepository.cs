using MDS.Model.Entity;
using MDS.Shared.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MDS.Repository.Implement
{
    public class CartRepository : IBaseRepository<Cart>
    {
        private AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Cart entity)
        {
            await _context.Carts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Cart>> FindAll()
        {
            return _context.Carts.ToListAsync();
        }

        public IQueryable<Cart> FindByCondition(Expression<Func<Cart, bool>>? filter = null)
        {
            IQueryable<Cart> query = _context.Carts;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public async Task<Cart> FirstOrDefaultAsync(Expression<Func<Cart, bool>>? filter = null)
        {
            IQueryable<Cart> query = _context.Carts;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(Cart entity)
        {
            _context.Carts.Remove(entity);
        }

        public void Update(Cart entity)
        {
            _context.Carts.Update(entity);
        }
    }
}
