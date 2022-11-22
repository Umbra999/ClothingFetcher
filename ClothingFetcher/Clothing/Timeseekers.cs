namespace ClothingFetcher.Clothing
{
    internal class Timeseekers
    {
        private static readonly string URL = "https://www.timeseekers.eu/collections/electronic-underground";
        private static readonly string WebhookURL = "https://discord.com/api/webhooks/1000738489429671966/hNmK2QAlJ_A4D4YOV-Eb_q5elmz0gIf-d4mWzPCq9Hk6ifncrWzJGjFf_s0GWoYnYRL9";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "Timeseekers";

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
                if (AllLine[i].Contains("ProductItem__Wrapper") && AllLine[i].Contains("/collections/electronic-underground/products/") && AllLine[i + 10].Contains("€"))
                {
                    string Price = AllLine[i + 10].Split('>', '<')[6];
                    string Name = AllLine[i].Split('=')[11].Split('"')[1];
                    string URL = AllLine[i].Split('=')[2].Split('"')[1];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price {Price}: https://www.timeseekers.eu{URL}");

                    if (!Main.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Main.CreateEmbed(Name, Price, $"https://www.timeseekers.eu{URL}", "Limited Product");

                    await Main.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}
