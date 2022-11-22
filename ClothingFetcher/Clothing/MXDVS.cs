namespace ClothingFetcher.Clothing
{
    internal class MXDVS
    {
        private static readonly string URL = "https://store.mxdvs.co/collections/products";
        private static readonly string WebhookURL = "https://discord.com/api/webhooks/1026484755199430677/1LxjlX5of8eiCors83KCRc0vsf2PLnHzK8xad5BGVQf9cAwEDIw7y0sF1q3sHMJLfoM2";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "MXDVS";

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
                if (AllLine[i].Contains("/collections/products/products/"))
                {
                    string Price = AllLine[i + 16].Trim(' ');
                    string Name = AllLine[i + 10].Split('"', '<', '>')[4];
                    string URL = AllLine[i].Split('>', '<', '"')[2];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price {Price}: https://store.mxdvs.co{URL}");

                    if (!Main.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Main.CreateEmbed(Name, Price, $"https://store.mxdvs.co{URL}", "Limited Product");

                    await Main.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}