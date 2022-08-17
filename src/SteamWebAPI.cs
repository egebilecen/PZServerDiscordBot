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
            public int    CreatorAppId;
            public int    ConsumerAppId;
            public string FileName;
            public int    FileSize;
            public string FileURL;
            public string HContentFile;
            public string PreviewURL;
            public string HContentPreview;
            public string Title;
            public string Description;
            public int    TimeCreated;
            public int    TimeUpdated;
            public int    Visibility;
            public int    Banned;
            public string BanReason;
            public int    Subscriptions;
            public int    Favorited;
            public int    LifetimeSubscriptions;
            public int    LifetimeFavorited;
            public List<Tag> Tags;
        }
    }

    private static HttpClient httpClient = WebRequest.CreateHTTPClient();
    private static string     baseAPIURL = "https://api.steampowered.com/";

    public static async Task GetWorkshopItemDetails(string[] idList)
    {
        string apiModule = "ISteamRemoteStorage/GetPublishedFileDetails/v1/";

        var parameters = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("itemCount", idList.Length.ToString())
        };

        for(int i=0; i < idList.Length; i++)
        {
            var id = idList[i];
            parameters.Add(new KeyValuePair<string, string>("publishedfileids["+i.ToString()+"]", id.ToString()));
        }

        string res = await WebRequest.PostAsync(httpClient, baseAPIURL + apiModule, parameters);

        JToken jsonData = JToken.Parse(res)["response"];

        if((int)jsonData["result"]      == 1
        && (int)jsonData["resultcount"] == idList.Length)
        {
            JArray workshopItems = (JArray)jsonData["publishedfiledetails"];

            foreach(JToken elem in workshopItems)
            {
                Model.WorkshopItemDetails itemDetails = elem.ToObject<Model.WorkshopItemDetails>();
            }
        }
    }
}
