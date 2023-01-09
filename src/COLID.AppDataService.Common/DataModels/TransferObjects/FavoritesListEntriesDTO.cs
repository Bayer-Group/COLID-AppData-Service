using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.DataModels.TransferObjects
{
    public class FavoritesListEntriesDTO : DtoBase
    {
        public int favoritesListId { get; set; }
        public string PIDUri { get; set; }
        public string PersonalNote { get; set; } = null;

    }
}
