using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SharedCognitoModel = CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel;

namespace CRS.ADMIN.APPLICATION.Services
{
    public class CognitoService
    {
        public string _configName { get; private set; }
        private AmazonCognitoIdentityProviderClient _provider;
        private CognitoUserPool _userPool;
        private string _clientId;
        private string _clientSecret;
        private string _userPoolId;
        private string _jwkUrl;
        private string _issuer;
        public void SetConfigName(string configName)
        {
            _configName = configName;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "App_Data", "AmazonConfigruation.json");
            var jsonData = JObject.Parse(File.ReadAllText(jsonPath));

            _clientId = jsonData[_configName]["Cognito"]["ClientId"].ToString();
            _clientSecret = jsonData[_configName]["Cognito"]["ClientSecret"].ToString();
            _userPoolId = jsonData[_configName]["Cognito"]["UserPoolId"].ToString();
            _jwkUrl = jsonData[_configName]["Cognito"]["JwkUrl"].ToString();
            _issuer = jsonData[_configName]["Cognito"]["Issuer"].ToString();

            var accessKey = jsonData[_configName]["Credentials"]["AccessKey"].ToString();
            var secretKey = jsonData[_configName]["Credentials"]["SecretKey"].ToString();
            var region = jsonData[_configName]["Cognito"]["Region"].ToString();

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var awsRegion = RegionEndpoint.GetBySystemName(region);
            _provider = new AmazonCognitoIdentityProviderClient(credentials, awsRegion);
            _userPool = new CognitoUserPool(_userPoolId, _clientId, _provider);
        }

        private string GenerateSecretHash(string userName) =>
            !string.IsNullOrEmpty(_clientSecret) ? GenerateSecretHash(userName, _clientId, _clientSecret) : null;

        #region Sign Up
        public async Task<SharedCognitoModel.SignUp.SignUpModel.Response> SignUp(SharedCognitoModel.SignUp.SignUpModel.Request request)
        {
            var attributes = request.AttributeType.MapObjects<AttributeType>();

            if (!attributes.Any())
                return new SharedCognitoModel.SignUp.SignUpModel.Response();

            foreach (var attribute in request.AttributeType)
            {
                if (attribute.Name == null ||
                    (!attribute.Name.Equals(AttributeTypeName.Email, StringComparison.OrdinalIgnoreCase) &&
                     !attribute.Name.Equals(AttributeTypeName.PhoneNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    return new SharedCognitoModel.SignUp.SignUpModel.Response();
                }
            }

            var signUpRequest = new SignUpRequest
            {
                ClientId = _clientId,
                Username = request.Username,
                Password = request.Password,
                SecretHash = GenerateSecretHash(request.Username),
                UserAttributes = attributes
            };

            var signUpResponse = await _provider.SignUpAsync(signUpRequest);
            return signUpResponse.MapObject<SharedCognitoModel.SignUp.SignUpModel.Response>();
        }

        public async Task<bool> ResendSignUpConfirmationCode(string userName)
        {
            var resendRequest = new ResendConfirmationCodeRequest
            {
                ClientId = _clientId,
                Username = userName,
                SecretHash = GenerateSecretHash(userName)
            };

            await _provider.ResendConfirmationCodeAsync(resendRequest);
            return true;
        }

        public async Task<bool> ConfirmSignUp(SharedCognitoModel.SignUp.ConfirmSignUpModel.Request request)
        {
            var confirmSignUpRequest = new ConfirmSignUpRequest
            {
                ClientId = _clientId,
                Username = request.Username,
                ConfirmationCode = request.ConfirmationCode,
                SecretHash = GenerateSecretHash(request.Username)
            };

            await _provider.ConfirmSignUpAsync(confirmSignUpRequest);
            return true;
        }

        public async Task<List<SharedCognitoModel.SignUp.SignUpModel.AdminCreateUserResponse>> AdminCreateUser(SharedCognitoModel.SignUp.SignUpModel.Request request)
        {
            var attributes = request.AttributeType.MapObjects<AttributeType>();

            if (!attributes.Any())
                return new List<SharedCognitoModel.SignUp.SignUpModel.AdminCreateUserResponse>();

            foreach (var attribute in request.AttributeType)
            {
                if (attribute.Name == null ||
                    (!attribute.Name.Equals(AttributeTypeName.Email, StringComparison.OrdinalIgnoreCase) &&
                     !attribute.Name.Equals(AttributeTypeName.PhoneNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    return new List<SharedCognitoModel.SignUp.SignUpModel.AdminCreateUserResponse>();
                }
            }

            var adminCreateUserRequest = new AdminCreateUserRequest
            {
                UserPoolId = _userPoolId,
                Username = request.Username,
                UserAttributes = attributes,
                MessageAction = "SUPPRESS",
                TemporaryPassword = request.Password
            };

            var adminCreateUserResponse = await _provider.AdminCreateUserAsync(adminCreateUserRequest);

            await _provider.AdminSetUserPasswordAsync(new AdminSetUserPasswordRequest
            {
                UserPoolId = _userPoolId,
                Username = request.Username,
                Password = request.Password,
                Permanent = true
            });

            return adminCreateUserResponse.User.Attributes.MapObjects<SharedCognitoModel.SignUp.SignUpModel.AdminCreateUserResponse>();
        }
        #endregion

        #region Auth
        public async Task<SharedCognitoModel.Auth.SignInModel.Response> SignIn(SharedCognitoModel.Auth.SignInModel.Request request)
        {
            var user = new CognitoUser(request.Username, _clientId, _userPool, _provider, _clientSecret);
            var authRequest = new InitiateSrpAuthRequest
            {
                Password = request.Password
            };

            var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            return new SharedCognitoModel.Auth.SignInModel.Response
            {
                TokenType = authResponse?.AuthenticationResult?.TokenType,
                IdToken = authResponse?.AuthenticationResult?.IdToken,
                AccessToken = authResponse?.AuthenticationResult?.AccessToken,
                RefreshToken = authResponse?.AuthenticationResult?.RefreshToken,
                ExpiresIn = authResponse.AuthenticationResult.ExpiresIn
            };
        }

        public async Task<SharedCognitoModel.Auth.RefreshModel.Response> Refresh(SharedCognitoModel.Auth.RefreshModel.Request request)
        {
            var refreshRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                {
                    {"REFRESH_TOKEN", request.RefreshToken},
                    {"SECRET_HASH", GenerateSecretHash(request.Username)}
                }
            };

            var refreshResponse = await _provider.InitiateAuthAsync(refreshRequest);
            return new SharedCognitoModel.Auth.RefreshModel.Response
            {
                TokenType = refreshResponse?.AuthenticationResult?.TokenType,
                IdToken = refreshResponse?.AuthenticationResult?.IdToken,
                AccessToken = refreshResponse?.AuthenticationResult?.AccessToken,
                RefreshToken = refreshResponse?.AuthenticationResult?.RefreshToken,
                ExpiresIn = refreshResponse.AuthenticationResult.ExpiresIn
            };
        }

        public async Task<bool> IsValidateToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonWebToken = tokenHandler.ReadJwtToken(accessToken);

            var kid = jsonWebToken.Header.Kid;

            var publicKeys = await GetCognitoPublicKeysAsync(_jwkUrl);

            var signingKey = GetSigningKey(publicKeys, kid);

            if (signingKey == null)
                return false;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false,
                ValidAudience = _clientId,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.Zero
            };

            var claimsPrincipal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);

            var response = await _provider.GetUserAsync(new GetUserRequest { AccessToken = accessToken });
            return true;
        }

