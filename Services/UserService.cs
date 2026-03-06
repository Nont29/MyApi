using Microsoft.EntityFrameworkCore;
using MyApi.Data.DbContexts;
using MyApi.Dto;
using MyApi.Model;

namespace MyApi.Services
{
    public interface IUserService
    {
        Task<PageResult<UsersResponse>> GetAsync(string? keyword, int Page = 1, int PageSize = 10);
        Task<UserResponse?> GetByIdAsync(int UserId);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext db;

        public UserService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<PageResult<UsersResponse>> GetAsync(string? keyword, int Page = 1, int PageSize = 10)
        {
            Page = Math.Max(Page, 1);
            PageSize = Math.Clamp(PageSize, 1, 200);

            var BaseQuery = from u in db.Users.AsNoTracking()
                            join ru in db.RoleUsers.AsNoTracking() on u.UserId equals ru.UserId into ruJoin
                            from ru in ruJoin.DefaultIfEmpty()
                            join r in db.Roles.AsNoTracking() on ru.RoleId equals r.RoleId into rJoin
                            from r in rJoin.DefaultIfEmpty()
                            select new { u, ru, r };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var _keyword = keyword.Trim();
                BaseQuery = BaseQuery.Where(x => (x.u.Firstname != null && x.u.Firstname.Contains(_keyword)) ||
                                                 (x.u.Lastname != null && x.u.Lastname.Contains(_keyword)) ||
                                                 (x.u.Username != null && x.u.Username.Contains(_keyword)) ||
                                                 (x.u.Email != null && x.u.Email.Contains(_keyword)));
            }

            var Total = await BaseQuery.CountAsync();

            var Items = await BaseQuery.OrderBy(x => x.u.UserId)
                                .Skip((Page - 1) * PageSize)
                                .Take(PageSize)
                                .Select(x => new UsersResponse
                                {
                                    UserId = x.u.UserId,
                                    Name = x.u.Firstname +" "+ x.u.Lastname,
                                    UserName = x.u.Username,
                                    Email = x.u.Email ?? "-",
                                    Role = x.r.RoleName,
                                    InActive = x.u.IsActive
                                }).Distinct().ToListAsync();

            return new PageResult<UsersResponse>
            {
                Page = Page,
                PageSize = PageSize,
                TotalItems = Total,
                Items = Items
            };
        }

        public async Task<UserResponse?> GetByIdAsync(int UserId)
        {
            var BaseQuery = from u in db.Users.AsNoTracking()
                            join ru in db.RoleUsers.AsNoTracking() on u.UserId equals ru.UserId
                            join r in db.Roles.AsNoTracking() on ru.RoleId equals r.RoleId
                            where u.UserId == UserId
                            select new UserResponse
                            {
                                UserId = u.UserId,
                                Firstname = u.Firstname,
                                Lastname = u.Lastname,
                                Username = u.Username,
                                Email = u.Email,
                                IsActive = u.IsActive,
                                RoleId = r.RoleId,
                                RoleName = r.RoleName
                            };

            return await BaseQuery.FirstOrDefaultAsync();
        }
    }
}
