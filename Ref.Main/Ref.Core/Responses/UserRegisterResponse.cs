namespace Ref.Core.Responses
{
    public class UserRegisterResponse
    {
        public bool Succeed => string.IsNullOrWhiteSpace(Message);
        public string Message { get; set; }

        protected UserRegisterResponse() { }

        public static UserRegisterResponse Success()
            => new UserRegisterResponse();

        public static UserRegisterResponse Error(string message)
            => new UserRegisterResponse { Message = message };
    }
}