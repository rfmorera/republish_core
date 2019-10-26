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
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/7046A194A",
            "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_7; da-dk) AppleWebKit/533.21.1 (KHTML, like Gecko) Version/5.0.5 Safari/533.21.1",
            "Mozilla/5.0 (Windows; U; Windows NT 6.0; zh-TW) AppleWebKit/530.19.2 (KHTML, like Gecko) Version/4.0.2 Safari/530.19.1",
            "Mozilla/5.0 (Windows NT 5.1) Gecko/20100101 Firefox/14.0 Opera/12.0",
            "Opera/9.80 (Windows NT 5.1; U; en) Presto/2.9.168 Version/11.51",
            "Opera/9.80 (X11; Linux x86_64; U; pl) Presto/2.7.62 Version/11.00",
            "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/4.0; InfoPath.2; SV1; .NET CLR 2.0.50727; WOW64)",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; Zune 4.0; Tablet PC 2.0; InfoPath.3; .NET4.0C; .NET4.0E)",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:21.0) Gecko/20130331 Firefox/21.0",
            "Mozilla/6.0 (Windows NT 6.2; WOW64; rv:16.0.1) Gecko/20121011 Firefox/16.0.1",
            "Mozilla/5.0 (compatible; Windows; U; Windows NT 6.2; WOW64; en-US; rv:12.0) Gecko/20120403211507 Firefox/12.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246",
            "Mozilla/5.0 (Linux; U; Android 4.0.3; ko-kr; LG-L160L Build/IML74K) AppleWebkit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
            "Mozilla/5.0 (Linux; U; Android 2.2.1; en-ca; LG-P505R Build/FRG83) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1",

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
                        if(responseHttp.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(30));
                            continue;
                        }
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
                        ex.Message.Contains("The operation was canceled") ||
                        ex.Message.Contains("The web server reported a bad gateway error."))
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
