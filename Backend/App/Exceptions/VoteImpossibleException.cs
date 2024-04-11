namespace Gemeinschaftsgipfel.Exceptions;

public class VoteImpossibleException(string id) : Exception("The Vote for Topic with id " + id + " failed")
{
}