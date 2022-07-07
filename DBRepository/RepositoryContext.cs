using Microsoft.EntityFrameworkCore;
using Model;
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
        public RepositoryContext() => Database.EnsureCreated();

        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculationResult>().HasKey(vf => new { vf.CalculationId, vf.ImplementationId }

            );
        }
    }
}
