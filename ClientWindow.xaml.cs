using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
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
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private Random random = new();
        private IPEndPoint? endPoint;

        public ClientWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "User" + random.Next(100);
            MessageTextBox.Text = "Hello, all!";
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            String[] address = HostTextBox.Text.Split(':');
            try
            {
                endPoint = new(
                    IPAddress.Parse(address[0]),
                    Convert.ToInt32(address[1]));

                new Thread(SendMessage).Start(
                    new ClientRequest
                    {
                        Command = "Message",
                        Message = new()
                        {
                            Login = LoginTextBox.Text,
                            Text = MessageTextBox.Text
                        }
                    }                    
                );
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SendMessage(Object? arg)
        {
            var clientRequest = arg as ClientRequest;
            if(endPoint == null || clientRequest == null)
            {
                return;
            }
            Socket clientSocket = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(endPoint);   // клієнт "викликає" (сервер слухає)
                clientSocket.Send(
                    Encoding.UTF8.GetBytes(
                       JsonSerializer.Serialize(clientRequest)
                ));
                // Одержуємо відповідь сервера
                MemoryStream memoryStream = new();   // "ByteBuilder" - спосіб накопичити байти
                byte[] buffer = new byte[1024];
                do
                {
                    int n = clientSocket.Receive(buffer);
                    memoryStream.Write(buffer, 0, n);
                } while (clientSocket.Available > 0);
                String str = Encoding.UTF8.GetString(memoryStream.ToArray());

                ServerResponse? response = null;
                try { response = JsonSerializer.Deserialize<ServerResponse>(str); }
                catch { }
                if(response == null)
                {
                    str = "JSON Error in " + str;
                }
                else
                {
                    str = response.Status;
                }
                // Виводимо відповідь на лог
                Dispatcher.Invoke(() => ClientLog.Text += str + "\n");
                // Повідомляємо сервер про розрив сокету
                clientSocket.Shutdown(SocketShutdown.Both);
                // Звільняємо ресурс
                clientSocket.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
/* Д.З. Реалізувати відображення статусу серверної відповіді
 * на клієнті при надсиланні повідомлення
 * - при успішному статусі: зелений фон + "надіслано"
 * - при помилці: червоний фон + "помилка"
 * ** прибирати це повідомлення через 3 секунди після показу
 */
