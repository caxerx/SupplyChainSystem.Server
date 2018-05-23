using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChainSystem.Server
{
    public class SupplyResponse
    {
        public SupplyResponse(bool success, object response)
        {
            Success = success;
            ResponseContent = response;
        }
        public bool Success { get; set; }
        public object ResponseContent { get; set; }

        public static SupplyResponse Ok(object response)
        {
            return new SupplyResponse(true, response);
        }

        public static SupplyResponse Fail(string message)
        {
            return new SupplyResponse(false, new ErrorMessage(message));
        }

        public static SupplyResponse NotFound()
        {
            return new SupplyResponse(false, new ErrorMessage("Not Found"));
        }

        public static SupplyResponse Exception(Exception e)
        {
            return new SupplyResponse(false, new ExceptionMessage(e));
        }
    }

    public class ErrorMessage
    {
        public ErrorMessage(string msg)
        {
            Message = msg;
        }
        public string Message { get; set; }
    }

    public class ExceptionMessage
    {
        public ExceptionMessage(Exception e)
        {
            Message = e.Message;
            Exception = e.GetType().ToString();
        }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
