using System.Net.Http;
using TSD.Droid.Services.Dependency;
using TSD.Services.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(HTTPClientHandlerCreationService_Android))]
namespace TSD.Droid.Services.Dependency
{
    public class HTTPClientHandlerCreationService_Android : IHTTPClientHandlerCreationService
    {
        public HttpClientHandler GetInsecureHandler()
        {
            return new IgnoreSSLClientHandler();
        }
    }
}