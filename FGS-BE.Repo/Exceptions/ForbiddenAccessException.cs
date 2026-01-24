using FGS_BE.Repo.Resources;

namespace FGS_BE.Repo.Exceptions;
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base(Resource.Forbidden) { }

    public ForbiddenAccessException(string message) : base(message) { }
}
