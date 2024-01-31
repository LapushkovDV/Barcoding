using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TSD.Services.Interfaces
{
    public interface IHTTPClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
