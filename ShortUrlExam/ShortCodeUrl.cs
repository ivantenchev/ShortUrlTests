using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShortUrl.ApiTests
{
    public class ShortCodeUrl
    {
        [JsonPropertyName("url")]
        public string url;

        [JsonPropertyName("shortCode")]
        public string shortCode;

        [JsonPropertyName("dateCreated")]
        public string dateCreated;

        [JsonPropertyName("visits")]
        public long visits;
    }
}