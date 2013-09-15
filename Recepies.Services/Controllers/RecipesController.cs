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
        public HttpResponseMessage EditRecipe([FromUri]int id, RecipeModel model,
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
        [ActionName("allbyuser")]
        public IQueryable<RecipeModel> GetAll(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))]
            string accessToken)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();
                var user = this.GetUserByAccessToken(accessToken, context);

                var recipeModels = user.Recepies
                    .AsQueryable()
                    .OrderByDescending(rec => rec.Name)
                    .Select(rec => new RecipeModel() { 
                        Id = rec.RecepieId,
                        Name = rec.Name,
                        CookingSteps = rec.CookingSteps,
                        Products = rec.Products,
                        ImagePath = rec.ImagePath
                    });

                return recipeModels;
            });
        }

        [HttpGet]
        public RecipeModel Get([FromUri]int id)
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();

                var recipe = this.GetRecipeById(id, context);

                var recipeModel = new RecipeModel() {
                    Id = recipe.RecepieId,
                    Name = recipe.Name,
                    CookingSteps = recipe.CookingSteps,
                    Products = recipe.Products,
                    ImagePath = recipe.ImagePath
                };
                return recipeModel;
            });
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<RecipeModel> GetAll()
        {
            return this.ExecuteOperationAndHandleExceptions(() =>
            {
                var context = new RecipeContext();

                var recipeModels = context.Recepies
                    .AsQueryable()
                    .OrderByDescending(rec => rec.Name)
                    .Select(rec => new RecipeModel()
                    {
                        Id = rec.RecepieId,
                        Name = rec.Name,
                        CookingSteps = rec.CookingSteps,
                        Products = rec.Products,
                        ImagePath = rec.ImagePath
                    });

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
        
    }
}
