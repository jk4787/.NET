using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Jobs;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;


namespace Sabio.Web.Api.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobApiController : BaseApiController
    {
        private IJobService _service = null;
        private IAuthenticationService<int> _authService = null;
        public JobApiController(IJobService service, IAuthenticationService<int> authService, ILogger<JobApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;

        }
        [HttpGet("types")]
        public ActionResult<ItemResponse<JobSelectors>> GetAllTypes()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                JobSelectors result = _service.GetAllTypes();

                if (result == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<JobSelectors> { Item = result };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<Job>>> GetAllByPagination(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Job> jobs = _service.GetAllPaginated(pageIndex, pageSize);
                if (jobs == null)

                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {

                    response = new ItemResponse<Paged<Job>> { Item = jobs };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());

            }
            return StatusCode(code, response);
        }

        [HttpGet("details/paged")]
        public ActionResult<ItemResponse<Paged<Job>>> GetAllDetailsPaged(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Job> jobs = _service.GetAllDetailsPaginated(pageIndex, pageSize);
                if (jobs == null)

                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {

                    response = new ItemResponse<Paged<Job>> { Item = jobs };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());

            }
            return StatusCode(code, response);
        }
        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(JobUpdateRequest model)
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

        [HttpPut("details/{id:int}")]
        public ActionResult<SuccessResponse> UpdateDetails(JobDetailsUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdateDetails(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);

        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(JobAddRequest model)
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
        [HttpPost("details")]
        public ActionResult<ItemResponse<int>> AddDetails(JobDetailsAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.AddDetails(model, userId);
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
        public ActionResult<ItemResponse<Job>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Job job = _service.Get(id);

                if (job == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Job> { Item = job };
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


        [HttpGet("details/{id:int}")]
        public ActionResult<ItemResponse<Job>> GetDetails(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Job job = _service.GetDetails(id);

                if (job == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Job> { Item = job };
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