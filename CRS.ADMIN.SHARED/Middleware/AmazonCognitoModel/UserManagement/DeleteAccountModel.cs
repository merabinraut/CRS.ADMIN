namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.UserManagement
{
    public class DeleteAccountModel
    {
        public class Request
        {
            public string AccessToken { get; set; }
        }
    }
    public class AdminDeleteAccountModel
    {
        public class Request
        {
            public string userName { get; set; }
        }
    }
}
