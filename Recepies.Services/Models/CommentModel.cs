using Recepies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Recepies.Services.Models
{
    [DataContract]
    public class CommentModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "recepieId")]
        public int RecepieId { get; set; }
    }
}