using Microsoft.EntityFrameworkCore;
using BLL;
using BLL.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CalculationResultContext : DbContext
    {
        public DbSet<PowerFlowResult> PowerFlowResults { get; set; } = null!;
        public DbSet<Calculations> Calculations { get; set; } = null!;
        public DbSet<VoltageResult> VoltageResults { get; set; } = null!;
        public CalculationResultContext() => Database.EnsureCreated();

        public CalculationResultContext(DbContextOptions<CalculationResultContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PowerFlowResult>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId });
            modelBuilder.Entity<Calculations>().HasKey(vf => new { vf.CalculationId });
            modelBuilder.Entity<VoltageResult>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId, vf.NodeNumber });
        }
    }
}
