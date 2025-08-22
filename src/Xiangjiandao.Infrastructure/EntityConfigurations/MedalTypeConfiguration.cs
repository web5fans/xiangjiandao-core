using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class MedalTypeConfiguration : IEntityTypeConfiguration<Medal>
{
    public void Configure(EntityTypeBuilder<Medal> builder)
    {
        builder.ToTable("t_medal");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.AttachId)
            .HasColumnName("attach_id")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("附件 Id");
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("勋章名称");
        builder.Property(t => t.FileId)
            .HasColumnName("file_id")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .HasDefaultValue(string.Empty)
            .HasComment("获得用户文件 Id");
        builder.Property(t => t.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("bigint")
            .HasComment("发放用户数量");
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
