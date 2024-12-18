using System.Collections.Generic;

namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.UserManagement
{
    public class UpdateUserAttributesModel
    {
        public class Request
        {
            public string AccessToken { get; set; }
            public List<AttributeType> AttributeType { get; set; } = new List<AttributeType>();
        }
        public class AttributeType
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }

    public class MarkEmailAsVerifiedModel
    {
        public class Request
        {
            public string Username { get; set; }
            public string Email { get; set; }
        }
    }
}
