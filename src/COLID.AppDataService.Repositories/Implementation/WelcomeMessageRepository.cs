using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Exceptions;
using COLID.AppDataService.Common.Extensions;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;
using COLID.Exception.Models.Business;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class WelcomeMessageRepository : GenericRepository<WelcomeMessage, int>, IWelcomeMessageRepository
    {
        public WelcomeMessageRepository(AppDataContext context) : base(context)
        {
        }

        public WelcomeMessage GetOne(ColidType type)
        {
            var welcomeMessage = FindByCondition(m => m.Type.Equals(type)).FirstOrDefault();
            if (welcomeMessage.IsEmpty())
            {
                throw new EntityNotFoundException($"Unable to find a welcome message for {type}", type.ToString());
            }

            return welcomeMessage;
        }
    }
}
