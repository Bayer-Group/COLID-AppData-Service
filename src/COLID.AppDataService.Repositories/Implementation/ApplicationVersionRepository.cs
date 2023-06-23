
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interfaces;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class ApplicationVersionRepository : IApplicationVersionRepository
    {
        private readonly AppDataContext _context;

        public ApplicationVersionRepository(AppDataContext context)
        {
            _context = context;
        }

        public ApplicationVersion GetLastVersion(string application)
        {
            return _context
                .ApplicationVersions.Where(v => v.Application == application)
                .OrderByDescending(v => v.ReleaseDate)
                .First();
        }

        public IList<ApplicationVersion> GetLastVersions(string application, int historyLength = 3)
        {
            return _context.ApplicationVersions
                .Where(v => v.Application == application)
                .OrderByDescending(v => v.ReleaseDate)
                .Take(historyLength)
                .ToList();
        }
    }
}
