using backend.Dto;
using backend.Services.ServiceResults;

namespace backend.Services
{
    public interface ISongService
    {
        ServiceResult<SongsResponseDto> GetSongs(int count);
    }
}