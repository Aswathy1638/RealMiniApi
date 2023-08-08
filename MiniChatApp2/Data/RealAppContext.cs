using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Model;

namespace MiniChatApp2.Data
{
    public class RealAppContext : IdentityDbContext
    {
        public RealAppContext (DbContextOptions<RealAppContext> options)
            : base(options)
        {
        }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<Logs>().ToTable("Logs");

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            modelBuilder.Entity<Message>()
               .HasOne(m => m.sender)
               .WithMany()
               .HasForeignKey(m => m.senderId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.receiver)
                .WithMany()
                .HasForeignKey(m => m.receiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Message> Message { get; set; }
        public DbSet<Logs>Logs { get; set; }
    }
}
