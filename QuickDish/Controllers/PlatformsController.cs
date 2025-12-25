using Microsoft.AspNetCore.Mvc;
using QuickDish.Client.Data.Models;
using QuickDish.Client.Data.Requests;

namespace QuickDish.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPlatforms([FromQuery] SearchPlatformsRequest request)
    {
        var fakePlatforms = new List<PlatformResponse>
        {
            new PlatformResponse { Id = Guid.NewGuid(), Name = "Facebook" },
            new PlatformResponse { Id = Guid.NewGuid(), Name = "Tiktok" }
        };

        return Ok(fakePlatforms.Where(p => p.Name.Contains(request?.SearchText ?? "")));
    }
}