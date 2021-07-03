using Abstraction.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration
{
    public class UserOtpEntityConfiguration : EntityConfiguration<UserOtpEntity>
    {
        public override void Configure(EntityTypeBuilder<UserOtpEntity> builder)
        {
            builder.ToTable("UserOtp");
            base.Configure(builder);
        }
    }
}