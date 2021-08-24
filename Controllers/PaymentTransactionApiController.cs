using Microsoft.AspNetCore.Components;
using Sabio.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Services;
using Sabio.Web.Models.Responses;
using Sabio.Models.Requests;
using Sabio.Models.Domain;
using System;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Sabio.Models;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/payments/transactions")]
    [ApiController]
    public class PaymentTransactionApiController : BaseApiController
    {
        private IPaymentTransactionService _service = null;
        private IAuthenticationService<int> _authService = null;

        public PaymentTransactionApiController(IPaymentTransactionService service, IAuthenticationService<int> authService, ILogger<JobApiController> logger) :base(logger)
        {
            _service = service;
            _authService = authService;
        }
        [HttpGet("{Id:int}")]
        public ActionResult<ItemResponse<PaymentTransactions>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                PaymentTransactions paymentTransactions = _service.Get(id);

                if (paymentTransactions == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<PaymentTransactions> { Item = paymentTransactions };
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

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(PaymentTransactionAddRequest model)
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

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<PaymentTransactions>>> GetAllByPagination(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<PaymentTransactions> paymentTransactions = _service.GetAllPaged(pageIndex, pageSize);
                if (paymentTransactions == null)

                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {

                    response = new ItemResponse<Paged<PaymentTransactions>> { Item = paymentTransactions };
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

        [HttpGet("createdbypaged")]
        public ActionResult<ItemResponse<Paged<PaymentTransactions>>> GetByCreatedByPaged(int pageIndex, int pageSize, int createdBy)
        {
            int code = 200;
            BaseResponse response = null;
            
            try
            {
                Paged<PaymentTransactions> paymentTransactions = _service.GetCreatedByAllPaged(pageIndex, pageSize, createdBy);
                if (paymentTransactions == null)

                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {

                    response = new ItemResponse<Paged<PaymentTransactions>> { Item = paymentTransactions };
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
