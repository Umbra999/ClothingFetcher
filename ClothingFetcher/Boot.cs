using ClothingFetcher.Clothing;
using Newtonsoft.Json;
using System.Text;

namespace ClothingFetcher
{
    internal class Boot
    {
        public static bool KeywordsEnabled = false;
        public static bool BlacklistEnabled = false;
        public static bool PingsEnabled = false;

        public static void Main()
        {
            Console.Title = "CLOTHING FETCHER";

            Console.WriteLine("Keywords Enabled:");
            Console.WriteLine("1 > YES");
            Console.WriteLine("2 > NO");
            int KeywordInput = Convert.ToInt32(Console.ReadLine());
            KeywordsEnabled = KeywordInput == 1;
            Console.WriteLine("");

            Console.WriteLine("Blacklist Enabled:");
            Console.WriteLine("1 > YES");
            Console.WriteLine("2 > NO");
            int BlacklistInput = Convert.ToInt32(Console.ReadLine());
            BlacklistEnabled = BlacklistInput == 1;
            Console.WriteLine("");

            Console.WriteLine("Pings Enabled:");
            Console.WriteLine("1 > YES");
            Console.WriteLine("2 > NO");
            int PingInput = Convert.ToInt32(Console.ReadLine());
            PingsEnabled = PingInput == 1;
            Console.WriteLine("");

            Task.Run(() => Timeseekers.Initialize());
            Task.Run(() => Overdxse.Initialize());
            Task.Run(() => Bape.Initialize());
            Task.Run(() => Ryder.Initialize());
            Task.Run(() => MXDVS.Initialize());

            Thread.Sleep(-1);
        }

        public static async Task SendEmbedWebHook(string URL, object[] MSG)
        {
            var req = new
            {
                content = PingsEnabled ? $"Pings: <@155552545782235137>" : $"Pings: DISABLED",
                embeds = MSG
            };

            HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
            HttpRequestMessage Payload = new(HttpMethod.Post, URL)
            {
                Content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json")
            };
            Payload.Headers.Add("User-Agent", RandomString(16));
            await CurrentClient.SendAsync(Payload);
        }

        private static readonly Random random = new(Environment.TickCount);

        public static string RandomString(int length)
        {
            char[] array = "abcdefghlijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToArray();
            string text = string.Empty;
            for (int i = 0; i < length; i++)
            {
                text += array[random.Next(array.Length)].ToString();
            }
            return text;
        }

        public static async Task<string> CollectionRequest(string URL)
        {
            try
            {
                HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false })
                {
                    Timeout = TimeSpan.FromSeconds(30),
                };
                HttpRequestMessage Payload = new(HttpMethod.Get, URL);
                Payload.Headers.Add("User-Agent", RandomString(16));

                HttpResponseMessage Response = await CurrentClient.SendAsync(Payload);
                if (Response.IsSuccessStatusCode) return await Response.Content.ReadAsStringAsync();
            }
            catch { }
            
            return null;
        }

        public static object[] CreateEmbed(string Name, string Price, string URL, string Title, int Color = 11342935)
        {
            var ProductInfos = new
            {
                name = Name,
                value = $"Price: **{Price}**"
            };

            var WebsiteInfos = new
            {
                name = "Link",
                value = $"**{URL}**"
            };

            object[] Fields = new object[]
            {
                ProductInfos,
                WebsiteInfos
            };

            var Embed = new
            {
                title = Title,
                color = Color,
                fields = Fields
            };

            return new object[]
            {
                Embed
            };
        }

        public static bool IsAllowedItem(string[] Whitelist, string[] Blacklist, string Item)
        {
            if (KeywordsEnabled)
            {
                string[] FoundKeywords = Whitelist.Where(x => Item.Contains(x))?.ToArray();
                if (FoundKeywords == null || FoundKeywords.Length == 0) return false;
            }

            if (BlacklistEnabled)
            {
                string[] FoundKeywords = Blacklist.Where(x => Item.Contains(x))?.ToArray();
                if (FoundKeywords != null && FoundKeywords.Length > 0) return false;
            }

            return true;
        }
    }
}
