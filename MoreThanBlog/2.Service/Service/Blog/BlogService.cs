using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstraction.Repository;
using Abstraction.Repository.Model;
using Abstraction.Service.Blog;
using AutoMapper;
using Core.Errors;
using Core.Helper;
using Core.Model.Blog;
using Core.Model.Category;
using Core.Model.Common;
using Core.Model.File;
using Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Service.Blog
{
    public class BlogService : BaseService.Service, IBlogService
    {
        private readonly IRepository<BlogEntity> _blogRepository;
        private readonly IRepository<BlogCategoryEntity> _blogCategoryRepository;

        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BlogService(IUnitOfWork unitOfWork, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, mapper)
        {
            _blogRepository = UnitOfWork.GetRepository<BlogEntity>();
            _blogCategoryRepository = UnitOfWork.GetRepository<BlogCategoryEntity>();

            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveAsync(AddBlogModel model, CancellationToken cancellationToken = default)
        {
            CheckDuplicateTitle(model.Title, model.Id);

            var entity = _mapper.Map<BlogEntity>(model);

            if (string.IsNullOrWhiteSpace(model.Id)) // add
            {
                entity.Id = Guid.NewGuid().ToString("N");
                entity.BlogCategories = model.CategoryIds?.Select(x => new BlogCategoryEntity
                {
                    CategoryId = x
                }).ToList();

                _blogRepository.Add(entity);
            }
            else // update
            {
                _blogRepository.Update(entity, x => x.Title, x => x.Content, x => x.Desc,
                    x => x.IsActive, x => x.MainImageId, x => x.ReadTime, x => x.Status);

                _blogCategoryRepository.DeleteWhere(x => x.BlogId == model.Id, true);

                if (model.CategoryIds != null && model.CategoryIds.Any())
                {
                    _blogCategoryRepository.AddRange(model.CategoryIds?.Select(x => new BlogCategoryEntity
                    {
                        BlogId = model.Id,
                        CategoryId = x
                    }).ToArray());
                }
            }

            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task UpdateAsync(string id, AddBlogModel model, CancellationToken cancellationToken = default)
        {
            CheckDuplicateTitle(model.Title, id);

            CheckExist(id);

            var entity = _mapper.Map<BlogEntity>(model);
            entity.Id = id;

            _blogRepository.Update(entity, x => x.Title, x => x.Content, x => x.Desc, 
                x => x.IsActive, x => x.MainImageId, x => x.ReadTime, x => x.Status);

            _blogCategoryRepository.DeleteWhere(x => x.BlogId == id, true);

            _blogCategoryRepository.AddRange(model.CategoryIds.Select(x => new BlogCategoryEntity
            {
                BlogId = id,
                CategoryId = x
            }).ToArray());

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResponseModel<BlogModel>> FilterAsync(FilterBlogRequestModel model, CancellationToken cancellationToken = default)
        {
            var query = _blogRepository.Get();

            if (!string.IsNullOrWhiteSpace(model.Terms))
            {
                query = query.Where(x => x.Title.Contains(model.Terms) || x.Desc.Contains(model.Terms));
            }

            var rs = new PagedResponseModel<BlogModel>
            {
                Total = await query.CountAsync(cancellationToken: cancellationToken),
                Items = await _mapper.ProjectTo<BlogModel>(query
                        .Skip(model.Skip)
                        .Take(model.Take))
                    .ToListAsync(cancellationToken: cancellationToken)
            };

            if (rs.Items != null && rs.Items.Any())
            {
                foreach (var item in rs.Items)
                {
                    if (item.MainImage != null)
                    {
                        item.MainImage.Preview = _httpContextAccessor.HttpContext.Request.GetDomain() + item.MainImage.Slug;
                    }
                }
            }

            return rs;
        }

        public async Task<BlogModel> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            var rs = await _mapper.ProjectTo<BlogModel>(
                    _blogRepository.Get(x => x.Id == id))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (rs?.MainImage != null)
            {
                rs.MainImage.Preview = _httpContextAccessor.HttpContext.Request.GetDomain() + rs.MainImage.Slug;
            }

            return rs;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _blogRepository.DeleteWhere(x => x.Id == id);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        #region Utilities

        private void CheckDuplicateTitle(string title, string id = null)
        {
            var query = _blogRepository.Get(x => x.Title == title.Trim());

            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(x => x.Id != id);
            }

            if (query.Any())
            {
                throw new MoreThanBlogException(nameof(ErrorCode.DuplicateTitle), ErrorCode.DuplicateTitle);
            }
        }

        private void CheckExist(string id)
        {
            if (!_blogRepository.Get(x => x.Id == id).Any())
            {
                throw new MoreThanBlogException(nameof(ErrorCode.BlogNotFound), ErrorCode.BlogNotFound);
            }
        }

        #endregion
    }
}