﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Recepies.Data;

namespace Recepies.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        protected static Random rand = new Random();

        protected T ExecuteOperationAndHandleExceptions<T>(Func<T> operation)
        {
            try
            {
                return operation();
            }
            catch (InvalidOperationException ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex.Message);
                throw new HttpResponseException(errResponse);
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }

        protected User GetUserByAccessToken(string accessToken, RecipeContext context)
        {
            var user = context.Users.FirstOrDefault(usr => usr.AccessToken == accessToken);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user credentials");
            }
            return user;
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
