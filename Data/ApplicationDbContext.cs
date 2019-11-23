using Republish.Models;
using Republish.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace Republish.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cuenta> Cuenta { get; set; }
        public DbSet<Grupo> Grupo { get; set; }
        public DbSet<Notificacion> Notificacion { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.HasDefaultSchema("dbo");

            builder.Entity<Company>(entity =>
            {
                entity.HasKey(b => b.Id);
            });

            builder.Entity<Anuncio>(entity =>
            {
                entity.HasKey("Id");

                entity.HasOne(a => a.Grupo)
                      .WithMany(g => g.Anuncios)
                      .HasForeignKey(a => a.GroupId)
                      .HasConstraintName("ForeignKey_Grupo_Anuncio")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Actualizado).HasDefaultValue(false);
                entity.Property(p => p.Caducado).HasDefaultValue(false);
            });

            builder.Entity<Grupo>(entity => 
            {
                entity.HasKey("Id");

                entity.Property(p => p.Activo).HasDefaultValue(true);
            });

            builder.Entity<Temporizador>(entity => 
            {
                entity.HasOne(a => a.Grupo)
                      .WithMany(g => g.Temporizadores)
                      .HasForeignKey(a => a.GrupoId)
                      .HasConstraintName("ForeignKey_Grupo_Temporizador")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CaptchaKeys>(entity =>
            {
                entity.HasKey("Id");
            });

            builder.Entity<Registro>(entity =>
            {
                entity.HasKey("Id");
            });

            builder.Entity<Recarga>(entity =>
            {
                entity.HasKey("Id");
            });

            builder.Entity<Cuenta>(entity =>
            {
                entity.HasKey("Id");

                entity.HasIndex(c => c.UserId);
            });

            builder.Entity<ClienteOpciones>(entity =>
            {
                entity.HasKey("Id");
            });

            builder.Entity<ShortQueue>(entity =>
            {
                entity.HasKey("Id");
            });

            builder.Entity<Notificacion>(entity =>
            {
                entity.HasKey("Id");
            });
        }
    }
}
