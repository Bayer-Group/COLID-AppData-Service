using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class SearchFilterDataMarketplaceBuilder : EntityBuilderBase<SearchFilterDataMarketplaceBuilder, SearchFilterDataMarketplace, SearchFilterDataMarketplaceDto, int>
    {
        protected override SearchFilterDataMarketplaceBuilder SpecificBuilder => this;

        public SearchFilterDataMarketplaceBuilder()
        {
            _entity = new SearchFilterDataMarketplace();
        }

        public SearchFilterDataMarketplaceBuilder WithName(string name)
        {
            _entity.Name = name;
            return this;
        }

        public SearchFilterDataMarketplaceBuilder WithFilterJson(JObject jObject)
        {
            _entity.FilterJson = jObject;
            return this;
        }

        public SearchFilterDataMarketplaceBuilder WithFilterJson(string jsonString)
        {
            _entity.FilterJson = JObject.Parse(jsonString);
            return this;
        }

        public SearchFilterDataMarketplaceBuilder WithUser(User user)
        {
            _entity.User = user;
            return this;
        }
    }
}
