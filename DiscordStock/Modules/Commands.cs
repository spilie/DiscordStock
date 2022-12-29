using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DIscordTest.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private static bool isSleep = false;

        /// <summary>
        /// Online
        /// </summary>
        /// <returns></returns>
        [Command("出發")]
        public async Task Online()
        {
            if (!isSleep)
            {
                await ReplyAsync("已經在收割了是要去哪裡");
                return;
            }
            else
            {
                isSleep = false;
                await ReplyAsync("轟轟轟!出發收割拉!!");
                return;
            }
        }

        [Command("回家")]
        public async Task Offline()
        {
            if (isSleep) return;

            await ReplyAsync("轟轟轟!收割完回家啦!!");

            isSleep = true;
        }

        [Command("韭菜幫幫我")]
        public async Task Help()
        {
            if (isSleep) return;

            string rtnMessage = string.Empty;

            rtnMessage += string.Format("今日股票 (代號) : 查詢今日股價\n");
            rtnMessage += string.Format("股票 (日期) (代號): 查詢股價\n");

            await ReplyAsync(rtnMessage);
            return;
        }

        [Command("今日股票")]
        public async Task Stock([Remainder] string code = null)
        {
            if (isSleep) return;

            if (code == null)
            {
                await ReplyAsync("所以我說那個代號呢");
                return;
            }

            string url = string.Format("https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&date={0}&stockNo={1}", DateTime.Now.Date.ToString("yyyyMMdd"), code);

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 30000;

            string result = "";
            // 取得回應資料
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                response.Close();
            }

            await ReplyAsync(result);
            return;
        }

        private static DateTime yesterDate = DateTime.Now.AddDays(-1).Date;

        [Command("股票")]
        public async Task Stock(string date = null, [Remainder] string code = null)
        {
            if (isSleep) return;

            if (code == null)
            {
                await ReplyAsync("所以我說那個代號呢");
                return;
            }

            if (date.Length == 4)
                date = DateTime.Now.Year + date;

            if (date.Length != 8)
            {
                await ReplyAsync("日期都打錯");
                return;
            }

            //string key = this.getKey(DateTime.ParseExact(date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces), code);
            //string value = RedisHelper.Get(key);

            string value = "";

            string result = "";
            if (string.IsNullOrEmpty(value))
            {
                string url = string.Format("https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&date={0}&stockNo={1}", date, code);

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

                request.Method = "GET";
                request.ContentType = "application/json";
                request.Timeout = 30000;

                // 取得回應資料
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }

                    response.Close();
                }

                //RedisHelper.Set(key, result);
            }
            else
            {
                result = value;
            }

            try
            {
                TwseStockModel stock = JsonConvert.DeserializeObject<TwseStockModel>(result);
                List<TwseDataModel> dataList = new List<TwseDataModel>();

                if (stock.stat == "OK")
                {
                    foreach (string[] datas in stock.data)
                    {
                        DateTime d = Convert.ToDateTime(datas[0]).AddYears(1911);

                        dataList.Add(new TwseDataModel(d, Convert.ToDecimal(datas[1]), Convert.ToDecimal(datas[2]), Convert.ToDecimal(datas[3]), Convert.ToDecimal(datas[4]), Convert.ToDecimal(datas[5]), Convert.ToDecimal(datas[6]),
                            Convert.ToDecimal(datas[7]), Convert.ToInt32(datas[8])));
                    }
                }

                if (date.Length == 4)
                {
                    date = DateTime.Now.Year + date;
                }

                TwseDataModel data = dataList.Where(x => x.Date.ToString("yyyyMMdd") == date).FirstOrDefault();

                //result = JsonConvert.SerializeObject(data);

                if (data == null)
                {
                    result = "那天休市";
                }
                else
                {
                    result = @"日期 :" + data.Date.ToShortDateString() + "\n成交股數 : " + data.TransactionPrice + "\n成交金額 : " + data.Turnover + "\n" +
                        "開盤價 : " + data.OpeningPrice + "\n最高價 : " + data.HighestPrice + "\n最低價 : " + data.LowestPrice + "\n" +
                        "收盤價 : " + data.CloseingPrice + "\n漲跌價差 : " + data.PriceDifference + "\n成交筆數 : " + data.TransCount + "";

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            await ReplyAsync(result);
            Thread.Sleep(1500);
            return;
        }

        private string getKey(DateTime date, string code)
        {
            return string.Format("Stock_{0}_{1}:{2}", date.Year, date.Month, code);
        }
    }
}
