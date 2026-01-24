using FGS_BE.Repo.Resources;

namespace FGS_BE.Repo.Exceptions;
public class BadRequestException : Exception
{
    public BadRequestException() : base(Resource.BadRequest) { }
    public BadRequestException(string message) : base(message) { }
    public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
}
