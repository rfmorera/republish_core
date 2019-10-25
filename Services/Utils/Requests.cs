﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utils
{
    static class Requests
    {
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
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(35);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                HttpResponseMessage responseHttp = await client.PostAsync(requestUri, httpContent);
                return await responseHttp.Content?.ReadAsStringAsync();
            }
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
