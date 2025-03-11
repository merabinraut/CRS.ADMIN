namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Auth
{
    public class SignInModel
    {
        public class Request
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public string TokenType { get; set; }
            public string IdToken { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public int ExpiresIn { get; set; }
        }
    }
}
