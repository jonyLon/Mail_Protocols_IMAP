using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Mail_Protocols_IMAP
{
    public class EmailClient
    {
        private readonly string _email;
        private readonly string _password;

        public EmailClient(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task<List<string>> GetFoldersAsync()
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                await client.AuthenticateAsync(_email, _password);

                var folders = new List<string>();
                foreach (var folder in client.GetFolders(client.PersonalNamespaces[0]))
                {
                    folders.Add(folder.FullName);
                }

                await client.DisconnectAsync(true);
                return folders;
            }
        }

        public async Task<List<MimeMessage>> GetMessagesAsync(string folderName)
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                await client.AuthenticateAsync(_email, _password);

                var inbox = client.GetFolder(folderName);
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                var messages = new List<MimeMessage>();
                for (int i = 0; i < 10; i++)
                {
                    var message = await inbox.GetMessageAsync(i);
                    messages.Add(message);
                }

                await client.DisconnectAsync(true);
                return messages;
            }
        }

        public async Task SendMessageAsync(string to, string subject, string body, List<string> attachments = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_email, _email));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment);
                }
            }
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(_email, _password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                using (var client = new ImapClient())
                {
                    await client.ConnectAsync("imap.gmail.com", 993, true);
                    await client.AuthenticateAsync(_email, _password);
                    await client.DisconnectAsync(true);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
