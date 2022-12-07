using Infrastructure.Persistance.Entities;
using Infrastructure.Persistance.Entities.Result;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class CalculationResultContext : DbContext
    {
        public DbSet<PowerFlowResultEntity> PowerFlowResults { get; set; } = null!;
        public DbSet<CalculationEntity> Calculations { get; set; } = null!;
        public DbSet<VoltageResultEntity> VoltageResults { get; set; } = null!;
        public DbSet<CurrentResultEntity> CurrentResults { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<WorseningSettingsEntity> WorseningSettings { get; set; } = null!;

        //public CalculationResultContext() => Database.EnsureCreated();

        public CalculationResultContext(DbContextOptions<CalculationResultContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PowerFlowResultEntity>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId });
            modelBuilder.Entity<CalculationEntity>().HasKey(vf => new { vf.Id });
            modelBuilder.Entity<VoltageResultEntity>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId, vf.NodeNumber });
            modelBuilder.Entity<CurrentResultEntity>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId, vf.BrunchName });
            modelBuilder.Entity<WorseningSettingsEntity>().HasKey(vf => new { vf.CalculationId, vf.NodeNumber });
        }
    }
}
