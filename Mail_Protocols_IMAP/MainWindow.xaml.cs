using Mail_Protocols_IMAP;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Mail_Protocols_IMAP
{
    public partial class MainWindow : Window
    {
        private EmailClient _emailClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;
            _emailClient = new EmailClient(email, password);

            bool isAuthenticated = await _emailClient.AuthenticateAsync();
            if (isAuthenticated)
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var folders = await _emailClient.GetFoldersAsync();
                FoldersListBox.ItemsSource = folders;
            }
            else
            {
                MessageBox.Show("Login failed! Please check your email and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }




        }

        private async void FoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var folderName = FoldersListBox.SelectedItem as string;
            if (folderName != null)
            {
                var messages = await _emailClient.GetMessagesAsync(folderName);

                MessageBox.Show($"Number of messages in {folderName}: {messages.Count}");

                MessagesListBox.ItemsSource = messages;
                MessagesListBox.DisplayMemberPath = "Subject";
                MessageDetailsTextBlock.Text = "";
            }
        }

        private void MessagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var message = MessagesListBox.SelectedItem as MimeMessage;
            if (message != null)
            {
                MessageDetailsTextBlock.Text = $"From: {message.From}\nTo: {message.To}\nSubject: {message.Subject}\nDate: {message.Date}\n\n{message.TextBody}";
            }
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var message = MessagesListBox.SelectedItem as MimeMessage;
            if (message != null)
            {
                var replyWindow = new NewMessageWindow(_emailClient, message);
                replyWindow.Show();
            }
        }

        private void NewMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var newMessageWindow = new NewMessageWindow(_emailClient);
            newMessageWindow.Show();
        }
    }
}

