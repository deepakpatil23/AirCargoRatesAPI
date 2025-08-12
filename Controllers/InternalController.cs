using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirCargoRatesAPI.Attributes;

[InternalApi]
[ApiController]
[Route("api/internal/[controller]")]
public class InternalController : ControllerBase
{
    // Will be hidden in Swagger
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hidden from Swagger");
    }
    
}
