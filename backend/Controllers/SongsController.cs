using Microsoft.AspNetCore.Mvc;
using backend.Services;
namespace backend.Controllers;

[ApiController]
[Route("/api/songs")]
public class SongsController(ISongService songService) : ControllerBase
{
    private readonly ISongService _songService = songService;

    [HttpGet]
    public async Task<IActionResult> GetSongs(
        [FromQuery] int seed = 0,
        [FromQuery] string language = "en",
        [FromQuery] float likes = 0f,
        [FromQuery] int page = 0
    )
    {
        _songService.UpdateParameters(likes, seed, language, page);
        var result = await _songService.GetSongs(12);
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        else
        {
            return StatusCode(500, result.Message);
        }
    }

    [HttpGet("{songId}/audio")]
    public IActionResult GetSongAudio(
        int songId,
        [FromQuery] int seed = 0
    )
    {
        Console.WriteLine($"Generating audio for songId: {songId} with seed: {seed}");
        _songService.UpdateParameters(0f, seed, "en", 0);
        var audioBytes = _songService.GenerateMusicForSong(songId);
        return File(audioBytes, "audio/wav");
    }
}
