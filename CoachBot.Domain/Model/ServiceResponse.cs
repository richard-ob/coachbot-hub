namespace CoachBot.Domain.Model
{
    public enum ServiceResponseStatus
    {
        Success,
        NegativeSuccess,
        Failure,
        Info
    }

    public class ServiceResponse
    {

        public ServiceResponseStatus Status { get; set; }

        public string Message { get; set; }

        public ServiceResponse(ServiceResponseStatus status, string message)
        {
            Message = message;
            Status = status;
        }
    }
}
