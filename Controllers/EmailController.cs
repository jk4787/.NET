using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sabio.Models;
using Sabio.Models.AppSettings;
using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using SendGrid;
using SendGrid.Helpers.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sabio.Web.Api
{
    [Route("api/emails")]
    [ApiController]
    public class EmailApiController : BaseApiController
    {
        private IEmailService _service = null;
        private IAuthenticationService<int> _authService = null;
        private AppKeys _appKeys;
        public EmailApiController(IOptions<AppKeys> appKeys, IEmailService service, IAuthenticationService<int> authService, ILogger<EmailApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
            _appKeys = appKeys.Value;
        }

        //POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<Response>> Execute(EmailAddRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
               await _service.SendTestEmail(model);
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



