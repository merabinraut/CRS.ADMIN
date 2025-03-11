namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.UserManagement
{
    public class UserDetailModel
    {
        public class Request
        {
            public string Username { get; set; }
        }

        public class Response
        {
            public string Sub { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }
    }
}
