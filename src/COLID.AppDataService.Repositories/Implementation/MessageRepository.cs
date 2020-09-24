using System;
using System.Collections.Generic;
using System.Linq;
using COLID.AppDataService.Common.DataModel;
using COLID.AppDataService.Repositories.Context;
using COLID.AppDataService.Repositories.Interface;

namespace COLID.AppDataService.Repositories.Implementation
{
    public class MessageRepository : GenericRepository<Message, int>, IMessageRepository
    {
        public MessageRepository(AppDataContext context) : base(context)
        {
            AddInclude(m => m.User);
        }

        public IList<Message> GetUnreadMessagesToSend()
        {
            // TODO: what about timezone here?
            var messagesToSend = GetAll()
                .Where(m =>
                {
                    return !m.ReadOn.HasValue &&
                            m.SendOn.HasValue &&
                            m.SendOn < DateTime.UtcNow;
                })
                .OrderBy(m => m.User?.Id)
                .ToList();

            return messagesToSend;
        }
    }
}
