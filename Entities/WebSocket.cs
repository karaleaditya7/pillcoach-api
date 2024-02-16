using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("webSocket")]
    public class WebSocket
    {
        [Key]
        public int Id { get; set; }
        [Column("UserId")]
        public User User { get; set; }
        [Column("connectionID")]
        public string ConnectionID { get; set; }
    }

    internal class WebSocketConfiguration : IEntityTypeConfiguration<WebSocket>
    {
        public void Configure(EntityTypeBuilder<WebSocket> builder)
        {
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
