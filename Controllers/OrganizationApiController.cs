using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;


namespace Sabio.Web.Api.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationApiController : BaseApiController
    {
        private IOrganizationService _service = null;
        private IAuthenticationService<int> _authService = null;
        public OrganizationApiController(IOrganizationService service, IAuthenticationService<int> authService, ILogger<OrganizationApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;

        }

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<Organization>>> GetAllPaged(int pageIndex, int pageSize)
        {
            int code = 200;

            BaseResponse response = null;

            try
            {
                Paged<Organization> organizations = _service.GetAllPaginated(pageIndex, pageSize);
                if (organizations == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");

                }
                else
                {
                    response = new ItemResponse<Paged<Organization>> { Item = organizations };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);

            }
            return StatusCode(code, response);
        }

        [HttpGet("pagination")]
        public ActionResult<ItemResponse<Paged<Organization>>> GetAllPaginatedDetails(int pageIndex, int pageSize)
        {
            int code = 200;

            BaseResponse response = null;

            try
            {
                Paged<Organization> organizations = _service.GetAllPaginatedDetails(pageIndex, pageSize);
                if (organizations == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");

                }
                else
                {
                    response = new ItemResponse<Paged<Organization>> { Item = organizations };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);

            }
            return StatusCode(code, response);
        }


        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(OrganizationUpdateRequest model)
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

        [HttpPut("{id:int}/details")]
        public ActionResult<SuccessResponse> UpdateDetails(OrganizationUpdateRequest model)
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
        public ActionResult<ItemResponse<int>> Add(OrganizationAddRequest model)
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
        public ActionResult<ItemResponse<Organization>> GetDetails(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Organization organization = _service.GetDetails(id);

                if (organization == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Organization> { Item = organization };
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

        [HttpGet]
        public ActionResult<ItemsResponse<OrganizationType>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                List<OrganizationType> list = _service.GetAll();
                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<OrganizationType> { Items = list };
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

        [HttpGet("{id:int}/workers")]
        public ActionResult<ItemResponse<Paged<OrganizationEmployeeSubcontractor>>> GetWorkers(int id, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<OrganizationEmployeeSubcontractor> paged = _service.GetWorkers(id, pageIndex, pageSize);
                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<OrganizationEmployeeSubcontractor>> { Item = paged };
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

        [HttpGet("search/{id:int}/workers")]
        public ActionResult<ItemResponse<Paged<OrganizationEmployeeSubcontractor>>> SearchWorkers(int id, int pageIndex, int pageSize, string q)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<OrganizationEmployeeSubcontractor> paged = _service.SearchWorkers(id, pageIndex, pageSize, q);
                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<OrganizationEmployeeSubcontractor>> { Item = paged };
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

        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<Organization>>> Search(int pageIndex, int pageSize, string q)
        {
            ActionResult result = null;
            try
            {
                Paged<Organization> paged = _service.SearchPaginated(pageIndex, pageSize, q);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found"));
                }
                else
                {
                    ItemResponse<Paged<Organization>> response = new ItemResponse<Paged<Organization>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
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