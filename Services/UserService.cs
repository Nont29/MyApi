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
        Task<UserResponse> CreateAsync(UserCreateRequest Request);
        Task<UserResponse> UpdateAsync(int UserId ,UserUpdateRequest Request);
        Task<bool> DeleteAsync(int UserId);
        Task<List<RoleResponse>> GetRoleList();
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext db;

        public UserService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<UserResponse> CreateAsync(UserCreateRequest Request)
        {
            var Username = Request.Username.Trim();
            var IsDuplicate = await db.Users.AnyAsync(x => x.Username == Username);
            if (IsDuplicate)
            {
                throw new InvalidOperationException("มีชื่อผู้ใช้งานในระบบแล้ว");
            }

            var user = new User
            {
                Firstname = Request.Firstname.Trim(),
                Lastname = Request.Lastname.Trim(),
                Username = Username,
                Email = string.IsNullOrWhiteSpace(Request.Email) ? null : Request.Email.Trim(),
                IsActive = Request.IsActive
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var Role = await db.Roles.FirstOrDefaultAsync(x => x.RoleId == Request.RoleId);    

            var roleUser = new RoleUser
            {
                UserId = user.UserId,
                RoleId = Request.RoleId
            };

            db.RoleUsers.Add(roleUser);
            await db.SaveChangesAsync();

            return new UserResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                RoleId = roleUser.RoleId,
                RoleName = Role.RoleName      
            };
        }

        public async Task<bool> DeleteAsync(int UserId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (user is null)
            {
                return false;
            }

            var RoleUser = await db.RoleUsers.Where(x => x.UserId == UserId).ToListAsync();
            if (RoleUser.Count > 0)
            {
                db.RoleUsers.RemoveRange(RoleUser);
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return true;
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

        public async Task<List<RoleResponse>> GetRoleList()
        {
            var RoleList = await db.Roles.Select(x => new RoleResponse
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToListAsync();

            return RoleList;
        }

        public async Task<UserResponse> UpdateAsync(int UserId ,UserUpdateRequest Request)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (user is null)
            {
                return null;
            }

            var Username = Request.Username.Trim();

            var IsDuplicate = await db.Users.AnyAsync(x => x.Username == Username && x.UserId != UserId);
            if (IsDuplicate)
            {
                throw new InvalidOperationException("มีชื่อผู้ใช้ซ้ำในระบบแล้ว");
            }

            var Role = await db.Roles.FirstOrDefaultAsync(x => x.RoleId == Request.RoleId);


            user.Firstname = Request.Firstname;
            user.Lastname = Request.Lastname;
            user.Username=  Username;
            user.Email = string.IsNullOrWhiteSpace(Request.Email) ? null : Request.Email.Trim();
            user.IsActive = Request.IsActive;

            var RoleUser = await db.RoleUsers.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (RoleUser is null)
            {
                db.RoleUsers.Add(new RoleUser
                {
                    RoleId = Request.RoleId,
                    UserId = UserId
                });
            }
            else
            {
                RoleUser.RoleId = Request.RoleId;
            }

            await db.SaveChangesAsync();

            return new UserResponse
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                RoleId = Role.RoleId,
                RoleName = Role.RoleName
            };
        }
    }
}
