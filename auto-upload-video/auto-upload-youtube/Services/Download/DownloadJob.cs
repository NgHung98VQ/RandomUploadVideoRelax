using BaseService.MemoryService;
using BaseService.Shared.Models;
using System.Diagnostics;

namespace auto_upload_youtube.Services.Download
{
    public class DownloadJobInput
    { 
        public string GoogleDriveLink { get; set; }
        public string GoogleDriveImageLink { get; set; }
    }

    public class DownloadJobOutput
    {
        public string Message { get; set; }
    }

    public class DownloadJob : MemoryJob<DownloadJobInput, DownloadJobOutput>
    {
        protected override async Task<(JobResult Result, string Message)> ExecuteProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                // init
                Output = new DownloadJobOutput();

                // execute job
                var imageThumbGGDriveURL = Input.GoogleDriveImageLink;
                var videoGGDriveURL = Input.GoogleDriveLink;
                var downloadProcess = new DownloadProcess(imageThumbGGDriveURL, videoGGDriveURL);
                await downloadProcess.Run();

                return (JobResult.Success, "");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                return (JobResult.Error, ex.Message);
            }
            finally
            {
                // release resources
            }
        }
    }
}
