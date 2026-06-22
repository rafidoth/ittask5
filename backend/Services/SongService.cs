using backend.Dto;
using backend.Entity;
using backend.Services.AlbumCoverGeneration;
using backend.Services.ServiceResults;
using Bogus;

namespace backend.Services
{
    public class SongService : ISongService
    {
        private readonly Faker<Song> _faker;
        private int seed = 123456789;

        public SongService()
        {
            _faker = new Faker<Song>().UseSeed(seed)
                .RuleFor(s => s.Id, f => f.IndexFaker + 1);
            ConfigureFakerRules();
        }


        private void ConfigureFakerRules()
        {
            AddTitleRules();
            AddArtistRules();
            AddAlbumRules();
            AddGenreRules();
        }

        private void AddTitleRules()
        {
            _faker.RuleFor(s => s.Title, f => f.Random.Number(0, 13) switch
            {
                0 => $"{f.Commerce.Color()} {f.Commerce.Product()}",
                1 => $"{f.Commerce.Color()} {f.Date.Weekday()}",
                2 => $"{f.Address.CityPrefix()} night",
                3 => $"The {f.Address.City()}",
                4 => $"{f.Hacker.Adjective()} day",
                5 => $"That {f.Commerce.Product()}",
                6 => $"{f.Hacker.Adjective()} Heart",
                7 => $"Lost in {f.Address.City()}",
                8 => $"{f.Commerce.Color()} Sky",
                9 => $"Broken {f.Commerce.Product()}",
                10 => $"{f.Date.Month()} {f.Hacker.Noun()}",
                11 => $"Midnight {f.Address.City()}",
                _ => $"The {f.Date.Month()} We {f.Hacker.Verb()}",
            }).FinishWith((f, s) => s.Title = char.ToUpper(s.Title[0]) + s.Title[1..]);
        }

        private void AddArtistRules()
        {
            _faker.RuleFor(s => s.Artist, f => f.Random.Number(0, 3) switch
            {
                0 => $"{f.Person.FullName}",
                1 => $"{f.Person.FirstName} {f.Name.Suffix()}",
                _ => $"{f.Person.FirstName} {f.Name.Suffix()}",
            }).FinishWith((f, s) => s.Title = char.ToUpper(s.Title[0]) + s.Title[1..]);
        }

        private void AddAlbumRules()
        {
            _faker.RuleFor(s => s.Album, f => f.Random.Int(1, 3) switch
            {
                1 => $"{f.Hacker.Adjective()} {f.Hacker.Noun()}",
                2 => $"{f.Hacker.IngVerb()} {f.Hacker.Noun()}",
                _ => $"{f.Hacker.Adjective()} {f.Hacker.IngVerb()}"
            });
        }

        private void AddGenreRules()
        {
            _faker.RuleFor(s => s.Genre, f => f.Music.Genre());
        }


        private async Task<List<Song>> GenerateSongs(int count)
        {
            try
            {
                List<Song> songs = _faker.Generate(count);
                await AttachCoverImage(songs);
                return songs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating songs: {ex.Message}");
            }
        }

        private async Task AttachCoverImage(List<Song> songs)
        {
            foreach (var song in songs)
            {
                var coverParams = new AlbumCoverParams
                {
                    Title = song.Title,
                    Artist = song.Artist,
                    Album = song.Album
                };
                song.CoverImageBase64 = await AlbumCover.Make(coverParams, new Random(seed + song.Id));
            }
        }

        public async Task<ServiceResult<SongsResponseDto>> GetSongs(int count)
        {
            try
            {
                var response = new SongsResponseDto { Songs = await GenerateSongs(count), success = true };
                return ServiceResult<SongsResponseDto>.Success(response, "Songs generated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<SongsResponseDto>.Failure($"An error occurred while generating songs: {ex.Message}", "SONG_GENERATION_ERROR");
            }
        }
    }
}