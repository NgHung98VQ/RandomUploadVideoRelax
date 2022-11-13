using auto_upload_youtube.Services.Download;
using BaseService.MemoryService;
using BaseService.Shared.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace auto_upload_youtube.Services.Upload
{
    public class UploadJob : MemoryJob<UploadJobInput, UploadJobOutput>
    {
        protected override async Task<(JobResult Result, string Message)> ExecuteProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // init
                Output = new UploadJobOutput();
                Output.TaskStatus = TaskStatus.Running;
                Output.IsSuccessfully = false;
                Output.Message = "Đang bắt đầu ...";

                // Validate input
                if (string.IsNullOrEmpty(Input.ImageThumbGoogleDriveURL) || string.IsNullOrEmpty(Input.VideoGoogleDriveURl))
                {
                    throw new Exception("Đầu vào không hợp lệ");
                }

                if (string.IsNullOrEmpty(Input.ProfilePath))
                {
                    throw new Exception("Thiếu profile path");
                }

                // Download Video
                Output.Message = "Đang download video ...";
                var imageThumbGGDriveURL = Input.ImageThumbGoogleDriveURL;
                var videoGGDriveURL = Input.VideoGoogleDriveURl;
                var downloadProcess = new DownloadProcess(imageThumbGGDriveURL, videoGGDriveURL);
                await downloadProcess.Run();

                Input.VideoFilePath = Path.GetFullPath(downloadProcess.VideoFilePath);
                Input.ImageFilePath = Path.GetFullPath(downloadProcess.ImageThumbFilePath);

                // Upload Youtube
                Output.Message = "Đang Upload Youtube ...";
                YoutubeAutoUploadProcess uploadYoutubeProcess = new YoutubeAutoUploadProcess(Input);
                await uploadYoutubeProcess.Run();

                Output.Message = "Thành công !";
                Output.Message = uploadYoutubeProcess.Message;
                Output.TaskStatus = TaskStatus.Completed;
                Output.IsSuccessfully = uploadYoutubeProcess.IsSuccessful;

                return (JobResult.Success, ProcessingMessage);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, ex.Message, new StackTrace(ex, true).GetFrames().Last(), ex);
                ProcessingMessage = $"Lỗi: {ex.Message}";
                Output.TaskStatus = TaskStatus.Completed;
                Output.IsSuccessfully = false;
                Output.Message = $"Lỗi khi upload youtube studio: {ex.Message}";
                return (JobResult.Error, $"Lỗi: {ex.Message}");
            }
            finally
            {
                // release resources
            }
        }
    }
}

