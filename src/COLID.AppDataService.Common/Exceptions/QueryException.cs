using System;
using System.Collections.Generic;
using System.Text;
using COLID.Exception.Models;

namespace COLID.AppDataService.Common.Exceptions
{
    public class QueryException : TechnicalException
    {
        public QueryException()
        {
        }

        public QueryException(string message) : base(message)
        {
        }

        public QueryException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
