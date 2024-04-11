namespace Gemeinschaftsgipfel.Exceptions;

public class UnauthorizedTopicModificationException(string id)
    : UnauthorizedAccessException("You are not allowed to modify the topic of id " + id + ".")
{
}