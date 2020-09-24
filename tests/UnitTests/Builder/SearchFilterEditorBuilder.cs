using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public class SearchFilterEditorBuilder : EntityBuilderBase<SearchFilterEditorBuilder, SearchFilterEditor, DtoBase, int>
    {
        protected override SearchFilterEditorBuilder SpecificBuilder => this;

        public SearchFilterEditorBuilder()
        {
            _entity = new SearchFilterEditor();
        }

        public SearchFilterEditorBuilder WithFilterJson(JObject jObject)
        {
            _entity.FilterJson = jObject;
            return this;
        }

        public SearchFilterEditorBuilder WithFilterJson(string jsonString)
        {
            _entity.FilterJson = JObject.Parse(jsonString);
            return this;
        }
    }
}
