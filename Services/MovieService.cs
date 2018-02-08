using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace Netflix
{
    public class MovieService
    {
        public static readonly int MinSearchLength = 5;

        private const string Url = "http://netflixroulette.net/api/api.php";
        private HttpClient _client = new HttpClient();

        public async Task<IEnumerable<Movie>> FindMoviesByActor(string actor)
        {
            if (actor.Length < MinSearchLength)
                return Enumerable.Empty<Movie>();

            var response = await _client.GetAsync($"{Url}?actor={actor}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Enumerable.Empty<Movie>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Movie>>(content);
        }

        public async Task<Movie> GetMovie(string title)
        {
            var response = await _client.GetAsync($"{Url}?title={title}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Movie>(content);
        }
    }
}
