using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcasting.Streaming.Domain;

internal class Streamer
{
    private Streamer() { }

    public Streamer(string userId, string channelSlug)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID is required.", nameof(userId));


        if (string.IsNullOrWhiteSpace(channelSlug))
            throw new ArgumentException("Channel Slug is required.", nameof(channelSlug));

        Id = Guid.CreateVersion7();
        UserId = userId;
        ChannelSlug = channelSlug;
    }

    public Guid Id { get; private init; }

    public string UserId { get; private init; } = default!;

    public string ChannelSlug { get; private init; } = default!;

    public string? StreamSecretKey { get; private set; }

    public void GenerateStreamSecretKey()
    {
        StreamSecretKey = $"live_sec_{Guid.CreateVersion7():N}";
    }
}

internal class StreamerConfiguration : IEntityTypeConfiguration<Streamer>
{
    public void Configure(EntityTypeBuilder<Streamer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasIndex(x => x.ChannelSlug)
            .IsUnique();
    }
}
