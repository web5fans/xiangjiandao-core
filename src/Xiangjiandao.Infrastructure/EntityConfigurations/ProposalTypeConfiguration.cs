using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class ProposalTypeConfiguration : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.ToTable("t_proposal");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("提案名称");
        builder.Property(t => t.InitiatorId)
            .HasColumnName("initiator_id")
            .HasComment("发起方名称");
        builder.Property(t => t.InitiatorDid)
            .HasColumnName("initiator_did")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("发起方 Did");
        builder.Property(t => t.InitiatorDomainName)
            .HasColumnName("initiator_domain_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("发起方 DomainName");
        builder.Property(t => t.InitiatorEmail)
            .HasColumnName("initiator_email")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("发起方邮箱");
        builder.Property(t => t.InitiatorName)
            .HasColumnName("initiator_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("发起方名称");
        builder.Property(t => t.InitiatorAvatar)
            .HasColumnName("initiator_avatar")
            .HasColumnType("varchar")
            .HasMaxLength(512)
            .HasDefaultValue(string.Empty)
            .HasComment("发起方头像");
        builder.Property(t => t.EndAt)
            .HasColumnName("end_at")
            .HasColumnType("datetime")
            .HasDefaultValue(DateTimeOffset.MinValue)
            .HasComment("投票截至时间");
        builder.Property(t => t.AttachId)
            .HasColumnName("attach_id")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("附件 Id");
        builder.Property(t => t.TotalVotes)
            .HasColumnName("total_votes")
            .HasColumnType("bigint")
            .HasComment("总投票数");
        builder.Property(t => t.OpposeVotes)
            .HasColumnName("oppose_votes")
            .HasColumnType("bigint")
            .HasComment("反对票数");
        builder.Property(t => t.AgreeVotes)
            .HasColumnName("agree_votes")
            .HasColumnType("bigint")
            .HasComment("同意票数");
        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasColumnType("int")
            .HasDefaultValue(ProposalStatus.Unknown)
            .HasComment("提案状态");
        builder.Property(t => t.OnShelf)
            .HasColumnName("on_shelf")
            .HasColumnType("tinyint")
            .HasDefaultValue(false)
            .HasComment("是否上架");
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
        
        builder.HasMany(t => t.Votes)
            .WithOne(t => t.Proposal)
            .HasForeignKey(t => t.ProposalId);

        builder.Navigation(t => t.Votes).AutoInclude();
    }
}
