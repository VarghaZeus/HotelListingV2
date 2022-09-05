namespace TermixListing.API.Core.Exceptions
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(object key) : base($"Invalid Record Id: ({key}).")
        {

        }
    }
}
