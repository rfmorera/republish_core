using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;

namespace Captcha2Api
{
    public class Captcha2Solver
    {
        private const string BASE_URL = "http://2captcha.com";
        private const string USER_AGENT = "csharpClient1.0";
        private const int TIMEOUT = 30000;

        private string _access_token;
        private int _timeout;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="timeout"></param>
        public Captcha2Solver(string access_token, int timeout = 30000)
        {
            this._access_token = access_token;
            this._timeout = timeout;
        }

        /// <summary>
        /// Get account's balance
        /// </summary>
        /// <returns></returns>
        public string account_balance()
        {
            var url = string.Format("{0}/user/balance?access_token={1}", BASE_URL, this._access_token);
            var resp = Utils.GET(url, USER_AGENT, TIMEOUT);
            dynamic d = JObject.Parse(resp);
            return string.Format("${0}", d.balance.ToString());
        }

        /// <summary>
        /// Submit image captcha
        /// </summary>
        /// <param name="opts"></param>
        /// <returns>captchaID</returns>
        public string submit_image_captcha(Dictionary<string, string> opts)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var url = string.Format("{0}/captcha/image", BASE_URL);
            dict.Add("access_token", this._access_token);
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
        public string submit_recaptcha(string _uri, string siteKey)
        {
            var uri2Captcha = string.Format("{0}/in.php", BASE_URL);
            var postData = "";
            postData += "key=" + _access_token;
            postData += "&method=userrecaptcha";
            postData += "&googlekey=" + siteKey;
            postData += "&pageurl=" + _uri;

            var data = Encoding.ASCII.GetBytes(postData);

            WebRequest request = WebRequest.Create(uri2Captcha);

            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().GetAwaiter().GetResult();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());

            var responseString = streamReader.ReadToEnd();
            streamReader.Close();
            // streamReader.Dispose();

            response.Close();
            //response.Dispose();

            return responseString.Substring(3, responseString.Length - 3);
        }

        /// <summary>
        /// Retrieve captcha text / gresponse using captcha ID
        /// </summary>
        /// <param name="captchaid"></param>
        /// <returns></returns>
        public string retrieve(string captchaid)
        {
            string answerUrl = "http://2captcha.com/res.php?key=" + _access_token + "&action=get&id=" + captchaid;

            HttpClient client = new HttpClient();
            HttpResponseMessage responseClient = client.GetAsync(answerUrl).GetAwaiter().GetResult();
            string responseString = responseClient.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            responseClient.Dispose();

            if (responseString.Substring(0, 2) == "OK")
            {
                return responseString.Substring(3, responseString.Length - 3);
            }
            return null;
        }

        /// <summary>
        /// Set captcha bad
        /// </summary>
        /// <param name="captchaid"></param>
        /// <returns></returns>
        public string set_captcha_bad(string captchaid)
        {
            var url = string.Format("{0}/res.php?key={1}&action=reportbad&id={2}", BASE_URL, this._access_token, captchaid);
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
        public string set_captcha_good(string captchaid)
        {
            var url = string.Format("{0}/res.php?key={1}&action=reportgood&id={2}", BASE_URL, this._access_token, captchaid);
            var resp = Utils.GET(url, USER_AGENT, TIMEOUT);
            dynamic d = JObject.Parse(resp);
            return d.ToString();
        }
    }
}
