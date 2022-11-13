namespace auto_upload_youtube.Services.RequestData
{
    public class RequestLinkGoogleDriveModel
    {
        public int actionState { get; set; }
        public string message { get; set; }
        public Datas data { get; set; }
    }
    public class Datas
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int itemCount { get; set; }
        public int pageCount { get; set; }
        public Itemss[] items { get; set; }
    }

    public class Itemss
    {
        public string googleDriveFileId { get; set; }
        public string googleDriveThumbFileId { get; set; }
    }
}