using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;

namespace AirCargoRatesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DiagnosticController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    return Ok("✅ Connection to SQL Server successful.");
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"❌ SQL Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Exception: {ex.Message}");
            }
        }

        [HttpGet("check-table")]
        public async Task<IActionResult> CheckSampleTable()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM YourTableNameHere", conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return Ok("✅ Table exists and has data.");
                            }
                            else
                            {
                                return Ok("⚠️ Table exists but has no data.");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"❌ SQL Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ General Exception: {ex.Message}");
            }
        }
    }
}
