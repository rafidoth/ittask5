using Microsoft.AspNetCore.Mvc;
using backend.Services;
namespace backend.Controllers;

[ApiController]
[Route("/api")]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;
    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet("songs")]
    public IActionResult GetSongs()
    {
        var result = _songService.GetSongs(10);
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
