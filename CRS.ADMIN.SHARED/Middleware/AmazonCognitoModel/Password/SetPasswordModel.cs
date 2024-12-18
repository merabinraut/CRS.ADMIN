namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Password
{
    public class SetPasswordModel
    {
        public class Request
        {
            public string Username { get; set; } 
            public string Password { get; set; } 
            public bool IsPermanent { get; set; } = false;
        }
    }
}
