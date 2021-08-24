using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dwolla.Client.Models.Responses;
using Dwolla.Client.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Dwolla;
using Sabio.Services;

using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using BaseResponse = Sabio.Web.Models.Responses.BaseResponse;
using ErrorResponse = Sabio.Web.Models.Responses.ErrorResponse;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/dwolla")]
    [ApiController]
    public class OAuthController : BaseApiController
    {
        private IOAuthService _service = null;
        private IAuthenticationService<int> _authService = null;
        private IPaymentAccountServices _paymentService = null;
        private IUserService _tokenService = null;
        public OAuthController(IOAuthService service, IPaymentAccountServices paymentService, IUserService tokenService, ILogger<PaymentAccountApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
            _paymentService = paymentService;
            _tokenService = tokenService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<OAuthResponse>> GetToken()
        {
            ObjectResult result = null;
            try
            {
                string tokenString = _service.CheckToken();
                ItemResponse<OAuthResponse> response = null;
                if (tokenString == "")
                {
                    
                    Task<OAuthResponse> token = _service.GetAsync();
                    response = new ItemResponse<OAuthResponse>() { Item = token.Result };
                    IUserAuthData user = _authService.GetCurrentUser();
                    _tokenService.AddRegistrationToken(user.Id, response.Item.access_token.ToString(), Sabio.Models.Enums.TokenType.AccessToken);
                }
                else
                {
                    OAuthResponse resp = new OAuthResponse();
                    resp.access_token = tokenString;
                    response = new ItemResponse<OAuthResponse>() { Item = resp };
                }
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
        [HttpGet]
        public ActionResult<ItemResponse<string>> GetAccountDetailsAsync()
        {

            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Task<AccountDetails> account = _service.GetAccountDetailsAsync();

                if (account == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {

                    response = new ItemResponse<AccountDetails> { Item = account.Result };
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
        //[HttpPost("customer")]
        //public ActionResult<ItemResponse<string>> CreateCustomer()
        //{
        //    int iCode = 200;
        //  BaseResponse response = null;
        //    try
        //    {
        //        Task<string> customer = _service.CreateCustomer();

        //        if (customer == null)
        //        {
        //            iCode = 404;
        //            response = new ErrorResponse("Application Resource not found.");
        //        }
        //        else
        //        {

        //            response = new ItemResponse<string> { Item = customer.Result };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        iCode = 500;
        //        base.Logger.LogError(ex.ToString());
        //        response = new ErrorResponse(ex.Message);
        //    }

        //    return StatusCode(iCode, response);
        //}
        [HttpPost("customers")]
        public ActionResult<ItemResponse<string>> NewCustomer(string DwollaId, string AccessToken)
        {
            int iCode = 200;
            BaseResponse response;
            try
            {
                IUserAuthData user = _authService.GetCurrentUser();
                var call = _service.GetAsync();// GetTokenAsync();
                var token = call.Result.access_token;
                string[] names = user.Name.Split(" ");

                var body = new Dictionary<string, string>
            {
                    { "email", user.Email },
                    { "firstName", names[0] },
                    { "lastName", names[names.Length - 1] },
                    { "type", "receive-only" }
                };
                Task<string> customer = _service.NewCustomer(body, token);


                if (customer == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    _paymentService.CreateAccount("DwollaId", customer.Result.ToString(), token, user.Id);
                    response = new ItemResponse<string> { Item = customer.Result.ToString() };
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
    }
}