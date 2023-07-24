namespace ClothingFetcher.Clothing
{
    internal class MXDVS
    {
        private static readonly string URL = "https://store.mxdvs.co/collections/products";
        private static readonly string WebhookURL = "";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "MXDVS";

        public static async Task Initialize()
        {
            Console.WriteLine($"{Foldername} Initialized");

            if (Boot.KeywordsEnabled && File.Exists($"{Foldername}\\AllowedKeywords.txt")) AllowedKeywords = File.ReadAllLines($"{Foldername}\\AllowedKeywords.txt");
            if (Boot.BlacklistEnabled && File.Exists($"{Foldername}\\BlacklistedKeywords.txt")) BlacklistedKeywords = File.ReadAllLines($"{Foldername}\\BlacklistedKeywords.txt");

            for (; ; )
            {
                string Response = await Boot.CollectionRequest(URL);
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

                    if (!Boot.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Boot.CreateEmbed(Name, Price, $"https://store.mxdvs.co{URL}", "Limited Product");

                    await Boot.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}