using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EB_Utility;

public static class SteamWebAPI
{
    public static class Model
    {
        public class Tag
        {
            [JsonProperty("Tag")]
            public string Name;
        }

        public class WorkshopItemDetails
        {
            public string PublishedFileId;
            public int    Result;
            public string Creator;
            [JsonProperty("creator_app_id")]
            public int    CreatorAppId;
            [JsonProperty("consumer_app_id")]
            public int    ConsumerAppId;
            public string FileName;
            [JsonProperty("file_size")]
            public long   FileSize;
            [JsonProperty("file_url")]
            public string FileURL;
            [JsonProperty("hcontent_file")]
            public string HContentFile;
            [JsonProperty("preview_url")]
            public string PreviewURL;
            [JsonProperty("hcontent_preview")]
            public string HContentPreview;
            public string Title;
            public string Description;
            [JsonProperty("time_created")]
            public int    TimeCreated;
            [JsonProperty("time_updated")]
            public int    TimeUpdated;
            public int    Visibility;
            public int    Banned;
            [JsonProperty("ban_reason")]
            public string BanReason;
            public int    Subscriptions;
            public int    Favorited;
            [JsonProperty("lifetime_subscriptions")]
            public int    LifetimeSubscriptions;
            [JsonProperty("lifetime_favorited")]
            public int    LifetimeFavorited;
            public int    Views;
            public List<Tag> Tags;
        }
    }

    public  static HttpClient      HttpClient = WebRequest.CreateHTTPClient(connectionTimeout:60);
    private static readonly string baseAPIURL = "https://api.steampowered.com/";

    public static async Task<List<Model.WorkshopItemDetails>> GetWorkshopItemDetails(string[] idList)
    {
        string apiModule = "ISteamRemoteStorage/GetPublishedFileDetails/v1/";

        var parameters = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("itemCount", idList.Length.ToString())
        };

        for(int i=0; i < idList.Length; i++)
        {
            string workshopItemId = idList[i];
            parameters.Add(new KeyValuePair<string, string>("publishedfileids["+i.ToString()+"]", workshopItemId));
        }

        string res = await WebRequest.PostAsync(HttpClient, baseAPIURL + apiModule, parameters);
        
        List<Model.WorkshopItemDetails> itemDetailsList = new List<Model.WorkshopItemDetails>();
        JToken jsonData = JToken.Parse(res)["response"];

        if((int)jsonData["result"]      == 1
        && (int)jsonData["resultcount"] == idList.Length)
        {
            JArray workshopItems = (JArray)jsonData["publishedfiledetails"];

            int i=-1;
            foreach(JToken elem in workshopItems)
            {
                Logger.WriteLog($"SteamWebAPI.GetWorkshopItemDetails() - Workshop Item ID: {idList[++i]}");
                Model.WorkshopItemDetails itemDetails = elem.ToObject<Model.WorkshopItemDetails>();
                itemDetailsList.Add(itemDetails);
            }
        }

        return itemDetailsList;
    }
}
