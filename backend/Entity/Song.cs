using Bogus;

namespace backend.Entity
{
    public class Song
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Artist { get; set; }
        public required string Album { get; set; }
        public required string Genre { get; set; }
    }
}