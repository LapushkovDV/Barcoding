using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TSD.Services.Interfaces;
using Xamarin.Forms;

namespace TSD.Services.Rest
{
    public enum HttpMethodRest
    {
        GET,
        POST
    }
    /// <summary>
    /// Сервис передачи и принятия данных по HTTP протоколу
    /// </summary>
    public static class RestService
    {
        #region Приватные поля
        /// <summary>
        /// Статическое поле типа контента
        /// </summary>
        private static readonly string _contentType = Resources.ContentTypeJson;
        #endregion

        #region Публичные поля
        /// <summary>
        /// POST - метод для отправки и принятия данных
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<JObject> SendRequest(string uri, HttpMethodRest typeHttp = HttpMethodRest.POST, JObject context = null, string token = null, double minutes = 10)
        {
            try
            {
                using (var client = new HttpClient(DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler())

                {
                    Timeout = TimeSpan.FromMinutes(minutes)
                })
                {
                    var message = new HttpRequestMessage(typeHttp == HttpMethodRest.POST ? HttpMethod.Post : HttpMethod.Get, uri);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_contentType));

                    if (typeHttp == HttpMethodRest.POST)
                        message.Content = new StringContent(JsonConvert.SerializeObject(context), Encoding.UTF8, _contentType);

                    if (token != null)
                        message.Headers.Authorization = new AuthenticationHeaderValue(Resources.RestKeyAuthentificationBearer, token);

                    try
                    {
                        var response = await client.SendAsync(message);

                        var responseBody = await response.Content.ReadAsStringAsync();

                        return JObject.Parse(responseBody);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Компрессия данных (два метода компрессии: gzip, deflate)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isDeflate"></param>
        /// <returns></returns>
        public static string Compress(string data, bool isDeflate = true)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var memoryStreamInput = new MemoryStream(dataBytes))
            using (var memoryStreamOutput = new MemoryStream())
            {
                if (isDeflate)
                {
                    using (var gs = new DeflateStream(memoryStreamOutput, CompressionMode.Compress))
                    {
                        memoryStreamInput.CopyTo(gs);
                    }
                }
                else
                {
                    using (var gs = new GZipStream(memoryStreamOutput, CompressionMode.Compress))
                    {
                        memoryStreamInput.CopyTo(gs);
                    }
                }

                return Convert.ToBase64String(memoryStreamOutput.ToArray());
            }
        }

        /// <summary>
        /// Декомпрессия данных (два метода декомпрессии: gzip, deflate)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isDeflate"></param>
        /// <returns></returns>
        public static string Decompress(string data, bool isDeflate = true)
        {
            var dataBytes = Convert.FromBase64String(data);

            using (var memoryStreamInput = new MemoryStream(dataBytes))
            using (var memoryStreamOutput = new MemoryStream())
            {
                if (isDeflate)
                {
                    using (var gs = new DeflateStream(memoryStreamInput, CompressionMode.Decompress))
                    {
                        gs.CopyTo(memoryStreamOutput);
                    }
                }
                else
                {
                    using (var gs = new GZipStream(memoryStreamInput, CompressionMode.Decompress))
                    {
                        gs.CopyTo(memoryStreamOutput);
                    }
                }
                return Encoding.UTF8.GetString(memoryStreamOutput.ToArray()).Trim(new char[] { '\uFEFF', '\u200B' });
            }
        }

        /// <summary>
        /// Тестовое подключение к gettoken сервера для проверки соединения
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<bool> TestConnect(string url)
        {
            var requestJson = new JObject
                {
                    { Resources.RestKeyLogin, string.Empty },
                    { Resources.RestKeyPassword, string.Empty },
                    { Resources.RestKeyImei, string.Empty }
                };

            var response = await SendRequest(url + Resources.UrlGetToken, HttpMethodRest.POST, requestJson, null, 0.1);

            if (response != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
