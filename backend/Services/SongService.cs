using backend.Dto;
using backend.Entity;
using backend.Services.AlbumCoverGeneration;
using backend.Services.ServiceResults;
using backend.Services.MusicGeneration;
using Bogus;
namespace backend.Services
{

    public class SongService : ISongService
    {
        private Faker<Song> _faker;
        private int seed = 123456789;
        private float likes = 5.0f;
        private string language = "en";

        public SongService()
        {
            InitializeFaker(this.language, this.seed);
        }

        private void InitializeFaker(string lang, int seedValue)
        {
            _faker = new Faker<Song>(lang).UseSeed(seedValue);
            _faker.RuleFor(s => s.Id, f => f.IndexFaker + 1);
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
            InitializeFaker(this.language, this.seed);
        }

        private string ResolveLanguage(string language)
        {
            return language.ToLower() switch
            {
                "en" => "en",
                "fr" => "fr",
                "es" => "es",
                _ => "en"
            };
        }


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
            _faker.RuleFor(s => s.Title, f => f.Random.Number(0, 5) switch
            {
                0 => $"{f.Commerce.Color()} {f.Commerce.Product()}",
                1 => f.Address.City(),
                2 => f.Commerce.Department(),
                3 => $"{f.Date.Weekday()} {f.Commerce.Product()}",
                4 => $"{f.Address.CityPrefix()} {f.Commerce.Color()}",
                _ => f.Commerce.ProductName()
            });
        }

        private void AddArtistRules()
        {
            _faker.RuleFor(s => s.Artist, f => f.Random.Number(0, 1) switch
            {
                0 => f.Name.FullName(),
                _ => $"{f.Name.FirstName()} {f.Name.LastName()}"
            });
        }

        private void AddAlbumRules()
        {
            _faker.RuleFor(s => s.Album, f => f.Random.Int(1, 3) switch
            {
                1 => f.Commerce.Product(),
                2 => f.Address.City(),
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

        public byte[] GenerateMusicForSong(int songId)
        {
            return Music.Generate(this.seed, songId);
        }

    }
}