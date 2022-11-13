using API.Constant;
using API.Dto;
using BaseService.Shared;
using BaseService.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace API.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private Logger _logger = new Logger(AppConstant.LogFileName);

        [HttpGet]
        public async Task<IActionResult> GetService(string name)
        {
            try
            {
                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == name);
                if(service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }
                var status = await service.GetServiceStatus();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("start")]
        public async Task<IActionResult> StartService(string name)
        {
            try
            {
                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == name);
                if (service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }
                var result = await service.StartService();

                if (result.IsSuccess)
                {
                    return Ok(new ResponseMessage(MessageType.Success, result.Message));
                }
                else
                {
                    return Ok(new ResponseMessage(MessageType.Error, result.Message));
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("stop")]
        public async Task<IActionResult> StopService(string name)
        {
            try
            {
                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == name);
                if (service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }
                var result = await service.StopService();

                if (result.IsSuccess)
                {
                    return Ok(new ResponseMessage(MessageType.Success, result.Message));
                }
                else
                {
                    return Ok(new ResponseMessage(MessageType.Error, result.Message));
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
