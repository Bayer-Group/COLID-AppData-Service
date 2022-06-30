using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Common.Enums;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class StoredQueryBuilder : EntityBuilderBase<StoredQueryBuilder, StoredQuery, StoredQueryDto, int>
    {
        protected override StoredQueryBuilder SpecificBuilder => this;

        public StoredQueryBuilder()
        {
            _entity = new StoredQuery();
        }

        public StoredQueryBuilder WithExecutionInterval(ExecutionInterval execInterval)
        {
            _entity.ExecutionInterval = execInterval;
            return this;
        }

        public StoredQueryBuilder WithSearchResultHash(string hash)
        {
            _entity.SearchResultHash = hash;
            return this;
        }

        public StoredQueryBuilder WithlatestExecutionDate(DateTime date)
        {
            _entity.LatestExecutionDate = date;
            return this;
        }
        public StoredQueryBuilder WithSearchNumberResult(int numResults)
        {
            _entity.NumberSearchResults = numResults;
            return this;
        }

        public StoredQueryBuilder WithSearchFilterDataMarketplace(SearchFilterDataMarketplace SearchFilterDataMarketplace)
        {
            _entity.SearchFilterDataMarketplace = SearchFilterDataMarketplace;
            return this;
        }
    }
}
