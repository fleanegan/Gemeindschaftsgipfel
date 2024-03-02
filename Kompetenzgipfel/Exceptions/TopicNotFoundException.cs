namespace Kompetenzgipfel.Exceptions;

public class TopicNotFoundException(string id) : Exception("Topic with id " + id + " not found")
{
}