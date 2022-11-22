namespace ClothingFetcher.Clothing
{
    internal class Bape
    {
        private static readonly string URL = "https://int.bape.com";
        private static readonly string WebhookURL = "https://discord.com/api/webhooks/1018642978467745802/T4eGH5HILrZX5npv1ymPa3bajYhNVi0lFoLGLkUbboSe4XHccw9xa2fYjau9mf68JFpE";
        private static string[] AllowedKeywords = new string[0];
        private static string[] BlacklistedKeywords = new string[0];
        private static readonly string Foldername = "Bape";

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
                    string Price = null;
                    int priceindex = 1;
                    while (Price == null)
                    {
                        if (AllLine[i + priceindex].Contains("US$") && !AllLine[i + priceindex].Contains("FREE SHIPPING ON ORDERS OVER")) Price = AllLine[i + priceindex].Trim(' ');
                        else priceindex++;
                    }
                    string Name = AllLine[i + 1].Split('"', ':')[5].Trim(' ');
                    string URL = AllLine[i + 1].Split('"')[1];
                    Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {Name} for Price {Price}: {URL}");

                    if (!Main.IsAllowedItem(AllowedKeywords, BlacklistedKeywords, Name)) continue;

                    object[] Embeds = Main.CreateEmbed(Name, Price, URL, "Limited Product");

                    await Main.SendEmbedWebHook(WebhookURL, Embeds);
                    await Task.Delay(250);
                }
            }
        }
    }
}
