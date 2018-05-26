using System;
using System.Configuration;
using System.Transactions;
using Microsoft.AspNetCore.Authentication;

namespace SupplyChainSystem.Server
{
    public class SupplyResponse
    {
        public bool Success { get; set; }
        public object ResponseContent { get; set; }

        public static SupplyResponse Ok(object response)
        {
            return new SupplyResponse
            {
                Success = true,
                ResponseContent = response
            };
        }

        public static SupplyResponse Ok()
        {
            return new SupplyResponse {Success = true};
        }

        public static SupplyResponse Fail(string type, string message)
        {
            return new SupplyResponse
            {
                Success = false,
                ResponseContent = new ErrorMessage {FailType = type, Message = message}
            };
        }

        public static SupplyResponse NotFound(string itemType, string id)
        {
            return new SupplyResponse
            {
                Success = false,
                ResponseContent = new ErrorMessage
                {
                    FailType = "Not Found",
                    Message = string.Format(Properties.strings.ITEM_NOT_FOUND, itemType, id)
                }
            };
        }

        public static SupplyResponse BadRequest(string msg)
        {
            return new SupplyResponse
            {
                Success = false,
                ResponseContent = new ErrorMessage { FailType = "Bad Request", Message = msg }
            };
        }

        public static SupplyResponse RequiredFieldEmpty()
        {
            return new SupplyResponse
            {
                Success = false,
                ResponseContent = new ErrorMessage { FailType = "Bad Request", Message = Properties.strings.REQUIRED_FIELD_EMPTY }
            };
        }

        public static SupplyResponse DuplicateEntry(string itemType, string id)
        {
            return new SupplyResponse
            {
                Success = false,
                ResponseContent = new ErrorMessage { FailType = "Bad Request", Message = string.Format(Properties.strings.DUPLICATE_ENTRY, itemType, id) }
            };
        }

        public static SupplyResponse Exception(Exception e)
        {
            return new SupplyResponse {Success = false, ResponseContent = new ExceptionMessage(e)};
        }
    }

    public class ErrorMessage
    {
        public string FailType { get; set; }
        public string Message { get; set; }
    }

    public class ExceptionMessage : ErrorMessage
    {
        public ExceptionMessage(Exception e)
        {
            FailType = "Exception";
            Message = e.Message;
            Exception = e.GetType().ToString();
        }

        public string Exception { get; set; }
    }
}