using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SearchEngine.WebApp
{
    /// <summary>
    /// Interface for a class that wrapps <see cref="HttpClient"/>.
    /// </summary>
    public interface IHttpClientWrapper
    {
        Tuple<T, HttpStatusCode> Get<T>(string requestUri) where T : class;
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
