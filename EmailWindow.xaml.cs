﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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

namespace NetworkProgrammingP12
{
    /// <summary>
    /// Interaction logic for EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        public EmailWindow()
        {
            InitializeComponent();
        }

        private SmtpClient? GetSmtpClient()
        {
            #region get and check config
            String? host = App.GetConfiguration("smtp:host");
            if(host == null)
            {
                MessageBox.Show("Error getting host");
                return null;
            }
            String? portString = App.GetConfiguration("smtp:port");
            if (portString == null)
            {
                MessageBox.Show("Error getting port");
                return null;
            }
            int port;
            try { port = int.Parse(portString); }
            catch {
                MessageBox.Show("Error parsing port");
                return null;
            }
            String? email = App.GetConfiguration("smtp:email");
            if (email == null)
            {
                MessageBox.Show("Error getting email");
                return null;
            }
            String? password = App.GetConfiguration("smtp:password");
            if (password == null)
            {
                MessageBox.Show("Error getting password");
                return null;
            }
            String? sslString = App.GetConfiguration("smtp:ssl");
            if (sslString == null)
            {
                MessageBox.Show("Error getting ssl");
                return null;
            }
            bool ssl;
            try { ssl = bool.Parse(sslString); }
            catch
            {
                MessageBox.Show("Error parsing ssl");
                return null;
            }
            #endregion

            if( ! textBoxTo.Text.Contains('@'))
            {
                MessageBox.Show("Введіть правильну адресу");
                return null;
            }
            return new(host, port)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(email, password)
            };
        }

        private void SendButton1_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient? smtpClient = GetSmtpClient();
            if (smtpClient == null) { return; }
            smtpClient.Send(
                App.GetConfiguration("smtp:email")!,  // From
                textBoxTo.Text,
                textBoxSubject.Text,
                textBoxMessage.Text
            );
            MessageBox.Show("Надіслано");
        }

        private void SendButton2_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient? smtpClient = GetSmtpClient();
            if (smtpClient == null) { return; }

            MailMessage mailMessage = new(
                App.GetConfiguration("smtp:email")!,
                textBoxTo.Text, 
                textBoxSubject.Text,
                textBoxHtml.Text)
            {
                IsBodyHtml = true,
            };
            ContentType pngType = new("image/png");
            mailMessage.Attachments.Add(
                new Attachment("coin25.png", pngType));

            ContentType mp3Type = new("audio/mpeg");
            mailMessage.Attachments.Add(
                new Attachment("Jump_01.mp3", mp3Type));

            smtpClient.Send(mailMessage);
            MessageBox.Show("Надіслано");
        }
    }
}
/* Д.З. Реалізувати надсилання електронного листа із 
 * HTML контентом, який включає код підтвердження пошти
 * (взяти довільний код), а також вкладення файлу
 * privacy.txt з політикою сайту (взяти довільну)
 * із зазначенням правильного MIME-типу
 * ** також додати файл privacy.doc з тим самим вмістом
 */