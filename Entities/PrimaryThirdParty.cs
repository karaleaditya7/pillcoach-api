using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities
{
    [Table("primaryThirdParty")]
    public class PrimaryThirdParty
    {
        [Key]
        public int Id { get; set; }
        [Column("organizationMarketingName")]
        public string OrganizationMarketingName { get; set; }
        [Column("bin")]
        public string Bin { get; set; }
    }
}
