using Microsoft.EntityFrameworkCore;
using SADVO.Domain.Entities;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;
using SADVO.Domain.Entities.Elector;
using SADVO.Shared.Utils;

namespace SADVO.Infrastructure.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Ciudadano> Ciudadanos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<PartidoPolitico> PartidosPoliticos { get; set; }
        public DbSet<AsignacionDirigente> AsignacionesDirigente { get; set; }
        public DbSet<PuestoElectivo> PuestosElectivos { get; set; }
        public DbSet<Eleccion> Elecciones { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<CandidatoPuesto> CandidatosPuesto { get; set; }
        public DbSet<Voto> Votos { get; set; }
        public DbSet<AlianzaPolitica> AlianzasPoliticas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //NOTA PARA EL PROFESOR: Antes de migrar la base de datos, cambie los datos del administrador 
            //por los de ustedd

            string passwordHash = PasswordHasher.Hash("1234");

            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id = 1,
                Nombre = "Priscila",
                Apellido = "Perez",
                Email = "priscilaperezherrera@gmail.com",
                Username = "Priscila",
                PasswordHash = passwordHash,
                Activo = true,
                RolId = 1
            });

            modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = 1, Nombre = "Administrador" },
            new Rol { Id = 2, Nombre = "Dirigente" }
            );

            modelBuilder.Entity<Ciudadano>()
                .HasIndex(c => c.Cedula)
                .IsUnique();

            modelBuilder.Entity<PartidoPolitico>()
                .HasIndex(p => p.Siglas)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<AlianzaPolitica>()
                .HasOne(a => a.PartidoSolicitante)
                .WithMany()
                .HasForeignKey(a => a.PartidoSolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlianzaPolitica>()
                .HasOne(a => a.PartidoReceptor)
                .WithMany()
                .HasForeignKey(a => a.PartidoReceptorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlianzaPolitica>()
                .Property(a => a.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<CandidatoPuesto>()
                .HasOne(cp => cp.Candidato)
                .WithMany(c => c.Puestos)
                .HasForeignKey(cp => cp.CandidatoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidatoPuesto>()
                .HasOne(cp => cp.PuestoElectivo)
                .WithMany(p => p.Candidatos)
                .HasForeignKey(cp => cp.PuestoElectivoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidatoPuesto>()
                .HasOne(cp => cp.PartidoPolitico)
                .WithMany()
                .HasForeignKey(cp => cp.PartidoPoliticoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidatoPuesto>()
                .HasOne(cp => cp.Eleccion)
                .WithMany(e => e.Candidatos)
                .HasForeignKey(cp => cp.EleccionId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }

}