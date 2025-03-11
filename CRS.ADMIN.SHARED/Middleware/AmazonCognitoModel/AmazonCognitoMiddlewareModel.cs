﻿using System.Collections.Generic;

namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel
{
    public enum ResponseCode
    {
        Success = 0,
        Failed = 1,
        Warning = 2,
        Exception = 9
    }

    public class CommonResponse
    {
        public ResponseCode Code { get; set; } = ResponseCode.Exception;
        public string Message { get; set; }
        public object Data { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();
    }

    public class Error
    {
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class AttributeTypeName
    {
        public const string Email = "email";
        public const string PhoneNumber = "phone_number";
        public const string Sub = "sub";
    }
}
