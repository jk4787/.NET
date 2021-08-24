using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api
{
    [Route("api/paymentaccounts")]
    public class PaymentAccountApiController : BaseApiController
    {
        private IPaymentAccountServices _service = null;
        private IAuthenticationService<int> _authService = null;

        public PaymentAccountApiController(IPaymentAccountServices service, ILogger<PaymentAccountApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(PaymentAccountAddRequest model)
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


        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<PaymentAccount>> Get(int id)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                PaymentAccount account = _service.Get(id);
                if (account == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<PaymentAccount> { Item = account };
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
        public ActionResult<ItemResponse<Paged<PaymentAccount>>> SearchByOrgId(int pageIndex, int pageSize, int orgId)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                Paged<PaymentAccount> account = _service.SearchByOrgId(pageIndex, pageSize, orgId);
                if (account == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<PaymentAccount>> { Item = account };
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
        public ActionResult<ItemResponse<Paged<PaymentAccount>>> CreatedBy(int pageIndex, int pageSize, int id)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                Paged<PaymentAccount> account = _service.SearchByCreatedBy(pageIndex, pageSize, id);
                if (account == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<PaymentAccount>> { Item = account };
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
        public ActionResult<ItemResponse<Paged<PaymentAccount>>> GetAll(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                Paged<PaymentAccount> account = _service.GetAll(pageIndex, pageSize);
                if (account == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<PaymentAccount>> { Item = account };
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
        public ActionResult<SuccessResponse> Update(PaymentAccountUpdateRequest model)
        {
            int code = 200;
            BaseResponse response;
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
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

    }
}
