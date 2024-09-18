namespace TreeApi.Errors
{
    public class SecureException : Exception
    {
        public SecureException(string message) : base(message) { }
    }
}
