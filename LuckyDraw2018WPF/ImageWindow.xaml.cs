using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LuckyDraw2018WPF
{
    /// <summary>
    /// ImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();
        }
        private string _title;
        private string _imgSrc;
        public ImageWindow(string title, string imgSrc) : base()
        {
            _title = title;
            _imgSrc = imgSrc;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = _title;
            _imgSrc = System.IO.Path.Combine(Directory.GetCurrentDirectory(), _imgSrc);
            var bitmapImage = new BitmapImage(new Uri(_imgSrc));
            imgPrize.Source = bitmapImage;
        }
    }
}
