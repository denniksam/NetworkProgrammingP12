﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkProgrammingP12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // MessageBox.Show(  );
        }

        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            new ServerWindow().Show();
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            new ClientWindow().Show();
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            new EmailWindow().ShowDialog();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().ShowDialog();
        }

        private void HttpButton_Click(object sender, RoutedEventArgs e)
        {
            new HttpWindow().ShowDialog();
        }

        private void CryptoButton_Click(object sender, RoutedEventArgs e)
        {
            new CryptoWindow().ShowDialog();
        }
    }
}
