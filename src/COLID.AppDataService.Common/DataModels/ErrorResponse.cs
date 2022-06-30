using System.Net;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Common.DataModels
{
    public class ErrorResponse
    {
        public HttpStatusCode Code { get; set; }

        public string Message { get; set; }

#nullable enable
        public IEntity? Entity { get; set; }
#nullable disable

        public string ApplicationId { get; set; }
    }
}
