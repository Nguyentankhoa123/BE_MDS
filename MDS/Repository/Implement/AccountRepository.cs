using MDS.Model.Entity;
using MDS.Shared.Database.DbContext;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace MDS.Repository.Implement
{
    public class AccountRepository : IBaseRepository<ApplicationUser>
    {
        private AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        public AccountRepository(AppDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;

        }
        public async Task AddAsync(ApplicationUser entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<ApplicationUser>> FindAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ApplicationUser> FindByCondition(Expression<Func<ApplicationUser, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public void Remove(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
