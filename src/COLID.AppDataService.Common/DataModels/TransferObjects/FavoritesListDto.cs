using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class FavoritesListDto : DtoBase
    {
        public string Name { get; set; }
        public string PersonalNote { get; set; }
        public string PIDUri { get; set; }
    }
}
