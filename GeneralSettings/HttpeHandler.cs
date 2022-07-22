using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TechArchFillingSystem.GeneralSettings
{
    public static class HttpeHandler
    {
        public static async Task<HttpResponseMessage> PostAsync(string requestUrl, object data)
        {
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var httpClient = new HttpClient(clientHandler);
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage requestMessage = new(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

            return responseMessage;
        }
    }
}
