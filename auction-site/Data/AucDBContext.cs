using Microsoft.EntityFrameworkCore;

public class AucDBContext : DbContext
{
    public AucDBContext(DbContextOptions<AucDBContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<Item> Items { get; set; }
    public DbSet<Admin> Admins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=AuctionDatabase.sqlite");
    }
}