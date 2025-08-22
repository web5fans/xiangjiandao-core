using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class ScoreDistributeRecordTypeConfiguration : IEntityTypeConfiguration<ScoreDistributeRecord>
{
    public void Configure(EntityTypeBuilder<ScoreDistributeRecord> builder)
    {
        builder.ToTable("t_point_distribute_record");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .HasComment("用户id");
        builder.Property(t => t.NickName)
            .HasColumnName("nick_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("昵称");
        builder.Property(t => t.Phone)
            .HasColumnName("phone")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("手机号");
        builder.Property(t => t.PhoneRegion)
            .HasColumnName("phone_region")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("用户手机区号");
        builder.Property(t => t.Email)
            .HasColumnName("email")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("邮箱");
        builder.Property(t => t.Score)
            .HasColumnName("score")
            .HasColumnType("bigint")
            .HasComment("发放积分");
        builder.Property(t => t.GetTime)
            .HasColumnName("get_time")
            .HasColumnType("datetime")
            .HasDefaultValueSql("current_timestamp")
            .HasComment("获取时间");
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
