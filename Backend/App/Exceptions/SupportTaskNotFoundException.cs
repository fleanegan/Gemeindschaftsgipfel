namespace Gemeinschaftsgipfel.Exceptions;

public class SupportTaskNotFoundException(string id) : Exception("Topic with id " + id + " not found")
{
}