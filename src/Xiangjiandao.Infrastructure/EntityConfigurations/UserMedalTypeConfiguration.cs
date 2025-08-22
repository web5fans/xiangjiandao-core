using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class UserMedalTypeConfiguration : IEntityTypeConfiguration<UserMedal>
{
    public void Configure(EntityTypeBuilder<UserMedal> builder)
    {
        builder.ToTable("t_user_medal");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .HasComment("用户 Id");
        builder.Property(t => t.MedalId)
            .HasColumnName("medal_id")
            .HasComment("勋章 Id");
        builder.Property(t => t.NickName)
            .HasColumnName("nick_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("用户昵称");
        builder.Property(t => t.Avatar)
            .HasColumnName("avatar")
            .HasColumnType("varchar")
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty)
            .HasComment("用户头像");
        builder.Property(t => t.Phone)
            .HasColumnName("phone")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("用户手机号");
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
        builder.Property(t => t.AttachId)
            .HasColumnName("attach_id")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("封面 Id");
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("勋章名称");
        builder.Property(t => t.GetTime)
            .HasColumnName("get_time")
            .HasColumnType("datetime")
            .HasDefaultValue(DateTimeOffset.MinValue)
            .HasComment("获得时间");
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
