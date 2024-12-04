namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Password
{
    public class ChangePasswordModel
    {
        public class Request
        {
            public string AccessToken { get; set; } 
            public string OldPassword { get; set; } 
            public string NewPassword { get; set; } 
        }
    }
}
