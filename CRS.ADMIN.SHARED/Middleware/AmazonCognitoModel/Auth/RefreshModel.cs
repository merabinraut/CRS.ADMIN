namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.Auth
{
    public class RefreshModel
    {
        public class Request
        {
            public string Username { get; set; }
            public string RefreshToken { get; set; }
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
