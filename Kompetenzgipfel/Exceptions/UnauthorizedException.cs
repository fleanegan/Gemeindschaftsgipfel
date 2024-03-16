namespace Kompetenzgipfel.Exceptions;

public class UnauthorizedException(string userName)
    : Exception("User with user name: " + userName + " is not authorized to add new support topics")
{
}