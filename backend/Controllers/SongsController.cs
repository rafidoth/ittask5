using Microsoft.AspNetCore.Mvc;
using backend.Services;
namespace backend.Controllers;

[ApiController]
[Route("/api")]
public class SongsController(ISongService songService) : ControllerBase
{
    private readonly ISongService _songService = songService;

    [HttpGet("songs")]
    public async Task<IActionResult> GetSongs()
    {
        var result = await _songService.GetSongs(10);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        else
        {
            return StatusCode(500, result.Message);
        }
    }
}
