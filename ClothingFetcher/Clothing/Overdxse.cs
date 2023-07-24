namespace ClothingFetcher.Clothing
{
    internal class Overdxse
    {
        private static readonly string URL = "https://overdxseclothing.com/collections/all?page=1&sort_by=created-descending";
        private static readonly string WebhookURL = "";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "Overdxse";

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
                if (AllLine[i].Contains("Regular price"))
                {
                    string Price = AllLine[i + 1].Trim(' ');
                    string Name = AllLine[i - 7].Trim(' ').Split('>', '<')[2];
                    string URL = AllLine[i - 34].Split('"')[1];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price {Price}: https://overdxseclothing.com{URL}");

                    if (!Boot.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Boot.CreateEmbed(Name, Price, $"https://overdxseclothing.com{URL}", Name.Contains("1of1") ? "1of1 Product" : "Limited Product");

                    await Boot.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}
