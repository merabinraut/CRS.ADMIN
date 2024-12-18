namespace CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.UserManagement
{
    public class SetUserStatusModel
    {
        public class Request
        {
            public string userName { get; set; }
            public bool enable { get; set; } = false;
        }
    }
}
