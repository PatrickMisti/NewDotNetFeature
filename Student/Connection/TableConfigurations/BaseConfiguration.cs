using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Connection.Models;

namespace Connection.TableConfigurations;

public class BaseConfiguration<T>: IEntityTypeConfiguration<T> where T: BaseEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(entity=> entity.Id);
        builder.HasQueryFilter(entity => !entity.Deleted);
    }

    public virtual void Configuration(EntityTypeBuilder<T> builder)
    {
        Configure(builder);
    }
}