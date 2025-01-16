﻿using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.ApiResponseMessage
{
    public class ApiResponseMessageCommon:PaginationResponseCommon
    {
        public string MessageId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string MessageEng { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string MessageType { get; set; }
        public string HttpStatusCode { get; set; }
        public string ActionUser { get; set; }
    }
}