using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchEngine.WebApp
{
    /// <summary>
    /// Wrapper class for <see cref="HttpClient"/> to make it mockable.
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        public HttpClient httpClient;

        public HttpClientWrapper(string baseAddress, TimeSpan? timeout = null)
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress),
                Timeout = timeout.HasValue ? timeout.Value : TimeSpan.FromMinutes(3),
            };
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await httpClient.GetAsync(requestUri);
        }

        public Tuple<T, HttpStatusCode> Get<T>(string requestUri) where T : class
        {
            T result = null;

            var response = GetAsync(requestUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
            }

            return new Tuple<T, HttpStatusCode>(result, response.StatusCode);
        }
    }
}
