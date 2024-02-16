using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace OntrackDb.Entities
{
    [Table("safetyDisposal")]
    public class SafetyDisposal
    { 
        [Key]
        public int Id { get; set; }

        
        public string Name { get; set; }
        
        public string ADDLCOINFO { get; set; }
        
        public string ADDRESS1 { get; set; }
        
        public string ADDRESS2 { get; set; }
        
        public string CITY { get; set; }
        
        public string STATE { get; set; }
        
        public string ZIP { get; set; }
       
        public string LATITUDE { get; set; }
        
        public string LONGITUDE { get; set; }
        
        [NotMapped]
        public Point Location { get; set; }
        [NotMapped]
        public double Distance { get; set; }
    }
}
