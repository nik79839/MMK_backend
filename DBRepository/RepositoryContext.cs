using Microsoft.EntityFrameworkCore;
using Model;
using Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBRepository
{
    public class RepositoryContext : DbContext
    {
        public DbSet<CalculationResult> CalculationResults { get; set; } = null!;
        public DbSet<Calculations> Calculations { get; set; } = null!;
        public DbSet<VoltageResult> VoltageResults { get; set; } = null!;
        public RepositoryContext() => Database.EnsureCreated();

        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculationResult>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId });
            modelBuilder.Entity<Calculations>().HasKey(vf => new { vf.CalculationId });
            modelBuilder.Entity<VoltageResult>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId, vf.NodeNumber });
        }
    }
}
