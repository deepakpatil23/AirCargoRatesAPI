public class Module
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; }
    public string Url { get; set; }

    public int? ParentModuleId { get; set; }
    public Module ParentModule { get; set; }

    public ICollection<Module> ChildModules { get; set; }
    public ICollection<RoleModule> RoleModules { get; set; }
}
