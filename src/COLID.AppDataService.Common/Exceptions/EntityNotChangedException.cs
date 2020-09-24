using System;
using System.Net;
using COLID.Exception.Attributes;
using COLID.Exception.Models;

namespace COLID.AppDataService.Common.Exceptions
{
    [StatusCode(HttpStatusCode.OK)]
    public class EntityNotChangedException : BusinessException
    {
        public EntityNotChangedException()
        {
        }

        public EntityNotChangedException(string message) : base(message)
        {
        }

        public EntityNotChangedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
