using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Recepies.Services.Models
{
    [DataContract]
    public class RecipeModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "products")]
        public string Products { get; set; }

        [DataMember(Name = "cookingSteps")]
        public string CookingSteps { get; set; }

        [DataMember(Name = "imagePath")]
        public string ImagePath { get; set; }
    }
}