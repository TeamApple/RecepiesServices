using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Recepies.Services.AuthenticationHeaders;
using Recepies.Services.Models;
using Recepies.Data;

namespace Recepies.Services.Controllers
{
    public class RecipesController : BaseApiController
    {
        [HttpPost]
        [ActionName("new")]
        public HttpResponseMessage CreateRecipe(RecipeModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();

                var user = this.GetUserByAccessToken(accessToken, context);

                this.ValidateRecipe(model);

                var recipeEntity = new Recepy()
                {
                    Name = model.Name,
                    Products = model.Products,
                    CookingSteps = model.CookingSteps,
                    User = user,
                    ImagePath = model.ImagePath
                };

                context.Recepies.Add(recipeEntity);
                context.SaveChanges();

                var responseModel = new RecipeCreatedModel()
                {
                    Id = recipeEntity.RecepieId
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, responseModel);

                return response;
            });
        }

        [HttpPut]
        [ActionName("edit")]
        public HttpResponseMessage EditRecipe(int id, RecipeModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();

                var user = this.GetUserByAccessToken(accessToken, context);

                this.ValidateRecipe(model);

                var recipeEntity = this.GetRecipeById(id, context);

                recipeEntity.Name = model.Name;
                recipeEntity.Products = model.Products;
                recipeEntity.CookingSteps = model.CookingSteps;
                recipeEntity.ImagePath = model.ImagePath;

                context.SaveChanges();

                var responseModel = new RecipeCreatedModel()
                {
                    Id = recipeEntity.RecepieId
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, responseModel);

                return response;
            });
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<Recepy> GetAll(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();
                var user = this.GetUserByAccessToken(accessToken, context);

                var recipeModels = user.Recepies
                    .AsQueryable()
                    .OrderByDescending(rec => rec.Name);

                return recipeModels;
            });
        }

        private void ValidateRecipe(RecipeModel model)
        {
            if (String.IsNullOrEmpty(model.Name) ||
                String.IsNullOrEmpty(model.CookingSteps) ||
                String.IsNullOrEmpty(model.Products) ||
                String.IsNullOrEmpty(model.ImagePath))
            {
                throw new ArgumentNullException("Invalid Recipe");
            }
        }

        protected Recepy GetRecipeById(int id, RecipeContext context)
        {
            var recipe = context.Recepies.FirstOrDefault(rec => rec.RecepieId == id);
            if (recipe == null)
            {
                throw new InvalidOperationException("Invalid recipe id");
            }
            return recipe;
        }
    }
}
