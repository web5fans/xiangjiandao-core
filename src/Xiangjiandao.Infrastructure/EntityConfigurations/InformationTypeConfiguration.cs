using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class InformationTypeConfiguration : IEntityTypeConfiguration<Information>
{
    public void Configure(EntityTypeBuilder<Information> builder)
    {
        builder.ToTable("t_information");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty)
            .HasComment("公告名称");
        builder.Property(t => t.AttachId)
            .HasColumnName("attach_id")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty)
            .HasComment("附件 Id");
        builder.Property(t => t.Sort)
            .HasColumnName("sort")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .HasComment("公告排序");
        builder.Property(t => t.RowVersion)
            .HasColumnName("row_version")
            .HasColumnType("int")
            .HasDefaultValue(new RowVersion(0))
            .HasComment("并发乐观锁");
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime")
            .HasDefaultValueSql("current_timestamp")
            .HasComment("创建时间");
        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("创建者");
        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("datetime")
            .HasDefaultValueSql("current_timestamp")
            .HasComment("更新时间");
        builder.Property(t => t.UpdatedBy)
            .HasColumnName("updated_by")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("更新者");
        builder.Property(t => t.Deleted)
            .HasColumnName("deleted")
            .HasColumnType("tinyint")
            .HasDefaultValue(new Deleted(false))
            .HasComment("是否删除");
    }
}
