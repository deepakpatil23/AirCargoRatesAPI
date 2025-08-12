using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("menu/user/{userId}")]
    public async Task<IActionResult> GetMenuByUser(int userId)
    {
        // Get roles for the user
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // Get modules assigned to those roles
        var moduleIds = await _context.RoleModules
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.ModuleId)
            .Distinct()
            .ToListAsync();

        var modules = await _context.Modules
            .Where(m => moduleIds.Contains(m.ModuleId))
            .ToListAsync();

        // Collect parent modules recursively
        var allModules = new List<Module>(modules);
        var parentIds = modules.Select(m => m.ParentModuleId).Where(id => id != null).Distinct().ToList();

        while (parentIds.Any())
        {
            var parentModules = await _context.Modules
                .Where(m => parentIds.Contains(m.ModuleId))
                .ToListAsync();

            foreach (var pm in parentModules)
            {
                if (!allModules.Any(m => m.ModuleId == pm.ModuleId))
                {
                    allModules.Add(pm);
                }
            }

            parentIds = parentModules.Select(p => p.ParentModuleId)
                .Where(id => id != null && !allModules.Any(m => m.ModuleId == id))
                .Distinct().ToList();
        }

        // Add static Home and Logout
        allModules.Add(new Module { ModuleId = 0, ModuleName = "Home", Url = "/home", ParentModuleId = null });
        allModules.Add(new Module { ModuleId = 9999, ModuleName = "Logout", Url = "/logout", ParentModuleId = null });

        // Remove duplicates
        var result = allModules.GroupBy(m => m.ModuleId).Select(g => g.First()).ToList();

        return Ok(result);
    }
}
