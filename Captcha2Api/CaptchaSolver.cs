using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Captcha2Api.Exceptions;

namespace Captcha2Api
{
    public static class Captcha2Solver
    {
        private const string BASE_URL = "http://2captcha.com";
        private const string USER_AGENT = "csharpClient1.0";
        private const int TIMEOUT = 30000;

        /// <summary>
        /// Get account's balance
        /// </summary>
        /// <returns></returns>
        public static string account_balance(string _access_token)
        {
            var url = string.Format("{0}/user/balance?access_token={1}", BASE_URL, _access_token);
            var resp = Utils.GET(url, USER_AGENT, TIMEOUT);
            dynamic d = JObject.Parse(resp);
            return string.Format("${0}", d.balance.ToString());
        }

        /// <summary>
        /// Submit image captcha
        /// </summary>
        /// <param name="opts"></param>
        /// <returns>captchaID</returns>
        public static string submit_image_captcha(string _access_token, Dictionary<string, string> opts)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var url = string.Format("{0}/captcha/image", BASE_URL);
            dict.Add("access_token", _access_token);
            var image = "";
            // if no b64 string was given, but image path instead
            if (File.Exists(opts["image"])) image = Utils.read_captcha_image(opts["image"]);
            else image = opts["image"];
            dict.Add("b64image", image);
            // check case sensitive
            if (opts.ContainsKey("case_sensitive"))
            {
                if (opts["case_sensitive"].Equals("true")) dict.Add("case_sensitive", "1");
            }
            // affiliate ID
            if (opts.ContainsKey("affiliate_id")) dict.Add("affiliate_id", opts["affiliate_id"]);
            var data = JsonConvert.SerializeObject(dict);
            var resp = Utils.POST(url, data, USER_AGENT, TIMEOUT);
            dynamic d = JObject.Parse(resp);
            return d.id.ToString();
        }
        /// <summary>
        /// Submit image captcha
        /// </summary>
        /// <param name="opts"></param>
        /// <returns>captchaID</returns>
        public static async Task<string> submit_recaptcha(string _access_token, string _uri, string siteKey, bool v3)
        {
            WebException last = null;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    var uri2Captcha = string.Format("{0}/in.php", BASE_URL);
                    var postData = "";
                    postData += "key=" + _access_token;
                    postData += "&method=userrecaptcha";
                    postData += "&googlekey=" + siteKey;
                    postData += "&pageurl=" + _uri;
                    if (v3) {
                        postData += "&version=v3";
                    }

                    var data = Encoding.ASCII.GetBytes(postData);

                    WebRequest request = WebRequest.Create(uri2Captcha);

                    request.Method = "POST";

                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());

                    var responseString = streamReader.ReadToEnd();
                    streamReader.Close();
                    // streamReader.Dispose();

                    response.Close();
                    //response.Dispose();

                    return responseString.Substring(3, responseString.Length - 3);
                }
                catch (WebException ex)
                {
                    last = ex;
                    if (ex.Status != WebExceptionStatus.ConnectFailure)
                    {
                        break;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
            throw last;
        }

        /// <summary>
        /// Retrieve captcha text / gresponse using captcha ID
        /// </summary>
        /// <param name="captchaid"></param>
        /// <returns></returns>
        public static async Task<string> retrieve(string _access_token, string captchaid)
        {
            string answerUrl = "http://2captcha.com/res.php?key=" + _access_token + "&action=get&id=" + captchaid;

            HttpClient client = new HttpClient();
            HttpResponseMessage responseClient = await client.GetAsync(answerUrl);
            string responseString = await responseClient.Content.ReadAsStringAsync();
            responseClient.Dispose();

            if (responseString.Substring(0, 2) == "OK")
            {
                return responseString.Substring(3, responseString.Length - 3);
            }
            else if(responseString == "ERROR_CAPTCHA_UNSOLVABLE")
            {
                throw new BadCaptchaException("ERROR_CAPTCHA_UNSOLVABLE");
            }

            return null;
        }

        /// <summary>
        /// Set captcha bad
        /// </summary>
        /// <param name="captchaid"></param>
        /// <returns></returns>
        public static string set_captcha_bad(string _access_token, string captchaid)
        {
            string url = "http://2captcha.com/res.php?key=" + _access_token + "&action=reportbad&id=" + captchaid;
            var resp = Utils.GET(url, USER_AGENT, TIMEOUT);
            //dynamic d = JObject.Parse(resp);
            //return d.ToString();
            return resp;
        }

        /// <summary>
        /// Set captcha good
        /// </summary>
        /// <param name="captchaid"></param>
        /// <returns></returns>
        public static string set_captcha_good(string _access_token, string captchaid)
        {
            var url = string.Format("{0}/res.php?key={1}&action=reportgood&id={2}", BASE_URL, _access_token, captchaid);
            var resp = Utils.GET(url, USER_AGENT, TIMEOUT);
            //dynamic d = JObject.Parse(resp);
            //return d.ToString();
            return resp;
        }
    }
}
