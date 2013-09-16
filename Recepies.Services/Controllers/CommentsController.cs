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
                var recipe = this.GetRecipeById(model.RecepieId,context);


                this.ValidateComment(model);

                var commentEntity = new Comment
                {
                    Text = model.Text,
                    Recepy = recipe,
                    User = meUser
                };
                context.Comments.Add(commentEntity);                    
                context.SaveChanges();

                var responseModel = new CreatedComment()
                {
                    Id = commentEntity.CommentId,
                    OwnerId = commentEntity.UserId
                };
                var response = this.Request.CreateResponse(HttpStatusCode.Created, responseModel);
                return response;
            });
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<CommentModel> GetAll(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();
                var user = this.GetUserByAccessToken(accessToken, context);

                var commentsList = context.Comments.AsQueryable().OrderBy(cm => cm.Text).Select(cm => new CommentModel()
                {
                    Id = cm.CommentId,
                    Text = cm.Text,
                    RecepieId = cm.RecepieId
                });
                return commentsList;
            });
        }

        [HttpGet]
        [ActionName("allbyrecipe")]
        public IQueryable<CommentModel> GetSingleList(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.GetAll(accessToken)                    
                    .Where(rec => rec.RecepieId == id)                    
                    .Select(rec => new CommentModel()
                    {
                        Id = rec.Id,
                        Text = rec.Text,
                        RecepieId = rec.RecepieId
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
