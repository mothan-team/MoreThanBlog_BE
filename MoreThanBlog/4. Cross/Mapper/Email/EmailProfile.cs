using Abstraction.Repository.Model;
using AutoMapper;
using Core.Model.Email;

namespace Mapper.Email
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<EmailTemplateEntity, EmailTemplateModel>().IgnoreAllNonExisting();
        }
    }
}