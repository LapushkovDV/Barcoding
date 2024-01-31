using ERP_API_Service.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ERP_API_Service.Objects
{
    public class CredentialsBody
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Imei { get; set; }
    }

    public class AnswerBody
    {
        public string Result { get; set; }
    }
    public class AsyncAnswerBody
    {
        public Guid Guid { get; set; }
        public RequestStatus Status { get; set; }
        public string Result { get; set; }
    }

    public class AnswerBody<T>
    {
        public string Result { get; set; }
        public T Object { get; set; }
    }
    public class KatMcAnswerBody
    {
        public string Result { get; set; }
        public string Nrec { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
        public string Obozn { get; set; }
    }

    public class TokenBody
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; } 
        public string Message { get; set; }
    }

    public class RequestBody
    {
        public string Token { get; set; }
    }

    public class RequestMcBody
    {
        public string BarCode { get; set; }
    }

    public class RequestDescrShk
    {
        [JsonProperty("actionName")]
        public string actionName { get; set; }
    }

    public class RequestObjById
    {
        [JsonProperty("actionName")]
        public string actionName { get; set; }
        public string ident { get; set; }
    }

    public class RequestObjByIdESB
    {
        [JsonProperty("actionName")]
        public string ActionName { get; set; }
        [JsonProperty("ident")]
        public string Ident { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("imei")]
        public string Imei { get; set; }
    }

    public class UserMenuBody
    {
        public UserMenuBody(int _userId)
        {
            UserId = _userId;
            Menu = new List<MenuBody>();
        }
        public int UserId { get; set; }

        public List<MenuBody> Menu { get; set; }
    }

    public class MenuBody
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
    }
    public class OperationBody
    {
        public string RequestBody { get; set; }
    }
    public class OperationAnswerBody
    {
        [JsonProperty("@odata.context")]
        public string @OdataContext { get; set; }
        public string Value { get; set; }
    }
    public class GetStatusBody
    {
        public Guid Guid { get; set; }
    }
    
}
