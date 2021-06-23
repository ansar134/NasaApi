using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NasaApi.Models
{
    /// <summary>
    /// The response from Nasa API is deserialized into this model class.
    /// A cached version of this is stored in memory DB
    /// </summary>
    public class NasaAstroPicModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Date { get; set; }

        public string Explanation { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string HdUrl { get; set; }

        public string Media_Type { get; set; }






    }
}
