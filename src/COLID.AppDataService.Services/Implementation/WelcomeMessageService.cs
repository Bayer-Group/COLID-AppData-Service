using System;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Common.Enums;
using COLID.AppDataService.Common.Utilities;
using COLID.AppDataService.Repositories.Interfaces;
using COLID.AppDataService.Services.Interfaces;
using COLID.Cache.Services;

namespace COLID.AppDataService.Services.Implementation
{
    public class WelcomeMessageService : ServiceBase<WelcomeMessage>, IWelcomeMessageService
    {
        private readonly ICacheService _cache;
        private readonly IUserService _userService;

        public WelcomeMessageService(IGenericRepository repo, ICacheService cache, IUserService userService) : base(repo)
        {
            _cache = cache;
            _userService = userService;
        }

        public WelcomeMessage GetWelcomeMessageEditor()
        {
            return GetWelcomeMessage(ColidType.Editor);
        }

        public WelcomeMessage GetWelcomeMessageDataMarketplace()
        {
            return GetWelcomeMessage(ColidType.DataMarketplace);
        }

        private WelcomeMessage GetWelcomeMessage(ColidType type)
        {
            return _cache.GetOrAdd(type.ToString(), () => GetOne(wm => wm.Type.Equals(type)));
        }

        public WelcomeMessage UpdateWelcomeMessageEditor(string content)
        {
            return UpdateWelcomeMessage(ColidType.Editor, content);
        }

        public WelcomeMessage UpdateWelcomeMessageDataMarketplace(string content)
        {
            return UpdateWelcomeMessage(ColidType.DataMarketplace, content);
        }

        private WelcomeMessage UpdateWelcomeMessage(ColidType type, string content)
        {
            Guard.IsNotEmpty(content);
            var welcomeMessage = GetWelcomeMessage(type);
            welcomeMessage.Content = content;
            Update(welcomeMessage);
            Save();
            
            if (type == ColidType.DataMarketplace)
            {
                var users = _userService.GetAll();
                foreach (var user in users)
                {
                    user.ShowUserInformationFlagDataMarketplace = true;
                    _userService.Update(user);
                }
                _userService.Save();
            }

            _cache.Delete(type.ToString(), () => { });

            return welcomeMessage;
        }
    }
}
