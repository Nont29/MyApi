namespace MyApi.Dto
{
    public class UsersResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool InActive { get; set; }
    }


    public class UserResponse
    {
        public int UserId { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }

    public class UserCreateRequest
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public int RoleId { get; set; }
    }


    public class UserUpdateRequest
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public int RoleId { get; set; }
    }

    public class RoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
