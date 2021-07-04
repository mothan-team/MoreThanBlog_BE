using System.Linq;
using Abstraction.Repository.Model;
using AutoMapper;
using Core.Model.Blog;
using Core.Model.Category;
using Core.Model.File;

namespace Mapper.Blog
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<AddBlogModel, BlogEntity>().IgnoreAllNonExisting();

            CreateMap<BlogEntity, BlogModel>().IgnoreAllNonExisting()
                .ForMember(x => x.CreatedBy, y => y.MapFrom(z => $"{z.Creator.FirstName} {z.Creator.LastName}"))
                .ForMember(x => x.MainImage, y => y.MapFrom(z => z.MainImage == null
                    ? null
                    : new FileModel
                    {
                        Id = z.MainImageId,
                        Name = z.MainImage.Name,
                        Extension = z.MainImage.Extension,
                        Preview = z.MainImage.Slug,
                        Slug = z.MainImage.Slug,
                    }))
            .ForMember(x => x.Categories, y => y.MapFrom(z => z.BlogCategories.Where(x => x.DeletedTime == null)
                .Select(x => new SortCategoryModel
                {
                    Id = x.CategoryId,
                    Name = x.Category.Name
                }).ToArray()));
        }
    }
}