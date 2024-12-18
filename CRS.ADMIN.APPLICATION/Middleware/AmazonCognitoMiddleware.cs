using Amazon.CognitoIdentityProvider.Model;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Services;
using CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel;
using CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel.SignUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedCognitoModel = CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel;

namespace CRS.ADMIN.APPLICATION.Middleware
{
    public class AmazonCognitoMiddleware
    {
        private readonly CognitoService _cognitoService;

        public AmazonCognitoMiddleware(CognitoService cognitoService)
        {
            _cognitoService = cognitoService;
        }

        public void SetConfigNameViaUserType(string userType)
        {
            var configMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "club", "CognitoClub" },
                { "customer", "CognitoCustomer" },
                { "affiliate", "CognitoAffiliate" }
            };
            var configName = configMapping.TryGetValue(userType, out var result) ? result : string.Empty;

            if (!string.IsNullOrEmpty(configName))
                _cognitoService.SetConfigName(configName);
            else
                throw new ArgumentException($"Invalid user type: {userType}");
        }

        #region Sign Up
        public async Task<SharedCognitoModel.CommonResponse> SignUpAsync(SharedCognitoModel.SignUp.SignUpModel.Request request)
        {
            var signUpRequest = new SharedCognitoModel.SignUp.SignUpModel.Request
            {
                Username = request.Username,
                Password = request.Password,
                AttributeType = request.AttributeType.MapObjects<SharedCognitoModel.SignUp.SignUpModel.AttributeType>()
            };
            try
            {
                var signUpResponse = await _cognitoService.SignUp(signUpRequest);
                if (signUpResponse?.UserSub == null)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString(),
                    Data = signUpResponse.MapObject<SignUpModel.Response>()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> AdminCreateUserAsync(SharedCognitoModel.SignUp.SignUpModel.Request request)
        {
            var signUpRequest = new SharedCognitoModel.SignUp.SignUpModel.Request
            {
                Username = request.Username,
                Password = request.Password,
                AttributeType = request.AttributeType.MapObjects<SharedCognitoModel.SignUp.SignUpModel.AttributeType>()
            };
            try
            {
                var signUpResponse = await _cognitoService.AdminCreateUser(signUpRequest);
                if (signUpResponse.Count() <= 0 || string.IsNullOrEmpty(signUpResponse.FirstOrDefault(x => x.Name == SharedCognitoModel.AttributeTypeName.Sub)?.Value))
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString(),
                    Data = signUpResponse.MapObjects<SignUpModel.AdminCreateUserResponse>()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> ResendSignUpConfirmationCodeAsync(SharedCognitoModel.SignUp.ResendSignUpConfirmationCodeModel.Request request)
        {
            var resendSignUpConfirmationCodeRequest = request.MapObject<SharedCognitoModel.SignUp.ResendSignUpConfirmationCodeModel.Request>();
            try
            {
                var resendSignUpConfirmationCodeRequestResponse = await _cognitoService.ResendSignUpConfirmationCode(resendSignUpConfirmationCodeRequest.Username);
                if (!resendSignUpConfirmationCodeRequestResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> ConfirmSignUpAsync(SharedCognitoModel.SignUp.ConfirmSignUpModel.Request request)
        {
            var confirmSignUpRequest = request.MapObject<SharedCognitoModel.SignUp.ConfirmSignUpModel.Request>();
            try
            {
                var confirmSignUpResponse = await _cognitoService.ConfirmSignUp(confirmSignUpRequest);
                if (!confirmSignUpResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }
        #endregion

        #region Auth
        public async Task<SharedCognitoModel.CommonResponse> SignInAsync(SharedCognitoModel.Auth.SignInModel.Request request)
        {
            var signInRequest = request.MapObject<SharedCognitoModel.Auth.SignInModel.Request>();
            try
            {
                var signInResponse = await _cognitoService.SignIn(signInRequest);
                if (signInResponse == null)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString(),
                    Data = signInResponse.MapObject<SharedCognitoModel.Auth.SignInModel.Response>()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> RefreshAsync(SharedCognitoModel.Auth.RefreshModel.Request request)
        {
            var refreshRequest = request.MapObject<SharedCognitoModel.Auth.RefreshModel.Request>();
            try
            {
                var refreshResponse = await _cognitoService.Refresh(refreshRequest);
                if (refreshResponse == null)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString(),
                    Data = refreshResponse.MapObject<SharedCognitoModel.Auth.RefreshModel.Response>()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> SignOutAsync(SharedCognitoModel.Auth.SignOutModel.Request request)
        {
            var signOutRequest = request.MapObject<SharedCognitoModel.Auth.SignOutModel.Request>();
            try
            {
                var signOutResponse = await _cognitoService.SignOut(signOutRequest);
                if (!signOutResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<bool> IsValidTokenAsync(string accessToken)
        {
            return await _cognitoService.IsValidateToken(accessToken);
        }
        #endregion

        #region Password
        public async Task<SharedCognitoModel.CommonResponse> ChangePasswordAsync(SharedCognitoModel.Password.ChangePasswordModel.Request request)
        {
            var changePasswordRequest = request.MapObject<SharedCognitoModel.Password.ChangePasswordModel.Request>();
            try
            {
                var changePasswordResponse = await _cognitoService.ChangePassword(changePasswordRequest);
                if (!changePasswordResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> ForgotPasswordAsync(SharedCognitoModel.Password.ForgotPasswordModel.Request request)
        {
            var forgotPasswordRequest = request.MapObject<SharedCognitoModel.Password.ForgotPasswordModel.Request>();
            try
            {
                var forgotPasswordResponse = await _cognitoService.ForgotPassword(forgotPasswordRequest);
                if (!forgotPasswordResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> ResendForgotPasswordConfirmationCodeAsync(SharedCognitoModel.Password.ResendForgotPasswordConfirmationCodeModel.Request request)
        {
            var resendForgotPasswordConfirmationCodeRequest = request.MapObject<SharedCognitoModel.Password.ResendForgotPasswordConfirmationCodeModel.Request>();
            try
            {
                var resendForgotPasswordConfirmationCodeResponse = await _cognitoService.ResendForgotPasswordConfirmationCode(resendForgotPasswordConfirmationCodeRequest);
                if (!resendForgotPasswordConfirmationCodeResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> ConfirmForgotPasswordAsync(SharedCognitoModel.Password.ConfirmForgotPasswordModel.Request request)
        {
            var confirmForgotPasswordRequest = request.MapObject<SharedCognitoModel.Password.ConfirmForgotPasswordModel.Request>();
            try
            {
                var confirmForgotPasswordResponse = await _cognitoService.ConfirmForgotPassword(confirmForgotPasswordRequest);
                if (!confirmForgotPasswordResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> SetPasswordAsync(SharedCognitoModel.Password.SetPasswordModel.Request request)
        {
            var setPasswordRequest = request.MapObject<SharedCognitoModel.Password.SetPasswordModel.Request>();
            try
            {
                var setPasswordResponse = await _cognitoService.SetPassword(setPasswordRequest);
                if (!setPasswordResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }
        #endregion

        #region User management
        public async Task<SharedCognitoModel.CommonResponse> SetUserStatusAsync(SharedCognitoModel.UserManagement.SetUserStatusModel.Request request)
        {
            var setUserStatusRequest = request.MapObject<SharedCognitoModel.UserManagement.SetUserStatusModel.Request>();
            try
            {
                var setUserStatusResponse = await _cognitoService.SetUserStatus(setUserStatusRequest);
                if (!setUserStatusResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }
        public async Task<SharedCognitoModel.CommonResponse> DeleteAccountAsync(SharedCognitoModel.UserManagement.DeleteAccountModel.Request request)
        {
            var deleteAccountRequest = request.MapObject<SharedCognitoModel.UserManagement.DeleteAccountModel.Request>();
            try
            {
                var deleteAccountResponse = await _cognitoService.DeleteAccount(deleteAccountRequest);
                if (!deleteAccountResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> AdminDeleteAccountAsync(SharedCognitoModel.UserManagement.AdminDeleteAccountModel.Request request)
        {
            var adminDeleteAccountRequest = request.MapObject<SharedCognitoModel.UserManagement.AdminDeleteAccountModel.Request>();
            try
            {
                var adminDeleteAccountResponse = await _cognitoService.AdminDeleteAccount(adminDeleteAccountRequest);
                if (!adminDeleteAccountResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> UpdateUserAttributesAsync(SharedCognitoModel.UserManagement.UpdateUserAttributesModel.Request request)
        {
            var updateUserAttributesRequest = request.MapObject<SharedCognitoModel.UserManagement.UpdateUserAttributesModel.Request>();
            try
            {
                var updateUserAttributesResponse = await _cognitoService.UpdateUserAttributes(updateUserAttributesRequest);
                if (!updateUserAttributesResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> MarkEmailAsVerifiedAsync(SharedCognitoModel.UserManagement.MarkEmailAsVerifiedModel.Request request)
        {
            var markEmailAsVerifiedRequest = request.MapObject<SharedCognitoModel.UserManagement.MarkEmailAsVerifiedModel.Request>();
            try
            {
                var markEmailAsVerifiedResponse = await _cognitoService.MarkEmailAsVerified(markEmailAsVerifiedRequest);
                if (!markEmailAsVerifiedResponse)
                    return new SharedCognitoModel.CommonResponse
                    {
                        Code = SharedCognitoModel.ResponseCode.Warning,
                        Message = "Something went wrong. Please try again later."
                    };
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }

        public async Task<SharedCognitoModel.CommonResponse> GetUserDetailAsync(SharedCognitoModel.UserManagement.UserDetailModel.Request request)
        {
            var getUserDetailRequest = request.MapObject<SharedCognitoModel.UserManagement.UserDetailModel.Request>();
            try
            {
                var getUserDetailResponse = await _cognitoService.GetUserDetail(getUserDetailRequest);
                return new SharedCognitoModel.CommonResponse
                {
                    Code = ResponseCode.Success,
                    Message = ResponseCode.Success.ToString(),
                    Data = getUserDetailResponse.MapObject<SharedCognitoModel.UserManagement.UserDetailModel.Response>()
                };
            }
            catch (System.Exception ex)
            {
                return new SharedCognitoModel.CommonResponse
                {
                    Code = SharedCognitoModel.ResponseCode.Exception,
                    Message = $"Something went wrong. Please try again later. Message: {ex.Message}"
                };
            }
        }
        #endregion
    }
}
