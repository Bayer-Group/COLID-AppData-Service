using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;

namespace COLID.AppDataService.Services.Interfaces
{
    public interface IApplicationVersionService
    {
        ApplicationVersionDto GetLastVersions(string application, int historyLength = 3);
        ApplicationVersionDto GetLastVersion(string application);
        ApplicationVersion GetCOLIDVersion();
    }
}
