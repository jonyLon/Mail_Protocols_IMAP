using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mail_Protocols_IMAP
{
    /// <summary>
    /// Interaction logic for NewMessageWindow.xaml
    /// </summary>
    public partial class NewMessageWindow : Window
    {
        private EmailClient _emailClient;
        private MimeMessage _replyMessage;

        public NewMessageWindow(EmailClient emailClient, MimeMessage replyMessage = null)
        {
            InitializeComponent();
            _emailClient = emailClient;
            _replyMessage = replyMessage;

            if (_replyMessage != null)
            {
                ToTextBox.Text = _replyMessage.From.ToString();
                SubjectTextBox.Text = "Re: " + _replyMessage.Subject;
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var to = ToTextBox.Text;
            var subject = SubjectTextBox.Text;
            var body = BodyTextBox.Text;

            await _emailClient.SendMessageAsync(to, subject, body);

            MessageBox.Show("Message sent successfully!");
            Close();
        }
    }
}
