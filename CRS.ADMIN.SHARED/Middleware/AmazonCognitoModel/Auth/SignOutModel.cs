namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Auth
{
    public class SignOutModel
    {
        public class Request
        {
            public string AccessToken { get; set; }
        }
    }

    public class AdminSignOutModel
    {
        public class Request
        {
            public string Username { get; set; }
        }
    }
}
