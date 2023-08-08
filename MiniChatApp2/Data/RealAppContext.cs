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
    public class RealAppContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public RealAppContext (DbContextOptions<RealAppContext> options)
            : base(options)
        {
        }

      /*  public DbSet<User> User { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Logs>Logs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Message>().ToTable("Message");
            base.OnModelCreating(modelBuilder);

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
        }*/
    }
}
