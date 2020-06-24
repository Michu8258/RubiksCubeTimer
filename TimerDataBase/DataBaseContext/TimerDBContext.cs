using Microsoft.EntityFrameworkCore;
using TimerDataBase.TableModels;

namespace TimerDataBase.DataBaseContext
{
    public class TimerDBContext : DbContext
    {
        public TimerDBContext(DbContextOptions<TimerDBContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryOption> CategoryOptions { get; set; }
        public DbSet<CategoryOptionsSet> CategoryoptionsSets { get; set; }
        public DbSet<Cube> Cubes { get; set; }
        public DbSet<CubeRating> CubeRatings { get; set; }
        public DbSet<CubesCollection> CubesCollections { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<PlasticColor> PlasticColors { get; set; }
        public DbSet<Serie> Series { get; set; }
        public DbSet<Solve> Solves { get; set; }
        public DbSet<Scramble> Scrambles { get; set; }
        public DbSet<ScrambleMove> ScrambleMoves { get; set; }
    }
}
