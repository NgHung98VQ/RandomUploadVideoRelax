namespace auto_upload_youtube.Services.Upload
{
    public class UploadJobOutput
    {
        //public string UploadVideoPercent { get; set; }
        public string Message { get; set;}
        public TaskStatus TaskStatus { get; set; }
        public bool IsSuccessfully { get; set; }
    }

    public enum TaskStatus
    {
        Running,
        Completed
    }
}
