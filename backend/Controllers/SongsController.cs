using Microsoft.AspNetCore.Mvc;
using backend.Services;
namespace backend.Controllers;

[ApiController]
[Route("/api")]
public class SongsController(ISongService songService) : ControllerBase
{
    private readonly ISongService _songService = songService;

    [HttpGet("songs")]
    public async Task<IActionResult> GetSongs(
        [FromQuery] int seed = 0,
        [FromQuery] string language = "en",
        [FromQuery] float likes = 0f
    )
    {
        _songService.UpdateParameters(likes, seed, language);
        var result = await _songService.GetSongs(15);
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
