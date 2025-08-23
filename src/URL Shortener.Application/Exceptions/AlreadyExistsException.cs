
namespace URL_Shortener.Application.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message)
            : base(message) { }
    }
}