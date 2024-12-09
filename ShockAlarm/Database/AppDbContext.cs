using ComputerUtils.Logging;
using Microsoft.EntityFrameworkCore;
using ShockAlarm.Alarm;
using ShockAlarm.Users;

namespace ShockAlarm.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> Sessions { get; set; }
    public DbSet<OpenshockApiToken> OpenshockApiTokens { get; set; }
    public DbSet<Shocker> Shockers { get; set; }
    public DbSet<AlarmTone> AlarmTones { get; set; }
    public DbSet<Alarm.Alarm> Alarms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Config.LoadConfig();
        optionsBuilder.UseSqlite(Config.Instance.dbConnectionString == null ? "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "database.db" : Config.Instance.dbConnectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shocker>().HasIndex(x => x.ShockerId);
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<UserSession>().HasKey(x => x.Id);
        modelBuilder.Entity<OpenshockApiToken>().HasKey(x => x.Id);
        modelBuilder.Entity<OpenshockApiToken>().Navigation(x => x.User).AutoInclude();
        modelBuilder.Entity<Alarm.Alarm>().Navigation(x => x.Shockers).AutoInclude();
        modelBuilder.Entity<Alarm.Alarm>().Navigation(x => x.User).AutoInclude();
        modelBuilder.Entity<Alarm.Alarm>().HasMany(x => x.Shockers).WithOne(x => x.Alarm)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<AlarmTone>().Navigation(x => x.Components).AutoInclude();
        modelBuilder.Entity<AlarmTone>().Navigation(x => x.User).AutoInclude();
        modelBuilder.Entity<AlarmTone>().HasMany(x => x.Components).WithOne().OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Shocker>().Navigation(x => x.ApiToken).AutoInclude();
        modelBuilder.Entity<Shocker>().Navigation(x => x.Permissions).AutoInclude();
        modelBuilder.Entity<Shocker>().Navigation(x =>x.Limits).AutoInclude();
        modelBuilder.Entity<Shocker>().Navigation(x => x.Tone).AutoInclude();
        modelBuilder.Entity<Shocker>().HasOne(x => x.Permissions).WithOne().OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Shocker>().HasOne(x => x.Limits).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}