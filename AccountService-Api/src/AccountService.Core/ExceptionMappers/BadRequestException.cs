namespace AccountService.Core.ExceptionMappers
{
    public class BadRequestException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public BadRequestException(IEnumerable<string> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
