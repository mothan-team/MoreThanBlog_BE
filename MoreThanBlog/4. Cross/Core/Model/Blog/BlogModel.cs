using System;
using System.Collections.Generic;
using Core.Model.Category;
using Core.Model.File;

namespace Core.Model.Blog
{
    public class BlogModel
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset LastUpdatedTime { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public string Content { get; set; }

        public int ReadTime { get; set; } // minutes

        public FileModel MainImage { get; set; }

        public bool IsActive { get; set; }

        public SortCategoryModel[] Categories { get; set; }

    }
}