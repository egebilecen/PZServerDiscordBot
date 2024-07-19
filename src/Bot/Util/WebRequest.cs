using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EB_Utility
{
    public static class WebRequest
    {
        public static HttpClient CreateHTTPClient(double connectionTimeout=10.0, string userAgent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36")
        {
            CookieContainer cookieContainer     = new CookieContainer();
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip 
                                       | DecompressionMethods.Deflate
            };

            httpClientHandler.AllowAutoRedirect = true;
            httpClientHandler.UseCookies        = true;
            httpClientHandler.CookieContainer   = cookieContainer;

            HttpClient httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(connectionTimeout);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "600");

            return httpClient;
        }

        // TaskCanceledException     - Request timeouted
        // InvalidOperationException - Invalid URL
        public static async Task<string> GetAsync(HttpClient httpClient, string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if(response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            return null;
        }

        // TaskCanceledException     - Request timeouted
        // InvalidOperationException - Invalid URL
        public static async Task<string> PostAsync(HttpClient httpClient, string url, List<KeyValuePair<string, string>> paramList)
        {
            var content = new FormUrlEncodedContent(paramList);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseContent = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
                return responseContent;

            Logger.WriteLog($"WebRequest.PostAsync() - Response status code is not 200. Status code: {response.StatusCode} | URL: {url} | Body: {responseContent}");
            return null;
        }
    }
}