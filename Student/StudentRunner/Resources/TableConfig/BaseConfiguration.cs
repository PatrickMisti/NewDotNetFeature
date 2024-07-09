using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StudentRunner.Model;

namespace StudentRunner.Resources.TableConfig;

public class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.HasQueryFilter(entity => !entity.Deleted);
    }

    public virtual void Configuration(EntityTypeBuilder<T> builder)
    {
        Configure(builder);
    }
}