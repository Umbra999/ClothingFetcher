namespace ClothingFetcher.Clothing
{
    internal class Overdxse
    {
        private static readonly string URL = "https://overdxseclothing.com/collections/all?page=1&sort_by=created-descending";
        private static readonly string WebhookURL = "https://discord.com/api/webhooks/991884799398649997/ns7C8kCQS920qrVRju4lkDcBfyViP12WRfmX27DUa7C4Hx0uof4LW9IaSqEalYRtZcPn";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "Overdxse";

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
                if (AllLine[i].Contains("Regular price"))
                {
                    string Price = AllLine[i + 1].Trim(' ');
                    string Name = AllLine[i - 7].Trim(' ').Split('>', '<')[2];
                    string URL = AllLine[i - 34].Split('"')[1];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price {Price}: https://overdxseclothing.com{URL}");

                    if (!Main.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Main.CreateEmbed(Name, Price, $"https://overdxseclothing.com{URL}", Name.Contains("1of1") ? "1of1 Product" : "Limited Product");

                    await Main.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}
