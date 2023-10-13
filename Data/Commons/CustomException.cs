namespace FindingPets.Data.Commons
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException() : base() { }
        public RecordNotFoundException(string message) : base(message) { }
        public RecordNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ResourceAlreadyExistsException : Exception
    {
        public ResourceAlreadyExistsException() : base() { }
        public ResourceAlreadyExistsException(string message) : base(message) { }
        public ResourceAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
