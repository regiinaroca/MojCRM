using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.Sales.Models
{
    public class EducationNote
    {
        [Key]
        public int Id { get; set; }

        public int RelatedEducationEntityId { get; set; }
        [ForeignKey("RelatedEducationEntityId")]
        public virtual Education Education { get; set; }

        public string User { get; set; }
        public string Contact { get; set; }
        public string Note { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}