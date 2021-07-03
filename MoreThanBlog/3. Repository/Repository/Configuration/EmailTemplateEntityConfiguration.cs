using Abstraction.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration
{
    public class EmailTemplateEntityConfiguration : EntityConfiguration<EmailTemplateEntity>
    {
        public override void Configure(EntityTypeBuilder<EmailTemplateEntity> builder)
        {
            builder.ToTable("EmailTemplate");
            base.Configure(builder);
        }
    }
}