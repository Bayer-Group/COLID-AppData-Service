using COLID.AppDataService.Common.DataModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace COLID.AppDataService.Repositories.Interfaces
{
    public interface IApplicationVersionRepository
    {

        IList<ApplicationVersion> GetLastVersions(string application, int historyLength = 3);
        ApplicationVersion GetLastVersion(string application);
    }
}
