using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Sayt.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Sayt;

public class MainContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public MainContext(DbContextOptions<MainContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