        public async Task<bool> SignOut(SharedCognitoModel.Auth.SignOutModel.Request request)
        {
            var globalSignOutRequest = new GlobalSignOutRequest
            {
                AccessToken = request.AccessToken
            };

            await _provider.GlobalSignOutAsync(globalSignOutRequest);
            return true;
        }
        #endregion

        #region Password
        public async Task<bool> ChangePassword(SharedCognitoModel.Password.ChangePasswordModel.Request request)
        {
            var changePasswordRequest = new ChangePasswordRequest
            {
                AccessToken = request.AccessToken,
                PreviousPassword = request.OldPassword,
                ProposedPassword = request.NewPassword
            };

            await _provider.ChangePasswordAsync(changePasswordRequest);
            return true;
        }

        public async Task<bool> ForgotPassword(SharedCognitoModel.Password.ForgotPasswordModel.Request request)
        {
            var forgotPasswordRequest = new ForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = request.Username,
                SecretHash = GenerateSecretHash(request.Username)
            };

            await _provider.ForgotPasswordAsync(forgotPasswordRequest);
            return true;
        }

        public async Task<bool> ResendForgotPasswordConfirmationCode(SharedCognitoModel.Password.ResendForgotPasswordConfirmationCodeModel.Request request)
        {
            var resendRequest = new ResendConfirmationCodeRequest
            {
                ClientId = _clientId,
                Username = request.Username,
                SecretHash = GenerateSecretHash(request.Username)
            };

            await _provider.ResendConfirmationCodeAsync(resendRequest);
            return true;
        }

        public async Task<bool> ConfirmForgotPassword(SharedCognitoModel.Password.ConfirmForgotPasswordModel.Request request)
        {
            var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = request.Username,
                ConfirmationCode = request.ConfirmationCode,
                Password = request.NewPassword,
                SecretHash = GenerateSecretHash(request.Username)
            };

            await _provider.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);
            return true;
        }

        public async Task<bool> SetPassword(SharedCognitoModel.Password.SetPasswordModel.Request request)
        {
            var adminSetUserPasswordRequest = new AdminSetUserPasswordRequest
            {
                Username = request.Username,
                Password = request.Password,
                UserPoolId = _userPoolId,
                Permanent = request.IsPermanent
            };

            await _provider.AdminSetUserPasswordAsync(adminSetUserPasswordRequest);
            return true;
        }
        #endregion

        #region User management
        public async Task<bool> SetUserStatus(SharedCognitoModel.UserManagement.SetUserStatusModel.Request request)
        {
            if (request.enable)
            {
                var enableRequest = new AdminEnableUserRequest
                {
                    Username = request.userName,
                    UserPoolId = _userPoolId
                };
                await _provider.AdminEnableUserAsync(enableRequest);
            }
            else
            {
                var disableRequest = new AdminDisableUserRequest
                {
                    Username = request.userName,
                    UserPoolId = _userPoolId
                };
                await _provider.AdminDisableUserAsync(disableRequest);
            }
            return true;
        }

        public async Task<bool> DeleteAccount(SharedCognitoModel.UserManagement.DeleteAccountModel.Request request)
        {
            var deleteRequest = new DeleteUserRequest
            {
                AccessToken = request.AccessToken
            };

            await _provider.DeleteUserAsync(deleteRequest);
            return true;
        }

        public async Task<bool> AdminDeleteAccount(SharedCognitoModel.UserManagement.AdminDeleteAccountModel.Request request)
        {
            var deleteRequest = new AdminDeleteUserRequest
            {
                UserPoolId = _userPoolId,
                Username = request.userName
            };
            await _provider.AdminDeleteUserAsync(deleteRequest);
            return true;
        }

        public async Task<bool> UpdateUserAttributes(SharedCognitoModel.UserManagement.UpdateUserAttributesModel.Request request)
        {
            var attributes = request.AttributeType.MapObjects<AttributeType>();

            if (!attributes.Any())
                return false;

            foreach (var attribute in request.AttributeType)
            {
                if (attribute.Name == null ||
                    (!attribute.Name.Equals(AttributeTypeName.Email, StringComparison.OrdinalIgnoreCase) &&
                     !attribute.Name.Equals(AttributeTypeName.PhoneNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }

            var updateRequest = new UpdateUserAttributesRequest
            {
                AccessToken = request.AccessToken,
                UserAttributes = attributes
            };

            await _provider.UpdateUserAttributesAsync(updateRequest);
            return true;
        }

        public async Task<bool> MarkEmailAsVerified(SharedCognitoModel.UserManagement.MarkEmailAsVerifiedModel.Request request)
        {
            var adminUpdateUserAttributesRequest = new AdminUpdateUserAttributesRequest
            {
                UserPoolId = _userPoolId,
                Username = request.Username,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType {Name = "email", Value = request.Email},
                    new AttributeType { Name = "email_verified", Value = "true"}
                }
            };

            await _provider.AdminUpdateUserAttributesAsync(adminUpdateUserAttributesRequest);
            return true;
        }
        public async Task<SharedCognitoModel.UserManagement.UserDetailModel.Response> GetUserDetail(SharedCognitoModel.UserManagement.UserDetailModel.Request request)
        {
            var adminGetUserRequest = new AdminGetUserRequest
            {
                Username = request.Username,
                UserPoolId = _userPoolId
            };

            var userResponse = await _provider.AdminGetUserAsync(adminGetUserRequest);

            return new SharedCognitoModel.UserManagement.UserDetailModel.Response
            {
                Sub = userResponse?.UserAttributes?.FirstOrDefault(attr => attr?.Name == "sub")?.Value,
                Email = userResponse?.UserAttributes?.FirstOrDefault(attr => attr?.Name == "email")?.Value,
                PhoneNumber = userResponse?.UserAttributes?.FirstOrDefault(attr => attr?.Name == "phone_number")?.Value
            };
        }
        #endregion
        private static string GenerateSecretHash(string userName, string clientId, string clientSecret)
        {
            var data = userName + clientId;

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hashBytes);
            }
        }
        private static async Task<JsonArray> GetCognitoPublicKeysAsync(string jwkURL)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(jwkURL);
                var responseObject = JsonObject.Parse(response);
                return (JsonArray)responseObject["keys"];
            }
        }
        private static SecurityKey GetSigningKey(JsonArray keys, string kid)
        {
            var key = keys.FirstOrDefault(k => k["kid"].ToString() == kid);

            if (key == null)
                return null;

            var rsa = new RSAParameters
            {
                Modulus = Base64UrlDecode(key["n"].ToString()),
                Exponent = Base64UrlDecode(key["e"].ToString())
            };

            return new RsaSecurityKey(rsa);
        }
        private static byte[] Base64UrlDecode(string request)
        {
            var base64 = request
                .Replace('-', '+')
                .Replace('_', '/')
                .PadRight(request.Length + (4 - request.Length % 4) % 4, '=');

            return Convert.FromBase64String(base64);
        }
    }
}