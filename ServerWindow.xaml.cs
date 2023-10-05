using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private Socket? listenSocket;  // "слухаючий" сокет - очікує запитів
        private IPEndPoint? endPoint;  // точка, як він "слухає" 

        public ServerWindow()
        {
            InitializeComponent();
        }

        private void SwitchServer_Click(object sender, RoutedEventArgs e)
        {
            if(listenSocket == null)  // сервер OFF, включаємо
            {
                try
                {
                    // парсимо хост - визначаємо номер-адрес вузла з текстового вигляду
                    IPAddress ip = IPAddress.Parse(HostTextBox.Text);
                    // те ж саме з портом
                    int port = Convert.ToInt32(PortTextBox.Text);
                    // збираємо хост + порт в endpoint
                    endPoint = new(ip, port);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(
                        "Неправильні параметри конфігурації: " 
                        + ex.Message);
                    return;
                }
                listenSocket = new Socket(
                    AddressFamily.InterNetwork,  // IPv4
                    SocketType.Stream,           // Двосторонній (читає та пише)
                    ProtocolType.Tcp);           // Протокол - TCP
                // Стартуємо сервер. Оскільки процес слухання довгий (нескінченний)
                // запускаємо його в окремому потоці
                new Thread(StartServer).Start();
            }
            else  // сервер ON, виключаємо
            {
                // сервер зупинити, якщо він в очікуванні, дуже складно
                listenSocket.Close();  // створюємо конфлікт - закриваємо сокет
                // це призведе до exception у потоці сервера 
            }
        }
        /* Д.З. Реалізувати реакцію UI на перемикання стану сервера -
         * змінювати надпис та колір статусу, надпис (опціонально-колір)
         * кнопки запуску/зупинки серверу.
         */

        private void StartServer()
        {
            if(listenSocket == null || endPoint == null)
            {
                MessageBox.Show("Спроба запуску без ініціалізації даних ");
                return;
            }

            try
            {
                listenSocket.Bind(endPoint);  // якщо порт зайнятий, то буде виключення
                listenSocket.Listen(10);      // 10 запитів - максимальна черга (іншим-відмова)
                Dispatcher.Invoke(() => ServerLog.Text += "Сервер запущен\n");

                byte[] buffer = new byte[1024];  // буфер прийому даних
                while(true)  // нескінченний процес слухання - постійна робота сервера
                {
                    // очікування запиту, саме ця інструкція блокує потік до надходження запиту
                    Socket socket = listenSocket.Accept();

                    // цей код виконується коли сервер отримав запит
                    StringBuilder stringBuilder = new();
                    do
                    {
                        int n = socket.Receive(buffer);   // отримаємо пакет, зберігаємо 
                        // у буфері. n - кількість реально отриманих байт
                        // Декодуємо одержані байти у рядок та додаємо до stringBuilder
                        stringBuilder.Append(
                            Encoding.UTF8.GetString(buffer, 0, n));  // TODO: визначити кодування з налаштувань

                    } while (socket.Available > 0);   // повторюємо цикл доки у сокеті є дані
                    String str = stringBuilder.ToString();
                    Dispatcher.Invoke(() => ServerLog.Text += $"{DateTime.Now} {str}\n");
                }
            }
            catch (Exception ex)
            {
                // Імовірніше за все сервер зупинився кнопкою з UI
                // У будь-якому разі роботу припинено, зануляємо посилання
                listenSocket = null;
                Dispatcher.Invoke(() => ServerLog.Text += "Сервер зупинено\n");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // !! Не забувати зупиняти сервер при закритті вікна
            listenSocket?.Close();
        }
    }
}
