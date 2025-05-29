using Core.Concretes.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Kompozit birincil anahtar oluşturur. Iki sutun durumunun ayni anda tekil olmasını sağlar.
            builder.Entity<Follower>()
                   .HasKey(f => new { f.FollowerMemberId, f.FollowedMemberId });

            // Bir tablo baska bir tablonun iki ayri primary key yapisina ayri ayri foreignkey baglantisi asagidaki gibi bir fluent api kullanmadan baglanmaz.
            builder.Entity<Follower>()
                   .HasOne(f => f.FollowerMember)
                   .WithMany(u => u.Following)
                   .HasForeignKey(f => f.FollowerMemberId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Follower>()
                   .HasOne(f => f.FollowedMember)
                   .WithMany(u => u.Followers)
                   .HasForeignKey(f => f.FollowedMemberId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
