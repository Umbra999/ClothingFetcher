namespace ClothingFetcher.Clothing
{
    internal class Ryder
    {
        private static readonly string URL = "https://www.ryderstudios.us/shop";
        private static readonly string WebhookURL = "";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "Ryder";

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
                if (AllLine[i].Contains("product-name"))
                {
                    string Price = AllLine[i].Split(new string[] { "product-price" }, StringSplitOptions.None)[1].Split('<')[0].Trim('"', '>');
                    string Name = AllLine[i].Split(new string[] { "product-name" }, StringSplitOptions.None)[1].Split('<')[0].Trim('"', '>');
                    string URL = AllLine[i].Split(new string[] { "/products/" }, StringSplitOptions.None)[1].Split('"')[0];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price ${Price}: https://www.ryderstudios.us/products/{URL}");

                    if (!Boot.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Boot.CreateEmbed(Name, $"${Price}", $"https://www.ryderstudios.us/products/{URL}", "Limited Product");

                    await Boot.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}
