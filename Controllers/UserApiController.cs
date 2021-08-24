using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Enums;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : BaseApiController
    {
        private IAuthenticationService<int> _authService = null;
        private IUserService _data = null;
        private IEmailService _emailService = null;
        public UserApiController(IUserService userService, IAuthenticationService<int> authService, IEmailService emailService, ILogger<UserApiController> logger) : base(logger)
        
        {         
            _authService = authService;
            _emailService = emailService;
            _data = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ItemResponse<int>> Register(UserAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                string token = Guid.NewGuid().ToString();
                int id = _data.Register(model);
                _data.AddRegistrationToken(id, token, TokenType.Register);
                _emailService.ConfirmEmail(model.Email, token);

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<ItemResponse<User>> Login(UserLoginRequest model)
        {
            int icode = 200;
            BaseResponse response = null;
            ;
            try
            {
                User user = _data.GetByEmail(model);
                if(user != null)
                {

                        _data.LogInAsync(user);

                    response = new ItemResponse<User> { Item = user};
                }
                else
                {
                    icode = 404;
                    response = new ErrorResponse("The Email and Password do not match our crudentials");

                }
               
            }
            catch (Exception ex)
            {
                icode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(icode, response);
        }

        [HttpGet("logout")]
        public async Task<ActionResult<SuccessResponse>> LogoutAsync()
        {

            int icode = 200;
            BaseResponse response = null;
            
            try
            {
                await _authService.LogOutAsync();
                icode = 200;
                response = new SuccessResponse();
            }
            catch
            {
                icode = 401;
                response = new ErrorResponse("Not Logged In");
            }
            
            return StatusCode(icode, response);
        }

        [AllowAnonymous]
        [HttpPut("confirm")]
        public ActionResult<ItemResponse<int>> Confirm(string token)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _data.ConfirmEmail(token);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<UserDetailed>>> GetPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<UserDetailed> pagedResult = _data.GetPaginated(pageIndex, pageSize);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<UserDetailed>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<UserDetailed>>> SearchDetails(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<UserDetailed> pagedResult = _data.SearchDetails(pageIndex, pageSize, query);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<UserDetailed>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("types")]
        public ActionResult<ItemsResponse<UserStatus>> GetTypes()
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                List<UserStatus> list = _data.GetStatusTypes();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemsResponse<UserStatus> { Items = list };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("{id:int}/status")]
        public ActionResult<SuccessResponse> UpdateStatus(UserUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _data.UpdateStatus(model);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        //[HttpPut("{id:int}")]
        //public ActionResult<SuccessResponse> UpdateConfirm(UserUpdateRequest model)
        //{ 

        //}

        //[HttpGet("{email: string}")]
        //public ActionResult<ItemResponse<MyUser>> GetByEmail(string email)
        //{
        //    int icode = 200;
        //    BaseResponse response = null;
        //    try
        //    {
        //        MyUser user = _service.Get(email);

        //        if (user == null)
        //        {
        //            icode = 404;
        //            response = new ErrorResponse("application resource not found.");
        //        }
        //        else
        //        {
        //            response = new ItemResponse<MyUser> { Item = user };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        icode = 500;
        //        base.Logger.LogError(ex.ToString());
        //        response = new ErrorResponse($"generic error: {ex.Message}");
        //    }
        //    return StatusCode(icode, response);
        //}

      

        [AllowAnonymous]
        [HttpPost("recoverpassword")]
        public ActionResult<SuccessResponse> ForgotPassword(UserRecoverPassword model)
        {
            ObjectResult result = null;
            try
            {
                string token = Guid.NewGuid().ToString();
                _data.ForgotPassword(model.Email, token);
                _emailService.ResetPassword(model.Email, token);
                result = Ok200(new SuccessResponse());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPut("resetpassword")]
        public ActionResult<SuccessResponse> ResetPassword(UserResetPassword model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _data.ResetPassword(model);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
    }
}