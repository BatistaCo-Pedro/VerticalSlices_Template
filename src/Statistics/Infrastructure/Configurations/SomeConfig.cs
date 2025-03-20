using Statistics.Domain.Entities;

namespace Statistics.Infrastructure.Configurations;

public class SomeConfig : IEntityTypeConfiguration<SomeEntity>
{
    public void Configure(EntityTypeBuilder<SomeEntity> builder)
    {
        builder.ToTable("SomeEntity");
    }
}
