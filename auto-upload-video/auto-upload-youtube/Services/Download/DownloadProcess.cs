using API.Constant;

namespace auto_upload_youtube.Services.Download
{

    public class DownloadProcessEventArgs : EventArgs
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }

    public class DownloadProcess
    {
        // Private
        private string _imageThumbURL;
        private string _videoURL;

        public event EventHandler<DownloadProcessEventArgs> DownloadProcessEvent;
        private GoogleDriverAPIService _downloadByApiProcess;
        private long _ggDriverFileSize;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        // Public 
        public string ImageThumbFilePath { get; set; }
        public string VideoFilePath { get; set; }

        public DownloadProcess(string imageThumbURL, string videoURL)
        {
            _imageThumbURL = imageThumbURL;
            _videoURL = videoURL;
        }

        public async Task Run()
        {
            try
            {
                _downloadByApiProcess = new GoogleDriverAPIService();
                if (string.IsNullOrEmpty(_imageThumbURL) || string.IsNullOrEmpty(_videoURL))
                {
                    throw new Exception("Không tồn tại Link");
                }
                
                //Get file path folder
                var videoDir = Path.Combine(AppConstant.DataFolderName, "video");
                if (!Directory.Exists(videoDir))
                {
                    Directory.CreateDirectory(videoDir);
                }

                // Check disk free space
                _ggDriverFileSize = _downloadByApiProcess.GetFileSize(_videoURL);
                if (!IsEnoughFreeSpace(_ggDriverFileSize))
                {
                    string[] files = Directory.GetFiles(videoDir);
                    if (files.Length == 0)
                    {
                        throw new Exception("Dung lương ổ cứng không đủ!");
                    }
                    else
                    {
                        foreach (var file in files)
                        {
                            File.Delete(file);
                        }

                        Thread.Sleep(5000);
                        _ggDriverFileSize = _downloadByApiProcess.GetFileSize(_videoURL);
                        if (!IsEnoughFreeSpace(_ggDriverFileSize))
                        {
                            throw new Exception("Dung lương ổ cứng không đủ!");
                        }
                    }
                }

                // Download
                
                //var type = Type == 1 ? ".mp4" : ".jpg";
                //_downloadedFilePath = Path.Combine(videoDir, $"{Guid.NewGuid().ToString()}{type}");
                ImageThumbFilePath = Path.Combine(videoDir, $"{Guid.NewGuid().ToString()}.jpg");
                VideoFilePath = Path.Combine(videoDir, $"{Guid.NewGuid().ToString()}.mp4");

                _downloadByApiProcess.DownloadProcessChange += _downloadByApiProcess_DownloadProcessChange;
                ImageThumbFilePath = _downloadByApiProcess.DownloadFile(_imageThumbURL, ImageThumbFilePath, _tokenSource.Token).GetAwaiter().GetResult();
                VideoFilePath = _downloadByApiProcess.DownloadFile(_videoURL, VideoFilePath, _tokenSource.Token).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải file: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                _tokenSource.Cancel();
                try
                {
                    File.Delete(ImageThumbFilePath);
                    File.Delete(VideoFilePath);
                }
                catch (Exception)
                {
                    // do nothing
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi dừng download: {ex.Message}");
            }
        }

        public string GetFilePath(string nameFile)
        {
            return Path.GetFullPath(nameFile);
        }

        //private void _downloadProcess_DownloadProgressChanged(object sender, GoogleDriverDownloadProcess.DownloadProgress progress)
        //{
        //    Message = $"Đang tải file từ google drive: {progress.ProgressPercentage}%";
        //    Console.WriteLine(Message);
        //}

        //private void _downloadProcess_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    try
        //    {
        //        long fileSize = 0;
        //        try
        //        {
        //            fileSize = (new FileInfo(_downloadedFilePath)).Length;
        //        }
        //        catch
        //        {
        //        }
        //        // Check File size after download complete
        //        if (Math.Ceiling((double)fileSize * 100 / _ggDriverFileSize) < 90)
        //        {
        //            try
        //            {
        //                File.Delete(_downloadedFilePath);
        //            }
        //            catch (Exception)
        //            {
        //            }
        //            _downloadByApiProcess.DownloadProcessChange += _downloadByApiProcess_DownloadProcessChange;
        //            _downloadByApiProcess.DownloadFile(GoogleDriveLink, _downloadedFilePath, _tokenSource.Token).GetAwaiter().GetResult();

        //            try
        //            {
        //                fileSize = (new FileInfo(_downloadedFilePath)).Length;
        //            }
        //            catch (Exception)
        //            {
        //            }
                   
        //            if (Math.Ceiling((double)fileSize * 100 / _ggDriverFileSize) < 90)
        //            {
        //                Message = "Tải file từ google drive lỗi";
        //                DownloadProcessEventArgs _d = new DownloadProcessEventArgs();
        //                _d.Message = Message;
        //                _d.IsSuccessful = false;
        //                OnDownloadProcessCompleted(_d);
        //                return;
        //            }
        //        }

        //        DownloadProcessEventArgs _data = new DownloadProcessEventArgs();
        //        _data.Message = "Đã tải xong";
        //        _data.IsSuccessful = true;
        //        OnDownloadProcessCompleted(_data);
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = $"Lỗi khi download: {ex.Message}";
        //    }
        //}

        private void _downloadByApiProcess_DownloadProcessChange(Google.Apis.Download.IDownloadProgress obj)
        {
            switch (obj.Status)
            {
                case Google.Apis.Download.DownloadStatus.NotStarted:
                    break;

                case Google.Apis.Download.DownloadStatus.Downloading:
                    var percentage = Math.Floor((double)obj.BytesDownloaded * 100 / _ggDriverFileSize);
                    UpdateMessage($"Đang tải file từ google drive: {percentage}%", false);
                    break;

                case Google.Apis.Download.DownloadStatus.Completed:
                    UpdateMessage($"Đang tải file từ google drive: 100%", true);
                    break;

                case Google.Apis.Download.DownloadStatus.Failed:
                    UpdateMessage("Tải file từ google drive lỗi", false);
                    break;
                default:
                    break;
            }
        }

        private bool IsEnoughFreeSpace(long fileSize)
        {
            var currentDrive = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location);
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.Name == currentDrive)
                {
                    var total = drive.TotalSize;
                    var free = drive.TotalFreeSpace;
                    if (free < fileSize)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdateMessage(string message, bool isSucessful)
        {
            try
            {
                DownloadProcessEventArgs _data = new DownloadProcessEventArgs();
                _data.Message = message;
                _data.IsSuccessful = isSucessful;
                OnDownloadProcessCompleted(_data);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi UpdateMessage: {ex.Message}");
            }
        }

        protected virtual void OnDownloadProcessCompleted(DownloadProcessEventArgs e)
        {
            EventHandler<DownloadProcessEventArgs> handler = DownloadProcessEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
