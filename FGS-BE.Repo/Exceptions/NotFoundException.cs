namespace FGS_BE.Repo.Exceptions;

public class NotFoundException : Exception
{

    public NotFoundException() : base("We couldn't find what you were looking for.") { }

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

    public NotFoundException(string name, object key) : base($"Entity {name} ({key}) was not found.") { }

}