using backend.Dto;
using backend.Entity;
using backend.Services.ServiceResults;
using Bogus;

namespace backend.Services
{
    public class SongService : ISongService
    {
        private Faker<Song> _faker;

        public SongService()
        {
            ConfigureFaker();
        }

        public void ChangeLanguage(string language)
        {
            ConfigureFaker(language);
        }

        public void ChangeSeed(int seed)
        {
            ConfigureFaker(seed: seed);
        }

        private void ConfigureFaker(string language = "en", int seed = 1234)
        {
            _faker = new Faker<Song>(language).UseSeed(seed)
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
            _faker.RuleFor(s => s.Title, f =>
            {
                if (f.Random.Bool())
                    return $"{f.Commerce.ProductAdjective()} {f.Commerce.ProductMaterial()} {f.Hacker.Noun()}";
                else
                    return $"{f.Random.Word()} {f.Hacker.IngVerb()}";
            });

        }

        private void AddArtistRules()
        {
            _faker.RuleFor(s => s.Artist, f => f.Person.FullName);
        }

        private void AddAlbumRules()
        {
            _faker.RuleFor(s => s.Album, f => f.Random.Int(1, 3) switch
            {
                1 => $"{f.Commerce.Color()} {f.Address.CitySuffix()}",
                2 => $"{f.Date.Weekday()} {f.Name.Suffix()}",
                _ => $"{f.Hacker.IngVerb()} {f.Address.StreetName()}"
            });
        }

        private void AddGenreRules()
        {
            _faker.RuleFor(s => s.Genre, f => f.Music.Genre());
        }


        private List<Song> GenerateSongs(int count)
        {
            List<Song> songs = _faker.Generate(count);
            return songs;
        }

        public ServiceResult<SongsResponseDto> GetSongs(int count)
        {
            try
            {
                var response = new SongsResponseDto { Songs = GenerateSongs(count), success = true };
                return ServiceResult<SongsResponseDto>.Success(response, "Songs generated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<SongsResponseDto>.Failure($"An error occurred while generating songs: {ex.Message}", "SONG_GENERATION_ERROR");
            }
        }
    }
}