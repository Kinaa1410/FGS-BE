namespace FGS_BE.Repo.Exceptions;
public class BadRequestException : Exception
{
    //public BadRequestException() : base(Resource.BadRequest) { }
    public BadRequestException() : base("Invalid request. Please provide valid data.") { }
    public BadRequestException(string message) : base(message) { }
    public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
}
