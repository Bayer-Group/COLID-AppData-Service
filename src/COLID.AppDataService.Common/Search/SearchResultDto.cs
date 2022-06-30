using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.Search
{
    public class SearchResultDTO  
    {
        public string OriginalSearchTerm { get; set; }

        public string SuggestedSearchTerm { get; set; }

        public HitDTO Hits { get; set; }

        public dynamic Suggest { get; set; }

        public SearchResultDTO(string OriginalSearchTerm, string SuggestedSearchTerm, HitDTO Hits)
        {
            this.OriginalSearchTerm = OriginalSearchTerm;
            this.SuggestedSearchTerm = SuggestedSearchTerm;
            this.Hits = Hits;
        }
    }

    public class HitDTO
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("max_score")]
        public double MaxScore { get; set; }

        [JsonProperty("hits")]
        public List<SearchHit> Hits { get; set; }
    }
    public class SearchHit
    {
        public Dictionary<string, JObject> Source { get; set; }

        public SearchHit()
        {
            this.Source = new Dictionary<string, JObject>();
        }
    }
    //Internal Mapping of hit source
    public class DocumentMapDirection
    {
        public List<Document> outbound { get; set; }
        public List<Document> inbound { get; set; }
    }

    public class Document
    {
        public string uri { get; set; }
        public string value { get; set; }
        public string edge { get; set; }
    } 
}
