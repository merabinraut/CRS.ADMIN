namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Password
{
    public class ForgotPasswordModel
    {
        public class Request
        {
            public string Username { get; set; } 
        }
    }

    public class ResendForgotPasswordConfirmationCodeModel
    {
        public class Request
        {
            public string Username { get; set; } 
        }
    }

    public class ConfirmForgotPasswordModel
    {
        public class Request
        {
            public string Username { get; set; } 
            public string ConfirmationCode { get; set; } 
            public string NewPassword { get; set; } 
        }
    }
}
