using backend.Dto;
using backend.Entity;
using backend.Services.AlbumCoverGeneration;
using backend.Services.ServiceResults;
using Bogus;
using Bogus.Hollywood;

namespace backend.Services
{

    public enum Language
    {
        En,
        Es
    }

    public class SongService : ISongService
    {
        private readonly Faker<Song> _faker;
        private int seed = 123456789;
        private float likes = 5.0f;
        private Language language = Language.En;

        public SongService()
        {
            _faker = new Faker<Song>().UseSeed(seed)
                .RuleFor(s => s.Id, f => f.IndexFaker + 1);
            ConfigureFakerRules();
        }
        public async Task<ServiceResult<SongsResponseDto>> GetSongs(int count)
        {
            try
            {
                var response = new SongsResponseDto
                {
                    Songs = await GenerateSongs(count),
                    success = true
                };
                return ServiceResult<SongsResponseDto>.Success(response, "Songs generated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<SongsResponseDto>.Failure($"An error occurred while generating songs: {ex.Message}", "SONG_GENERATION_ERROR");
            }
        }

        public void UpdateParameters(float likes, int seed, string language, int page)
        {
            this.likes = likes;
            this.language = ResolveLanguage(language);
            unchecked
            {
                this.seed = HashCode.Combine(seed, page);
            }
            _faker.UseSeed(this.seed);
        }

        public Language ResolveLanguage(string lang) => lang.ToLower() switch
        {
            "en" => Language.En,
            "es" => Language.Es,
            _ => Language.En,
        };

        private void ConfigureFakerRules()
        {
            AddTitleRules();
            AddArtistRules();
            AddAlbumRules();
            AddGenreRules();
            AddLikesRules();
            _faker.FinishWith((f, s) =>
            {
                s.Title = Capitalize(s.Title);
                s.Artist = Capitalize(s.Artist);
                s.Album = Capitalize(s.Album);
            });
        }

        private void AddLikesRules()
        {
            if (likes < 0 || likes > 10)
            {
                throw new ArgumentException("likes must be between 0 and 10");
            }
            _faker.RuleFor(s => s.likes, f =>
            {
                int hi = (int)Math.Ceiling((double)likes);
                int lo = (int)Math.Floor((double)likes);
                if (hi == lo) return hi;
                if (f.Random.Double() <= GetFractionalPart(likes)) return lo;
                return hi;
            });
        }

        private float GetFractionalPart(float likes) => likes - (float)Math.Floor((double)likes);

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
            });
        }

        private void AddArtistRules()
        {
            _faker.RuleFor(s => s.Artist, f => f.Random.Number(0, 3) switch
            {
                0 => $"{f.Person.FullName}",
                1 => $"{f.Person.FirstName} {f.Name.Suffix()}",
                _ => $"{f.Person.FirstName} {f.Name.Suffix()}",
            });
        }

        private void AddAlbumRules()
        {
            _faker.RuleFor(s => s.Album, f => f.Random.Int(1, 3) switch
            {
                1 => $"{f.Hacker.Noun()}",
                2 => $"{f.Hacker.IngVerb()}",
                3 => $"{f.Commerce.Product()}",
                _ => "Single"
            });
        }

        private void AddGenreRules()
        {
            _faker.RuleFor(s => s.Genre, f => f.Music.Genre());
        }

        private static string Capitalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            return char.ToUpper(text[0]) + text[1..];
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
                    Title = song.Album == "Single" ? song.Title : song.Album,
                    Subtitle = song.Artist,
                };
                song.CoverImageBase64 = await AlbumCover.Make(coverParams, new Random(HashCode.Combine(seed, song.Id)));
            }
        }


    }
}