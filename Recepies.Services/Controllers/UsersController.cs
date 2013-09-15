using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Recepies.Data;
using Recepies.Services.AuthenticationHeaders;
using Recepies.Services.Models;

namespace Recepies.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private const int TokenLength = 50;
        private const string TokenChars = "qwertyuiopasdfghjklmnbvcxzQWERTYUIOPLKJHGFDSAZXCVBNM";
        private const int MinUsernameLength = 6;
        private const int MaxUsernameLength = 30;
        private const int AuthenticationCodeLength = 40;
        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.@";

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserModel model)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                this.ValidateUser(model);

                var context = new RecipeContext();
                var dbUser = GetUserByUsername(model, context);
                if (dbUser != null)
                {
                    throw new InvalidOperationException("This user already exists in the database");
                }
                dbUser = new User()
                {
                    UserName = model.Username,
                    AuthenticationCode = model.AuthCode
                };
                context.Users.Add(dbUser);

                context.SaveChanges();

                var responseModel = new RegisterUserResponseModel()
                {
                    Id = dbUser.UserId,
                    Username = dbUser.UserName,
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, responseModel);
                return response;
            });
        }

        [HttpPost]
        [ActionName("token")]
        public HttpResponseMessage LoginUser(UserModel model)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                //this.ValidateUser(model);
                if (model == null)
                {
                    throw new FormatException("invalid username and/or password");
                }
                this.ValidateAuthCode(model.AuthCode);
                this.ValidateUsername(model.Username);

                var context = new RecipeContext();
                var username = (model.Username).ToLower();
                var user = context.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    throw new InvalidOperationException("Invalid username or password");
                }
                if (user.AccessToken == null)
                {
                    user.AccessToken = this.GenerateAccessToken(user.UserId);
                    context.SaveChanges();
                }
                var responseModel = new LoginResponseModel()
                {
                    Id = user.UserId,
                    Username = user.UserName,
                    AccessToken = user.AccessToken
                };
                var response = this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                return response;
            });
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage LogoutUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();
                var user = this.GetUserByAccessToken(accessToken, context);
                user.AccessToken = null;
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.NoContent);
                return response;
            });
        }

        private User GetUserByUsername(UserModel model, RecipeContext context)
        {
            var usernameToLower = model.Username.ToLower();
            var dbUser = context.Users.FirstOrDefault(u => u.UserName == usernameToLower);
            return dbUser;
        }

        private string GenerateAccessToken(int userId)
        {
            StringBuilder tokenBuilder = new StringBuilder(TokenLength);
            tokenBuilder.Append(userId);
            while (tokenBuilder.Length < TokenLength)
            {
                var index = rand.Next(TokenChars.Length);
                var ch = TokenChars[index];
                tokenBuilder.Append(ch);
            }
            return tokenBuilder.ToString();
        }

        private void ValidateUser(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new FormatException("Username and/or password are invalid");
            }
            this.ValidateUsername(userModel.Username);
            this.ValidateAuthCode(userModel.AuthCode);
        }

        private void ValidateAuthCode(string authCode)
        {
            if (string.IsNullOrEmpty(authCode) || authCode.Length != AuthenticationCodeLength)
            {
                throw new FormatException("Password is invalid");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username.Length < MinUsernameLength || MaxUsernameLength < username.Length)
            {
                throw new FormatException(
                    string.Format("Username must be between {0} and {1} characters",
                        MinUsernameLength,
                        MaxUsernameLength));
            }
            if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new FormatException("Username contains invalid characters");
            }
        }
    }
}
