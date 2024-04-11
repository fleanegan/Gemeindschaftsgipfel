namespace Gemeinschaftsgipfel.Exceptions;

public class SupportPromiseImpossibleException(string id)
    : Exception("The SupportPromise for SupportTask with id " + id + " failed")
{
}