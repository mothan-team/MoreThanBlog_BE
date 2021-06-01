using Microsoft.AspNetCore.Mvc;
using MoreThanBlog.Filter;

namespace MoreThanBlog.Controllers
{
    [ModelValidationFilter]
    public class BaseController : ControllerBase
    {
    }
}