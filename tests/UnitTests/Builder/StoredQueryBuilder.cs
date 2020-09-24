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

        public StoredQueryBuilder WithQueryName(string queryName)
        {
            _entity.QueryName = queryName;
            return this;
        }

        public StoredQueryBuilder WithQueryJson(JObject jObject)
        {
            _entity.QueryJson = jObject;
            return this;
        }

        public StoredQueryBuilder WithQueryJson(string jsonString)
        {
            _entity.QueryJson = JObject.Parse(jsonString);
            return this;
        }

        public StoredQueryBuilder WithExecutionInterval(ExecutionInterval execInterval)
        {
            _entity.ExecutionInterval = execInterval;
            return this;
        }

        public StoredQueryBuilder WithLastExecutionResult(JObject jObject)
        {
            _entity.LastExecutionResult = jObject;
            return this;
        }

        public StoredQueryBuilder WithLastExecutionResult(string jsonString)
        {
            _entity.LastExecutionResult = JObject.Parse(jsonString);
            return this;
        }

        public StoredQueryBuilder WithNextExecutionAt(DateTime date)
        {
            _entity.NextExecutionAt = date;
            return this;
        }

        public StoredQueryBuilder WithUser(User user)
        {
            _entity.User = user;
            return this;
        }
    }
}
