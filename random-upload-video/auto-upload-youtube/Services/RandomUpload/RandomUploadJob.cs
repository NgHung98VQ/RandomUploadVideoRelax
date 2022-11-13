using auto_upload_youtube.Services.RequestData;
using BaseService.MemoryService;
using BaseService.Shared.Models;
using System.Diagnostics;

namespace auto_upload_youtube.Services.RandomUpload
{
    public class RandomUploadJob : MemoryJob<RandomUploadJobInput, RandomUploadJobOutput>
    {
        protected override async Task<(JobResult Result, string Message)> ExecuteProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                // init
                Output = new RandomUploadJobOutput();

                if (string.IsNullOrEmpty(Input.UserId))
                {
                    throw new Exception("Thiếu UserId");
                }

                if (string.IsNullOrEmpty(Input.ProfilePath))
                {
                    throw new Exception("Thiếu profile path");
                }

                //Random upload video
                RandomProcess randomProcess = new RandomProcess(Input, Output);
                await randomProcess.RunRandomUpload();

                return (JobResult.Success, "");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return (JobResult.Error, "Lỗi không xác định");
            }
            finally
            {
                // release resources
            }
        }
    }
}
