namespace TermixListing.API.Core.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key) : base($"{name} with id ({key}) Was Not Found.")
        {

        }
    }
}
