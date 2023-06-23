using System.Collections.Generic;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.DataModels.TransferObjects;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.AppDataService.Services.Interfaces;

namespace COLID.AppDataService.Services.Implementation
{
    public class ApplicationVersionService : IApplicationVersionService
    {
        private readonly IApplicationVersionRepository _repo;

        public ApplicationVersionService(IApplicationVersionRepository repo)
        {
            _repo = repo;
        }

        public ApplicationVersion GetCOLIDVersion()
        {
            return _repo.GetLastVersion("COLID");
        }

        public ApplicationVersionDto GetLastVersion(string application)
        {
            ApplicationVersionDto result = new ApplicationVersionDto();
            result.ApplicationVersion = new List<ApplicationVersion>() { _repo.GetLastVersion(application) };
            result.ColidVersion = GetCOLIDVersion();
            return result;
        }

        public ApplicationVersionDto GetLastVersions(string application, int historyLength = 3)
        {
            ApplicationVersionDto result = new ApplicationVersionDto();
            result.ApplicationVersion = _repo.GetLastVersions(application, historyLength);
            result.ColidVersion = GetCOLIDVersion();
            return result;
        }
    }
}
