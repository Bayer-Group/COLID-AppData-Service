using System;
using System.Net;
using COLID.AppDataService.Common.DataModel;
using COLID.Exception.Attributes;
using COLID.Exception.Models;

namespace COLID.AppDataService.Common.Exceptions
{
    [StatusCode(HttpStatusCode.BadRequest)]
    public class EntityAlreadyExistsException : BusinessException
    {
        public EntityAlreadyExistsException()
        {
        }

        public EntityAlreadyExistsException(string message, IEntity entity) : base(message)
        {
            base.Data["entity"] = entity;
        }

        public EntityAlreadyExistsException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
