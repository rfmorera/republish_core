using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utils
{
    static class Requests
    {
        static string[] User_Agents = {
            "Mozilla//5.0 (Windows NT 6.1) AppleWebKit//537.36 (KHTML, like Gecko) Chrome//41.0.2228.0 Safari//537.36",
            "Mozilla//5.0 (Windows; U; Windows NT 5.2; en-US) AppleWebKit//525.13 (KHTML, like Gecko) Chrome//0.2.149.30 Safari//525.13",
            "Mozilla//5.0 (Macintosh; U; Intel Mac OS X 10_6_0; en-US) AppleWebKit//528.10 (KHTML, like Gecko) Chrome//2.0.157.2 Safari//528.10",
            "Mozilla//5.0 (X11; U; Linux x86_64; en-US) AppleWebKit//532.0 (KHTML, like Gecko) Chrome//3.0.195.24 Safari//532.0",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET CLR 2.0.50727; Media Center PC 6.0)",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; Media Center PC 6.0; InfoPath.2; MS-RTC LM 8",
            "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_6; en-gb) AppleWebKit/533.20.25 (KHTML, like Gecko) Version/5.0.4 Safari/533.20.27",
            "Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_3 like Mac OS X; en-gb) AppleWebKit/533.17.9 (KHTML, like Gecko) Version/5.0.2 Mobile/8F190 Safari/6533.18.5"
        };
        /// <summary>
        /// Post a formated json string
        /// </summary>
        /// <param name="requestUri">Url to send request</param>
        /// <param name="jsonData">string to post</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string requestUri, string jsonData)
        {
            StringContent httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return await PostAsync(requestUri, httpContent);
        }

        /// <summary>
        /// Post HttpContent to Uri
        /// </summary>
        /// <param name="requestUri">Url to send request</param>
        /// <param name="httpContent">Content to post</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string requestUri, StringContent httpContent)
        {
            WebException last = null;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        int pUa = DateTime.Now.Millisecond % User_Agents.Length;
                        client.Timeout = TimeSpan.FromSeconds(35);
                        client.DefaultRequestHeaders.Add("User-Agent", User_Agents[pUa]);
                        HttpResponseMessage responseHttp = await client.PostAsync(requestUri, httpContent);
                        return await responseHttp.Content?.ReadAsStringAsync();
                    }
                }
                catch (WebException ex)
                {
                    last = ex;
                    if (ex.Status == WebExceptionStatus.ConnectFailure ||
                        ex.Status == WebExceptionStatus.RequestCanceled ||
                        ex.Status == WebExceptionStatus.Timeout ||
                        ex.Status == WebExceptionStatus.ConnectionClosed ||
                        ex.Status == WebExceptionStatus.UnknownError ||
                        ex.Message.Contains("The operation was canceled"))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                        continue;
                    }
                    break;
                }
            }
            throw last;
        }

        /// <summary>
        /// Get data from requestUri
        /// </summary>
        /// <param name="requestUri">Url to get</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string requestUri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(35);
                HttpResponseMessage responseHttp = await client.GetAsync(requestUri);
                return await responseHttp.Content?.ReadAsStringAsync();
            }
        }
    }
}
