using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace CRS.ADMIN.APPLICATION.CustomHelpers
{
    public static class ExternalApiCallHelpers
    {
        private static readonly HttpClient _client = new HttpClient();
        public static string CallApi(string url, HttpMethod method, HttpContent content = null)
        {
            using (var request = new HttpRequestMessage(method, url))
            {
                if (content != null)
                {
                    request.Content = content;
                }

                try
                {
                    HttpResponseMessage response =  _client.SendAsync(request).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode(); 
                    return  response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); 
                }
                catch (HttpRequestException ex)
                {
                   
                    throw new Exception($"Error calling API: {ex.Message}");
                }
            }
        }
    }
}
