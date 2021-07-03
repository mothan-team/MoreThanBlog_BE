using System.Threading.Tasks;
using Abstraction.Service.UserService;
using Core.Model.Common;
using Core.Model.User;
using Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MoreThanBlog.Controllers
{
    [Route(Endpoint)]
    public class UserController : BaseController
    {
        private const string Endpoint = "user";

        private const string Login = "login";
        private const string Create = Endpoint + "/create";
        private const string Update = Endpoint + "/{id}";
        private const string Delete = Endpoint + "/{id}";
        private const string Get = Endpoint + "/{id}";
        private const string Filter = Endpoint;
        private const string ResetPassWord = Endpoint + "/reset-password";

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(Login)]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Result", typeof(JwtTokenResultModel))]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            var rs = await _userService.LoginAsync(model);
            return Ok(rs);
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(Create)]
        [SwaggerResponse(StatusCodes.Status200OK, "Result", typeof(string))]
        public async Task<IActionResult> AddAsync([FromBody] AddUserModel model)
        {
            var rs = await _userService.AddAsync(model);
            return Ok(rs);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut(Update)]
        [SwaggerResponse(StatusCodes.Status200OK, "Result", typeof(string))]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] AddUserModel model)
        {
            await _userService.UpdateAsync(id, model);
            return NoContent();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete(Delete)]
        [SwaggerResponse(StatusCodes.Status200OK, "Result", typeof(string))]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// get user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet(Get)]
        [SwaggerResponse(StatusCodes.Status200OK, "Result", typeof(LoggedInUser))]
        public async Task<IActionResult> GetAsync([FromRoute] string id)
        {
            var rs = await _userService.GetAsync(id);
            return Ok(rs);
        }

        /// <summary>
        /// Filter users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet(Filter)]
        [SwaggerResponse(StatusCodes.Status200OK, "Result", typeof(PagedResponseModel<LoggedInUser>))]
        public async Task<IActionResult> FilterAsync([FromQuery] FilterUserRequestModel model)
        {
            var rs = await _userService.FilterAsync(model);
            return Ok(rs);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(ResetPassWord)]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Result")]
        public async Task<IActionResult> SetPasswordAsync([FromBody] SetPasswordModel model)
        {
            await _userService.SetPasswordAsync(model);
            return NoContent();
        }
    }
}