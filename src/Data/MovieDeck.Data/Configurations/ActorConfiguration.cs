namespace MovieDeck.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MovieDeck.Data.Models;

    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> actor)
        {
            actor
                .Property(e => e.Gender)
                .HasConversion<string>();
        }
    }
}
