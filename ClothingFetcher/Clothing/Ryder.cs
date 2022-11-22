namespace ClothingFetcher.Clothing
{
    internal class Ryder
    {
        private static readonly string URL = "https://www.ryderstudios.us/shop";
        private static readonly string WebhookURL = "https://discord.com/api/webhooks/1018936789022556171/V6iw81vwCl9AsgZqG-Qi34VjKOFDBnIJHK_UEeLs7x8-wTYNeenZDUd2pWo4SGjLwaud";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "Ryder";

        public static async Task Initialize()
        {
            Console.WriteLine($"{Foldername} Initialized");

            if (Main.KeywordsEnabled && File.Exists($"{Foldername}\\AllowedKeywords.txt")) AllowedKeywords = File.ReadAllLines($"{Foldername}\\AllowedKeywords.txt");
            if (Main.BlacklistEnabled && File.Exists($"{Foldername}\\BlacklistedKeywords.txt")) BlacklistedKeywords = File.ReadAllLines($"{Foldername}\\BlacklistedKeywords.txt");

            for (; ; )
            {
                string Response = await Main.CollectionRequest(URL);
                if (Response != null) await HandleResponse(Response);
                await Task.Delay(30000);
            }
        }

        private static async Task HandleResponse(string response)
        {
            string[] AllLine = response.Split('\n');

            for (int i = 0; i < AllLine.Length; i++)
            {
                if (AllLine[i].Contains("product-name"))
                {
                    string Price = AllLine[i].Split(new string[] { "product-price" }, StringSplitOptions.None)[1].Split('<')[0].Trim('"', '>');
                    string Name = AllLine[i].Split(new string[] { "product-name" }, StringSplitOptions.None)[1].Split('<')[0].Trim('"', '>');
                    string URL = AllLine[i].Split(new string[] { "/products/" }, StringSplitOptions.None)[1].Split('"')[0];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price ${Price}: https://www.ryderstudios.us/products/{URL}");

                    if (!Main.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Main.CreateEmbed(Name, $"${Price}", $"https://www.ryderstudios.us/products/{URL}", "Limited Product");

                    await Main.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}
