using auto_upload_youtube.Services.RandomUpload;

namespace auto_upload_youtube.Services.RandomUpload
{
    public class RandomUploadJobOutput
    {
            //public string UploadVideoPercent { get; set; }
        public string Message { get; set; }
        public TaskStatus TaskStatus { get; set; }
        
    }

    public enum TaskStatus
    {
        Running,
        Completed
    }
}
