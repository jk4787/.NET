using System;
using System.Collections.Generic;
using System.IO;
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
    [Route("api/files")]
    [ApiController]
    public class FileApiController : BaseApiController
    {
        private IFileServices _service = null;
        private IAuthenticationService<int> _authService = null;

        public FileApiController(IFileServices service, ILogger<FileApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<FileUpload>> GetById(int id)
        {
            int code = 200;
            BaseResponse response;
            try
            {
                FileUpload file = _service.Get(id);
                if(file == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<FileUpload> { Item = file };
                }
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(FileAddRequest model)
        {
            ObjectResult result;
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result  = StatusCode(500, response);
            }
            return result;
        }

        [HttpGet("paged")]
        public ActionResult<ItemResponse<Paged<FileUpload>>> GetPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response;
            
            try
            {
                Paged<FileUpload> pagedResult = _service.GetPaginated(pageIndex, pageSize);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<FileUpload>> { Item = pagedResult };
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
        public ActionResult<ItemResponse<Paged<FileUpload>>> SearchByCreated(int pageIndex, int pageSize, int createdBy)
        {
            int code = 200;
            BaseResponse response;

            try
            {
                Paged<FileUpload> pagedResult = _service.SearchByCreated(pageIndex, pageSize, createdBy);
                if (pagedResult == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<FileUpload>> { Item = pagedResult };
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

        [HttpPost("awsupload")]
        public ActionResult<ItemsResponse<FileBase>> AWSFileUpload(List<IFormFile> files)
        {
            int code = 200;
            BaseResponse response = null;
      
            try
            {
                Task<List<string>> url = _service.UploadAsync(files);
                int userId = _authService.GetCurrentUserId();
                List < FileBase > responseList= null;
                for (int i = 0; i < url.Result.Count; i++)
                {
                    FileAddRequest model = new FileAddRequest() { Url = url.Result[i], FileTypeId = 3 };

                    int id = _service.Add(model, userId);

                    FileBase aFile = new FileBase() { Id = id, Url = url.Result[i] };
                    if (responseList == null)
                    {
                        responseList = new List<FileBase>();
                    }
                    responseList.Add(aFile);
                }
                response = new ItemsResponse<FileBase> { Items = responseList };

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
        //[HttpPost("awsuploads")]
        //public ActionResult<ItemResponse<FileBase>> AWSFilesUpload(IFormFile file)
        //{
        //    int code = 200;
        //    BaseResponse response = null;

        //    try
        //    {
        //        Task<string> url = _service.UploadDirectoryAsync(file);
        //        int userId = _authService.GetCurrentUserId();
        //        FileAddRequest model = new FileAddRequest() { Url = url.Result, FileTypeId = 3 };

        //        int id = _service.Add(model, userId);

        //        FileBase aFile = new FileBase() { Id = id, Url = url.Result };

        //        response = new ItemResponse<FileBase> { Item = aFile };

        //    }
        //    catch (Exception ex)
        //    {
        //        code = 500;
        //        response = new ErrorResponse(ex.Message);
        //    }
        //    return StatusCode(code, response);
        //}

    }
}