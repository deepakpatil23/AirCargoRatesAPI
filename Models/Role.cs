public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleModule> RoleModules { get; set; }
}
