using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MojCRM.Models;

namespace MojCRM.Areas.Campaigns.Models
{
    public class CampaignMember
    {
        [Key]
        public int Id { get; set; }

        public int CampaignId { get; set; }

        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }

        public string MemberName { get; set; }
        public MemberRoleEnum MemberRole { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public enum MemberRoleEnum
        {
            [Description("Nositelj kampanje")]
            Head,

            [Description("Nadzornik kampanje")]
            Supervisor,

            [Description("Član")]
            Member
        }

        public string MemberRoleString
        {
            get
            {
                switch (MemberRole)
                {
                    case MemberRoleEnum.Head: return "Nositelj kampanje";
                    case MemberRoleEnum.Supervisor: return "Nadzornik kampanje";
                    case MemberRoleEnum.Member: return "Član kampanje";
                }
                return "Rola";
            }
        }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public IQueryable<CampaignMember> GetCamapigns(string agent)
        {
            var model = _db.CampaignMembers.Where(cm => cm.MemberName == agent && cm.Campaign.CampaignStatus == Campaign.CampaignStatusEnum.InProgress);
            return model;
        }
    }
}