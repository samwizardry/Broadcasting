using Microsoft.EntityFrameworkCore;

namespace Broadcasting.Identity.Data;

internal class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }
}
