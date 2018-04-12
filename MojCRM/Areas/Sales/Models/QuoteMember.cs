using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class QuoteMember
    {
        [Key]
        public int Id { get; set; }
        public int RelatedQuoteId { get; set; }

        [ForeignKey("RelatedQuoteId")]
        public virtual Quote Quote { get; set; }

        public string MemberName { get; set; }
        public QuoteMemberRoleEnum QuoteMemberRole { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public enum QuoteMemberRoleEnum
        {
            [Description("Jedini predstavnik")]
            SingleMember,

            [Description("Član grupe")]
            MultipartyMember,

            [Description("Odobravatelj")]
            Approver
        }

        public string QuoteMemberRoleString
        {
            get
            {
                switch (QuoteMemberRole)
                {
                    case QuoteMemberRoleEnum.SingleMember: return "Jedini predstavnik";
                    case QuoteMemberRoleEnum.MultipartyMember: return "Član grupe";
                    case QuoteMemberRoleEnum.Approver: return "Odobravatelj";
                }
                return "Uloga predstavnika";
            }
        }
    }
}