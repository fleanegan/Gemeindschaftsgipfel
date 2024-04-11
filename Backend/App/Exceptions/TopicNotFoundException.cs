namespace Gemeinschaftsgipfel.Exceptions;

public class TopicNotFoundException(string id) : Exception("SupportTask with id " + id + " not found")
{
}