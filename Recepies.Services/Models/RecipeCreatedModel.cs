using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Recepies.Services.Models
{
    [DataContract]
    public class RecipeCreatedModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
    }
}