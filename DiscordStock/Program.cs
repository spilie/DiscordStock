using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DiscordStock
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = "";

            _client.Log += _client_Log;
            _client.MessageReceived += client_MessageReceived;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(6000);

            await sayHello();

            await Task.Delay(-1);
        }

        private async Task sayHello()
        {
            ulong id = 798888816634757170; // 3
            var chnl = _client.GetChannel(id) as IMessageChannel; // 4
            await chnl.SendMessageAsync("轟轟轟!\n收割機上線啦!!"); // 5
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        static async private Task client_MessageReceived(SocketMessage msg)
        {
            string message = msg.Content;

            if (msg.Author.IsBot)
                return;

            if (msg.Channel is IDMChannel dmChannel)
            {
                await msg.Channel.SendMessageAsync("收割當然是要到韭菜田裡啊!!");
                return;
            }


            if (msg.Author.Id == 315328907110907904)
            {
                await msg.Channel.SendMessageAsync("目前不受理你的委託");
                return;
            }

            if (message.Length != 4)
                return;

            int stockno = 0;

            if (!int.TryParse(message, out stockno))
                return;

            string data = getStock(stockno);

            await msg.Channel.SendMessageAsync(data);

            await Task.Delay(5000);
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private static string getStock(int code)
        {
            string result = string.Empty;

            string url = string.Format("https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&date={0}&stockNo={1}", DateTime.Now.Date.ToString("yyyyMMdd"), code);

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

            return result;

            //return JsonConvert.DeserializeObject<TwseStockModel>(result);
        }
    }
}
