using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/jobattendance")]
    [ApiController]
    public class JobAttendanceApiController : BaseApiController
    {
        private IJobAttendanceServices _service = null;
        private IAuthenticationService<int> _authService = null;

        public JobAttendanceApiController(IJobAttendanceServices service, ILogger<JobAttendanceApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }
        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(JobAttendanceAddRequest model)
        {
            ObjectResult result;

            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(JobAttendanceUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);

        }

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<JobAttendance>>> GetAll(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            

            try
            {
                Paged<JobAttendance> pagedResult = _service.GetAll(pageIndex, pageSize);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<JobAttendance>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<JobAttendance>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                JobAttendance attendance = _service.GetById(id);
                if (attendance == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<JobAttendance> { Item = attendance };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("organization/{id:int}/search")]
        public ActionResult<ItemResponse<Paged<JobAttendance>>> GetByOrganization(int pageIndex, int pageSize, int orgId)
        {
            int code = 200;
            BaseResponse response = null;
           

            try
            {
                Paged<JobAttendance> pagedResult = _service.GetByOrganization(pageIndex, pageSize, orgId);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<JobAttendance>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }


        [HttpGet("createdby/{id:int}/search")]
        public ActionResult<ItemResponse<Paged<JobAttendance>>> GetByCreatedBy(int pageIndex, int pageSize, int id)
        {
            int code = 200;
            BaseResponse response = null;


            try
            {
                Paged<JobAttendance> pagedResult = _service.GetByCreatedBy(pageIndex, pageSize, id);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<JobAttendance>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _service.Delete(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }


        #region **OBSOLETE**
        [HttpGet("entity/{id:int}")]
        public ActionResult<ItemResponse<JobAttendance>> SearchByEntity(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                JobAttendance attendance = _service.SearchByEntity(id);
                if (attendance == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<JobAttendance> { Item = attendance };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
        #endregion

    }
}