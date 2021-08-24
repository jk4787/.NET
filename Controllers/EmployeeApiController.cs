using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sabio.Web.Api
{
    [Route("/api/employees")]
    [ApiController]
    public class EmployeeApiController : BaseApiController
    {
        private IEmployeeServices _service = null;
        private IAuthenticationService<int> _authService = null;

        public EmployeeApiController(IEmployeeServices service, ILogger<EmployeeApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> AddMultiple(EmployeeMultipleAddRequest model)
        {

            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.AddMultiple(model, userId);
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

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Employee>> Get(int id)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                Employee employee = _service.Get(id);
                if (employee == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Employee> { Item = employee };
                }

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("details/{id:int}")]
        public ActionResult<ItemResponse<Employee>> GetByIdDetailed(int id)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                Employee employee = _service.Get(id);
                if (employee == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Employee> { Item = employee };
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
            BaseResponse response;
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

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<Employee>>> GetPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response;


            try
            {
                Paged<Employee> pagedResult = _service.GetPaginated(pageIndex, pageSize);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Employee>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("organization/search")]
        public ActionResult<ItemResponse<Paged<Employee>>> SearchByOrganizationId(int pageIndex, int pageSize, int query)
        {
            int code = 200;
            BaseResponse response = null;


            try
            {
                Paged<Employee> pagedResult = _service.SearchByOrganizationId(pageIndex, pageSize, query);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Employee>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("department/{departmentId:int}/{id:int}")]
        public ActionResult<ItemResponse<Paged<Employee>>> SelectByDepartment(int pageIndex, int pageSize, int departmentId, int orgId)
        {
            int code = 200;
            BaseResponse response = null;


            try
            {
                Paged<Employee> pagedResult = _service.SearchByDepartment(pageIndex, pageSize, departmentId, orgId);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Employee>> { Item = pagedResult };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(EmployeeUpdateRequest model)
        {
            int code = 200;
            BaseResponse response;
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

        [HttpPut("active/{id:int}")]
        public ActionResult<SuccessResponse> UpdateStatus(int id, bool status)
        {
            int code = 200;

            BaseResponse response;
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdateStatus(id, status, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("types")]
        public ActionResult<ItemResponse<EmployeeSelector>> GetAllTypes()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                EmployeeSelector result = _service.GetAllTypes();

                if (result == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<EmployeeSelector> { Item = result };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

    }
}
