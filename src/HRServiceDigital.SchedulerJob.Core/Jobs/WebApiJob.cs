using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Core.Jobs
{
    public class WebApiJob : IJob
    {
        public string Host { private get; set; }
        public string Url { private get; set; }
        //public List<KeyValuePair<string, string>> Params { private get; set; }
        public string Params { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            var key = context.JobDetail.Key;

            HttpClient client = new HttpClient();
            if (!Url.StartsWith("/"))
            {
                Url = "/" + Url;
            }

            var paramsStr = (!string.IsNullOrEmpty(Params)) ? "?" + Params : string.Empty;

            string uri = Host + Url + paramsStr;
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                await Console.Out.WriteLineAsync($"job instance: {key}, result: {result}");
            }
            await Console.Out.WriteLineAsync($"{DateTime.Now} - Greetings from HelloJob!");
            
        }

        private static string BuildParam(List<KeyValuePair<string, string>> paramArray, Encoding encode = null)
        {
            string url = "";

            if (encode == null) encode = Encoding.UTF8;

            if (paramArray != null && paramArray.Count > 0)
            {
                var parms = "";
                foreach (var item in paramArray)
                {
                    parms += string.Format("{0}={1}&", Encode(item.Key, encode), Encode(item.Value, encode));
                }
                if (parms != "")
                {
                    parms = parms.TrimEnd('&');
                }
                url += parms;

            }
            return url;
        }

        private static string Encode(string content, Encoding encode = null)
        {
            if (encode == null) return content;

            return System.Web.HttpUtility.UrlEncode(content, Encoding.UTF8);

        }
    }
}
