using MvvmDemo.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MvvmDemo.Data;

// A DbContext connects C# objects to database tables.
public class PeopleContext : DbContext
{
    public DbSet<Person> People => Set<Person>();
    public static string DatabasePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DataGridDemo", "people-mvvm.db");

    public PeopleContext() { }
    public PeopleContext(DbContextOptions<PeopleContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
            optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
        }
    }
}



