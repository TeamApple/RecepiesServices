using Recepies.Services.AuthenticationHeaders;
using Recepies.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Recepies.Data;

namespace Recepies.Services.Controllers
{
    public class CommentsController : BaseApiController
    {
        [HttpPost]
        [ActionName("new")]
        public HttpResponseMessage CreateComment(CommentModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();
                var meUser = this.GetUserByAccessToken(accessToken, context);

                this.ValidateComment(model);

                var commentEntity = new Comment
                {
                    Text = model.Text,
                    RecepieId = model.RecepieId,
                    User = meUser
                }
                context.Comments.Add(commentEntity);                    
                context.SaveChanges();

                var responseModel = new CreatedComment()
                {
                    Id = commentEntity.CommentId
                };
                var response = this.Request.CreateResponse(HttpStatusCode.Created, responseModel);
                return response;
            });
        }

        private void ValidateComment(CommentModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Text))
            {
                throw new ArgumentNullException("Invalid comment!");
            }
        }
    }
}
