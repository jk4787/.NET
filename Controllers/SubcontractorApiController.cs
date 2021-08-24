using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Subcontractors;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Subcontractor;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/subcontractors")]
    [ApiController]
    public class SubcontractorApiController : BaseApiController
    {
        private ISubcontractorServices _service = null;
        private IAuthenticationService<int> _authService = null;

        public SubcontractorApiController(ISubcontractorServices service
            , IAuthenticationService<int> authService
            , ILogger<SubcontractorApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Subcontractor>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Subcontractor subcontractor = _service.Get(id);

                if (subcontractor == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemResponse<Subcontractor> { Item = subcontractor };
                }
            }
            catch (Exception ex)
            {

                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error {ex.Message}");
            }

            return StatusCode(iCode, response);
        }

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<Subcontractor>>> GetPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Subcontractor> page = _service.GetPaginated(pageIndex, pageSize);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Subcontractor>> { Item = page };
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
        public ActionResult<ItemResponse<Paged<Subcontractor>>> SearchByOrg(int pageIndex, int pageSize, string q)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Subcontractor> page = _service.SearchByOrg(pageIndex, pageSize, q);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Subcontractor>> { Item = page };
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

        [HttpGet("paged/organization")]
        public ActionResult<ItemResponse<Paged<Subcontractor>>> SelectByOrgId(int pageIndex, int pageSize, int orgId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Subcontractor> page = _service.SelectByOrgId(pageIndex, pageSize, orgId);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Subcontractor>> { Item = page };
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
        public ActionResult<SuccessResponse> Update(SubcontractorMultipleUpdateRequest model)
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
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("status/{id:int}")]
        public ActionResult<SuccessResponse> UpdateStatus(int id, bool status)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.UpdateStatus(id, status);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> AddMultiple(SubcontractorMultipleAddRequest model)
        {
            ObjectResult result;
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

        [HttpGet("industrytypes")]
        public ActionResult<ItemsResponse<IndustryType>> GetAll()
        {
            {
                int code = 200;
                BaseResponse response = null;

                try
                {
                    List<IndustryType> list = _service.GetAll();

                    if (list == null)
                    {
                        code = 404;
                        response = new ErrorResponse("App resource not found.");
                    }
                    else
                    {
                        response = new ItemsResponse<IndustryType> { Items = list };
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
        }

        [HttpGet("expertisetypes")]
        public ActionResult<ItemsResponse<FieldExpertiseType>> GetFieldTypes()
        {
            {
                int code = 200;
                BaseResponse response = null;

                try
                {
                    List<FieldExpertiseType> list = _service.GetFieldTypes();

                    if (list == null)
                    {
                        code = 404;
                        response = new ErrorResponse("App resource not found.");
                    }
                    else
                    {
                        response = new ItemsResponse<FieldExpertiseType> { Items = list };
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
        }

        [HttpGet("types")]
        public ActionResult<ItemResponse<TypeSelector>> GetAllTypes()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                TypeSelector result = _service.GetAllTypes();

                if (result == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<TypeSelector> { Item = result };
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