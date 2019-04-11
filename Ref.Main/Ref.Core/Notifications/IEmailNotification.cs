using Newtonsoft.Json;
using Ref.Core.Extensions;
using Ref.Core.Notifications.Payloads;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Core.Notifications
{
    public interface IEmailNotification
    {
        EmailResponse Send(string title, string rawMessage, string htmlMessage, string[] recipients);
    }

    public class EmailNotification : IEmailNotification
    {
        public string ApiKey { get; private set; }
        public string Sender { get; private set; }
        public string ReplyTo { get; private set; }
        public string Host { get; private set; }

        public EmailNotification(string apiKey, string sender, string replyTo, string host)
        {
            ApiKey = apiKey;
            Sender = sender;
            ReplyTo = replyTo;
            Host = host;
        }

        public EmailResponse Send(string title, string rawMessage, string htmlMessage, string[] recipients)
        {
            if (!string.IsNullOrWhiteSpace(title) &&
                (!string.IsNullOrWhiteSpace(rawMessage) || !string.IsNullOrWhiteSpace(htmlMessage)) &&
                recipients.AnyAndNotNull())
            {
                var to = new List<string>(recipients);

                var payload = new EmailPayload
                {
                    ApiKey = ApiKey,
                    To = to,
                    Sender = Sender,
                    Subject = title,
                    Text = rawMessage,
                    Html = htmlMessage,
                    Headers = new List<EmailPayloadCustomHeader>
                    {
                        new EmailPayloadCustomHeader
                        {
                            Header = "Reply-To",
                            Value = ReplyTo
                        }
                    }
                };

                using (var request = new HttpRequestMessage(HttpMethod.Post, "email/send"))
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient() { BaseAddress = new Uri(Host) })
                    {
                        var response = Task.Run(() => httpClient.SendAsync(request)).Result;

                        var error = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

                        try
                        {
                            var status = response.EnsureSuccessStatusCode();
                        }
                        catch (Exception ex)
                        {
                            return new EmailResponse
                            {
                                Message = ex.Message
                            };
                        }
                    }
                }

                return new EmailResponse();
            }

            return new EmailResponse
            {
                Message = "Pusta wiadomość nie będzie wysłana"
            };
        }
    }

    public class EmailResponse
    {
        public string Message { get; set; }

        public bool IsSuccess => string.IsNullOrWhiteSpace(Message);
    }
}