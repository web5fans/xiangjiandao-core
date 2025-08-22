using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("t_user");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.Email)
            .HasColumnName("email")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("邮箱");
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
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty)
            .HasComment("用户头像");
        builder.Property(t => t.NickName)
            .HasColumnName("nick_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("用户昵称");
        builder.Property(t => t.Introduction)
            .HasColumnName("introduction")
            .HasColumnType("varchar")
            .HasMaxLength(512)
            .HasDefaultValue(string.Empty)
            .HasComment("简介");
        builder.Property(t => t.DomainName)
            .HasColumnName("domain_name")
            .HasColumnType("varchar")
            .HasMaxLength(256)
            .HasDefaultValue(string.Empty)
            .HasComment("完整用户名，域名");
        builder.Property(t => t.Did)
            .HasColumnName("did")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .HasDefaultValue(string.Empty)
            .HasComment("DID");
        builder.Property(t => t.Score)
            .HasColumnName("score")
            .HasColumnType("bigint")
            .HasComment("积分");
        builder.Property(t => t.NodeUser)
            .HasColumnName("node_user")
            .HasColumnType("tinyint")
            .HasDefaultValue(false)
            .HasComment("是否是节点用户");
        builder.Property(t => t.Disable)
            .HasColumnName("disable")
            .HasColumnType("tinyint")
            .HasDefaultValue(false)
            .HasComment("是否禁用");
        builder.Property(t => t.RowVersion)
            .HasColumnName("row_version")
            .HasColumnType("int")
            .HasDefaultValue(new RowVersion(0))
            .HasComment("行版本，处理并发问题");
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
