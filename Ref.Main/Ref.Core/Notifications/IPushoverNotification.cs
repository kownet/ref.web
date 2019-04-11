using System.Collections.Specialized;
using System.Net;

namespace Ref.Core.Notifications
{
    public interface IPushOverNotification
    {
        void Send(string title, string message);
    }

    public class PushOverNotification : IPushOverNotification
    {
        public PushOverNotification(string token, string recipients, string endpoint)
        {
            Token = token;
            Recipients = recipients;
            Endpoint = endpoint;
        }

        public string Token { get; private set; }
        public string Recipients { get; private set; }
        public string Endpoint { get; private set; }

        public void Send(string title, string message)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(message))
            {
                var parameters = new NameValueCollection
                {
                    { "token", Token },
                    { "user", Recipients },
                    { "message", message },
                    { "title", title },
                    { "html", "1" }
                };

                using (var client = new WebClient())
                {
                    client.UploadValues(Endpoint, parameters);
                }
            }
        }
    }
}