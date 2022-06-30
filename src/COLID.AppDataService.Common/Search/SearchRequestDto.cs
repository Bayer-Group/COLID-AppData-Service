using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace COLID.AppDataService.Common.Search
{
    public class SearchRequestDto
    {
        public SearchRequestDto()
        {
            AggregationFilters = new Dictionary<string, List<string>>();
            RangeFilters = new Dictionary<string, Dictionary<string, string>>();
            NoAutoCorrect = false;
            EnableHighlighting = false;
            SearchIndex = SearchIndex.Published;
            EnableAggregation = true;
            EnableSuggest = true;
            Order = SearchOrder.Desc;
            OrderField = "_score";
            FieldsToReturn = null;
        }

        public string SearchTerm { get; set; }
        public int From { get; set; }
        public int Size { get; set; }
        public IDictionary<string, List<string>> AggregationFilters { get; set; }
        public IDictionary<string, Dictionary<string, string>> RangeFilters { get; set; }
        public bool NoAutoCorrect { get; set; }
        public bool EnableHighlighting { get; set; }
        public string ApiCallTime { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SearchIndex SearchIndex { get; set; }

        public bool EnableAggregation { get; set; }
        public bool EnableSuggest { get; set; }

        public SearchOrder Order { get; set; }

        public string OrderField { get; set; }

        public ISet<string> FieldsToReturn { get; set; }
    }
}
