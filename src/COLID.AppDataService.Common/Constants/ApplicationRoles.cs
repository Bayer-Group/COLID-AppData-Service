using System;
using System.Collections.Generic;
using System.Text;

namespace COLID.AppDataService.Common.Constants
{
    public class ApplicationRoles
    {
        public const string Administration = "COLID.Administration.ReadWrite";

        public class ConsumerGroup
        {
            public const string FullAccess = Administration + "," + "ConsumerGroup.ReadWrite";
        }

        public class Resource
        {
            public const string Notifications = Administration + "," + "Resource.Notifications.All";
        }

        public class Scheduler
        {
            public const string FullAccess = Administration + "," + "SchedulerData.ReadWrite";
        }
    }
}
