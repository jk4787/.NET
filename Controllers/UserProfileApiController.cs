using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;


namespace Sabio.Web.Api.Controllers
{
    [Route("api/userprofiles")]
    [ApiController]
    public class UserProfileApiController : BaseApiController
    {
        private IUserProfileService _service = null;
        private IAuthenticationService<int> _authService = null;
        public UserProfileApiController(IUserProfileService service, IAuthenticationService<int> authService, ILogger<UserProfileApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;

        }

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<UserProfile>>> GetAllByPagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            BaseResponse response = null;
        
            try
            {
                Paged<UserProfile> UserProfiles = _service.GetAllPaginated(pageIndex, pageSize);
                if (UserProfiles == null)
                {
                    result = NotFound(new ErrorResponse("Records not found"));

                }
                else
                {
                    response = new ItemResponse<Paged<UserProfile>> { Item = UserProfiles };
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message));

            }
            return result;
        }


        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(UserProfileUpdateRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);
                BaseResponse response = new SuccessResponse();
                result = Ok200(response);

            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, error);
            }
            return result;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(UserProfileAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);

            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, error);
            }
            return result;
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<UserProfile>> Get(int id)
        {
            int iCode = 201;
            BaseResponse response = null;
            try
            {
                UserProfile userProfile = _service.Get(id);

                if (userProfile == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<UserProfile> { Item = userProfile };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response);

        }
        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id, int userId)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                _service.Delete(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(iCode, response);
        }

    }
}