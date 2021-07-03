using Core.Constants.Enum;
using Core.Model.Blog;
using FluentValidation;

namespace Core.Validator.Blog
{
    public class AddBlogModelValidator : AbstractValidator<AddBlogModel>
    {
        public AddBlogModelValidator()
        {
            When(x => x.Status == BlogStatus.Public, () =>
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Desc).NotEmpty();
                RuleFor(x => x.CategoryIds).NotEmpty();
                RuleFor(x => x.Content).NotEmpty();
                RuleFor(x => x.MainImageId).NotEmpty();
            });
        }
    }
}