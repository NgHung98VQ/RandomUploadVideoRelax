namespace API.Dto
{
    public class ResponseMessage
    {
        public MessageType MessageType { get; set; }
        public string Message { get; set; }

        public ResponseMessage(MessageType type, string message)
        {
            MessageType = type;
            Message = message;
        }
    }

    public enum MessageType
    {
        Success,
        Info,
        Warning,
        Error
    }
}
