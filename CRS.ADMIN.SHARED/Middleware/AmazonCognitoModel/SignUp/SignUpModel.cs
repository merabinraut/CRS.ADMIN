using System.Collections.Generic;

namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp
{
    public class SignUpModel
    {
        public class Request
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public List<AttributeType> AttributeType { get; set; } = new List<AttributeType>();
        }

        public class AttributeType
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class Response
        {
            public string UserSub { get; set; }
        }
    }
}
