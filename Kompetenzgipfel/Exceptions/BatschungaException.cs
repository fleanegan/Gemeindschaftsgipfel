namespace Kompetenzgipfel.Exceptions;

//todo: replace with an Exception of the Identity Framework when internet is back
public class BatschungaException(string id) : Exception("The Vote for Topic with id " + id + " failed")
{
}