using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for HttpWindow.xaml
    /// </summary>
    public partial class HttpWindow : Window
    {
        public HttpWindow()
        {
            InitializeComponent();
        }

        private async void get1Button_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync("https://itstep.org/");
            textBlock1.Text = "";
            textBlock1.Text += (int)response.StatusCode + 
                " " + response.ReasonPhrase + "\r\n";
            foreach (var header in response.Headers)
            {
                textBlock1.Text += $"{header.Key, -20}: "
                    + String.Join(',', header.Value).Ellipsis(30)
                    + "\r\n";
            }
            String body = await response.Content.ReadAsStringAsync();
            textBlock1.Text += $"\r\n{body}";
            /* Д.З. Додати до вікна HttpWindow дві кнопки, які
             * завантажують дані про курси валют в різних форматах (XML, JSON)
             * https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange
             * https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json
             * та виводять їх у вигляді тексту на інтерфейс вікна.
             * Додати скріншоти
             */
        }
    }

    public static class EllipsisExtensions
    {
        public static string Ellipsis(this string str, int maxLength)
        {
            return str.Length > maxLength 
                ? str[..(maxLength - 3)] + "..." 
                : str ;
        }
    }
}
