namespace Kompetenzgipfel.Exceptions;

public class TopicNotFoundException(string id) : Exception("SupportTask with id " + id + " not found")
{
}