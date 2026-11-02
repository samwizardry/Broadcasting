using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Broadcasting.Streaming.Domain;

internal class StreamChannel
{
    private StreamChannel() { }

    public StreamChannel(string username, string channelSlug)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("User ID is required.", nameof(username));


        if (string.IsNullOrWhiteSpace(channelSlug) || channelSlug.Length < 1)
            throw new ArgumentException("Channel Slug is required.", nameof(channelSlug));

        Id = Guid.CreateVersion7();
        Username = username;
        ChannelSlug = channelSlug;
    }

    public Guid Id { get; private init; }

    public string Username { get; private init; } = default!;

    public string ChannelSlug { get; private init; } = default!;

    public string? StreamSecretKey { get; private set; }

    public bool IsLive { get; private set; }

    public void GenerateStreamSecretKey()
    {
        StreamSecretKey = $"live_sec_{Guid.CreateVersion7():N}";
    }

    public void SetLiveStatus(bool isLive)
    {
        IsLive = isLive;
    }
}

internal class StreamerConfiguration : IEntityTypeConfiguration<StreamChannel>
{
    public void Configure(EntityTypeBuilder<StreamChannel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasIndex(x => x.ChannelSlug)
            .IsUnique();
    }
}
