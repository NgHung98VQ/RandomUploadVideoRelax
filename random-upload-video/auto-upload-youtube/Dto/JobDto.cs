namespace API.Dto
{
    public class AddJobDto
    {
        public string ServiceName { get; set; }

        public string JobId { get; set; }
        public Object Input { get; set; }
        public int TimeoutMilisecond { get; set; }
    }
}
