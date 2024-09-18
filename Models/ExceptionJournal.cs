namespace TreeApi.Models
{
    public class ExceptionJournal
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string QueryParams { get; set; }
        public string BodyParams { get; set; }
        public string ExceptionType { get; set; }
    }
}
