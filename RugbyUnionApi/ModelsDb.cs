using Microsoft.EntityFrameworkCore;

namespace RugbyUnionApi
{
    public class ModelsDb : DbContext
    {
        public ModelsDb(DbContextOptions<ModelsDb> options)
            :base(options) { }
        
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Team> Teams => Set<Team>();
    }
}
