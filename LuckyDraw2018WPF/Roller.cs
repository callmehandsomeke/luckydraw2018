using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace LuckyDraw2018WPF
{
    public class Roller
    {
        private List<ContentControl> _tables;
        private List<ContentControl> _seats;
        private List<Image> _tableImages;
        private List<Image> _seatImages;
        private Grid _parentGrid;
        private List<dynamic> _source;
        private const string GIF_BIG_SUFFIX = "_big";
        private const string GIF_SMALL_SUFFIX = "_small";
        private int _prize = 0;
        private List<dynamic> _selectedValues = new List<dynamic>();

        public List<dynamic> SelectedValues
        {
            get
            {
                return _selectedValues;
            }
        }

        public Roller(Grid parentGrid, List<ContentControl> tables, List<ContentControl> seats, List<dynamic> source, int prize)
        {
            _tables = tables;
            _seats = seats;
            _parentGrid = parentGrid;
            _source = source;
            _prize = prize;
        }

        public void Start()
        {
            _tableImages = CreateRollerImages(_tables);
            _seatImages = CreateRollerImages(_seats);
            Random random = new Random();
            for (int i = 0; i < _seats.Count; i++)
            {
                if (_source.Count == 0)
                {
                    break;
                }
                int r = random.Next(_source.Count);
                var value = _source[r];
                //remove the value already been selected
                _source.RemoveAt(r);
                _selectedValues.Add(value);
            }
        }

        private List<Image> CreateRollerImages(List<ContentControl> controls)
        {
            if (controls == null)
            {
                return null;
            }
            Image imageControl;
            List<Image> images = new List<Image>();
            Random random = new Random();
            int count = 1;
            foreach (var control in controls)
            {
                imageControl = new Image();
                imageControl.Name = control.Name + "_image";
                imageControl.Width = control.Width;
                imageControl.Height = control.Height;
                imageControl.Stretch = System.Windows.Media.Stretch.None;
                imageControl.HorizontalAlignment = control.HorizontalAlignment;
                imageControl.VerticalAlignment = control.VerticalAlignment;
                var r = random.Next(1, 4);
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(GetPathWithNumber(r, controls.Count == 1 || _prize == 4));
                image.EndInit();
                ImageBehavior.SetAnimatedSource(imageControl, image);
                _parentGrid.Children.Add(imageControl);
                Grid.SetColumn(imageControl, Grid.GetColumn(control));
                Grid.SetRow(imageControl, Grid.GetRow(control));
                Grid.SetColumnSpan(imageControl, Grid.GetColumnSpan(control));
                images.Add(imageControl);
                imageControl.Visibility = count > _source.Count ? Visibility.Hidden : Visibility.Visible;
                control.Visibility = Visibility.Hidden;
                count++;
            }
            return images;
        }

        private string GetPathWithNumber(int number, bool useBig)
        {
            string suffix = useBig ? GIF_BIG_SUFFIX : GIF_SMALL_SUFFIX;
            return Path.Combine(Directory.GetCurrentDirectory(), $"Images\\roller{suffix}{number}.gif");
        }

        public void StopTable()
        {
            for (int i = 0; i < _tables.Count; i++)
            {
                _tableImages[i].Visibility = Visibility.Hidden;
                _tables[i].Visibility = Visibility.Visible;
                _tables[i].Content = _selectedValues.Count > i ? _selectedValues[i].table : string.Empty;
            }
        }

        public void StopSeat()
        {
            for (int i = 0; i < _seats.Count; i++)
            {
                _seatImages[i].Visibility = Visibility.Hidden;
                _seats[i].Visibility = Visibility.Visible;
                _seats[i].Content = _selectedValues.Count > i ? _selectedValues[i].seat : string.Empty;
            }
        }
    }
}
