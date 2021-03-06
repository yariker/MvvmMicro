using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MvvmMicro.Sample.Wpf.Services
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Sends a GET request to the specified URI, gets the response body as JSON, and deserializes
        /// the JSON into the given type.
        /// </summary>
        public static async Task<T> GetAsync<T>(this HttpClient client, string uri)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            string json = await client.GetStringAsync(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
