namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp
{
    public class ConfirmSignUpModel
    {
        public class Request
        {
            public string Username { get; set; }
            public string ConfirmationCode { get; set; }
        }
    }
}
