using System.Text;

namespace Ref.Core.Services
{
    public class EmailUI
    {
        public EmailUI(string token)
        {
            Token = token;
        }

        public string Token { get; private set; }

        public EmailMessage Prepare()
        {
            var sbRaw = new StringBuilder();
            var sbHtml = new StringBuilder();

            var sbFooter =
                $"<br><br>-----" +
                $"<br><strong>Udanych zakupów życzy zespół:</strong> <a href=\"https://pewnemieszkanie.pl/\">PewneMieszkanie.pl</a>" +
                $"<br>Zapraszamy na nasz <a href=\"https://www.facebook.com/PewneMieszkanie/\">Facebook</a>.";

            sbRaw.AppendLine($"Dziękujemy za rejestracje!");
            sbHtml.AppendLine($"<h3>Dziękujemy za rejestracje!</h3>");

            sbRaw.AppendLine("Przez następne 24 godziny możesz testować nasz system bez żadnych opłat.");
            sbHtml.AppendLine("<p>Przez następne <strong>24 godziny</strong> możesz testować nasz system bez żadnych opłat.</p>");

            sbHtml.AppendLine($"<br><strong><a href=\"https://app.pewnemieszkanie.pl/index.html?guid={Token}\">Twoje konto</a></strong><br>");

            sbHtml.AppendLine(sbFooter);

            return new EmailMessage(
                $"[PewneMieszkanie.pl] - Rejestracja",
                sbRaw.ToString(),
                sbHtml.ToString());
        }

        public bool CanBeSend => !string.IsNullOrWhiteSpace(Token);
    }

    public class EmailMessage
    {
        public EmailMessage(string title, string message)
        {
            Title = title;
            RawMessage = message;
        }

        public EmailMessage(string title, string message, string htmlMessage)
        {
            Title = title;
            RawMessage = message;
            HtmlMessage = htmlMessage;
        }

        public string Title { get; private set; }
        public string RawMessage { get; private set; }
        public string HtmlMessage { get; set; }
    }
}