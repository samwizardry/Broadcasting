using Broadcasting.Streaming.Domain;

using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Streaming.Data;

internal class StreamingDbContext : DbContext
{
    public StreamingDbContext(DbContextOptions<StreamingDbContext> options)
        : base(options)
    {
    }

    public DbSet<StreamChannel> StreamChannels => Set<StreamChannel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("streaming");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StreamingDbContext).Assembly);
    }
}
