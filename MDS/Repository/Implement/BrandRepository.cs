using MDS.Model.Entity;
using MDS.Shared.Database.DbContext;
using System.Linq.Expressions;

namespace MDS.Repository.Implement
{
    public class BrandRepository : IBaseRepository<Brand>
    {
        private AppDbContext _context;
        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Brand entity)
        {
            await _context.Brands.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Brand>> FindAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Brand> FindByCondition(Expression<Func<Brand, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Brand> FirstOrDefaultAsync(Expression<Func<Brand, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public void Remove(Brand entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Brand entity)
        {
            throw new NotImplementedException();
        }
    }
}
