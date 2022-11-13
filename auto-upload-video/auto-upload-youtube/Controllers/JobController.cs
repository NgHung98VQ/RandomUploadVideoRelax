using API.Constant;
using API.Dto;
using BaseService.Shared;
using BaseService.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace API.Controllers
{
    [Route("api/job")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private Logger _logger = new Logger(AppConstant.LogFileName);

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddJob([FromBody]AddJobDto dto)
        {
            try
            {
                // validate dto
                if (dto == null)
                {
                    return BadRequest(new ResponseMessage(MessageType.Error, "Dữ liệu đầu vào không hợp lệ"));
                }
                if (string.IsNullOrEmpty(dto.JobId?.Trim()))
                {
                    return BadRequest(new ResponseMessage(MessageType.Error, "Dữ liệu đầu vào không hợp lệ - ID"));
                }
                if (dto.TimeoutMilisecond < 0)
                {
                    return BadRequest(new ResponseMessage(MessageType.Error, "Dữ liệu đầu vào không hợp lệ - timeout"));
                }

                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == dto.ServiceName);
                if (service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }

                var result = await service.AddJob(dto.JobId, dto.Input, TimeSpan.FromMilliseconds(dto.TimeoutMilisecond));

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

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetJob(string serviceName, string jobId)
        {
            try
            {
                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == serviceName);
                if (service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }
                
                var job = await service.GetJob(jobId);
                if(job == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "job không tồn tại"));
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> FilterJobs(string serviceName, JobState? state = null, JobResult? result = null, string message = null)
        {
            try
            {
                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == serviceName);
                if (service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }

                var jobs = await service.GetJobs(state, result, message);
                if (jobs == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "job không tồn tại"));
                }

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("stop")]
        public async Task<IActionResult> StopJob(string serviceName, string jobId)
        {
            try
            {
                var service = ServiceManager.Services.FirstOrDefault(s => s.ServiceName == serviceName);
                if (service == null)
                {
                    return NotFound(new ResponseMessage(MessageType.Error, "service không tồn tại"));
                }

                var result = await service.StopJob(jobId);

                if (result.IsSuccess)
                {
                    return Ok(new ResponseMessage(MessageType.Success, ""));
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
