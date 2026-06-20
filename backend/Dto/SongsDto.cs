using backend.Entity;

namespace backend.Dto
{
    public class SongsGenerationParametersDto
    {
        public int Seed { get; set; }
        public float Likes { get; set; }
        public required string Language { get; set; }
    }




    public class SongsResponseDto
    {
        public List<Song> Songs { get; set; } = [];
        public bool success { get; set; }
    }
}