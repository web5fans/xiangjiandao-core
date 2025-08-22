using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.GlobalConfigAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class GlobalConfigTypeConfiguration : IEntityTypeConfiguration<GlobalConfig>
{
    public void Configure(EntityTypeBuilder<GlobalConfig> builder)
    {
        builder.ToTable("t_global_config");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.FundScale)
            .HasColumnName("fund_scale")
            .HasColumnType("bigint")
            .HasComment("基金规模");
        builder.Property(t => t.IssuePointsScale)
            .HasColumnName("issue_points_scale")
            .HasColumnType("bigint")
            .HasComment("发行积分规模");
        builder.Property(t => t.FoundationPublicDocument)
            .HasColumnName("foundation_public_document")
            .HasColumnType("json")
            .HasComment("基金会公开信息文件");
        builder.Property(t => t.ProposalApprovalVotes)
            .HasColumnName("proposal_approval_votes")
            .HasColumnType("int")
            .HasComment("提案通过票数");
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
