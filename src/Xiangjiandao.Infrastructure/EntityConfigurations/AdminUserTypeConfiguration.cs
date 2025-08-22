using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class AdminUserTypeConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("t_admin_user");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.Email)
            .HasColumnName("email")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty)
            .HasComment("注册邮箱");
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
            .HasComment("手机区号");
        builder.Property(t => t.Avatar)
            .HasColumnName("avatar")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty)
            .HasComment("用户头像");
        builder.Property(t => t.Role)
            .HasColumnName("role")
            .HasColumnType("int")
            .HasDefaultValue(RoleType.Unknown)
            .HasComment("用户角色");
        builder.Property(t => t.Special)
            .HasColumnName("special")
            .HasColumnType("tinyint")
            .HasDefaultValue(false)
            .HasComment("是否特殊账号超级管理员");
        builder.Property(t => t.SecretData)
            .HasColumnName("secret_data")
            .HasColumnType("json")
            .HasComment("密码摘要");
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
