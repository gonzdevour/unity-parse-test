using Newtonsoft.Json;

namespace FancyScrollView.Leaderboard
{
    class ChatUserData
    {
        [JsonProperty("userId")]
        public string UserID { get; }

        [JsonProperty("userName")]
        public string UserName { get; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; }

        [JsonConstructor]
        public ChatUserData(string userId = "UNKNOWN", string userName = "UNKNOWN", string imageUrl = "")
        {
            UserID = userId;
            UserName = userName;
            ImageUrl = imageUrl;
        }
    }
}
