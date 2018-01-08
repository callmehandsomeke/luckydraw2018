using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LuckyDraw2018WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cmbNumbers.PreviewKeyDown += (o, e) =>
            {
                if (e.Key == Key.F4)
                    e.Handled = true;
                var relaunchedEvent = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key);
                relaunchedEvent.RoutedEvent = Keyboard.KeyDownEvent;
                relaunchedEvent.Source = e.OriginalSource;
                this.RaiseEvent(relaunchedEvent);
            };
        }

        private BusinessLogic _bll = new BusinessLogic();
        private bool _isStarted = false;
        private Roller _currentRoller;
        private PrizeType _currentPrize;
        private List<ContentControl> _currentTableList = new List<ContentControl>();
        private List<ContentControl> _currentSeatList = new List<ContentControl>();
        private List<ContentControl> _currentNameList = new List<ContentControl>();
        private const string FONT_FAMILY_NAME = "Snap ITC";
        private FontFamily _fontFamily = new FontFamily(FONT_FAMILY_NAME);
        private SolidColorBrush _foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0));
        private SolidColorBrush _foregroundFixed = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private const double LABEL_FONT_SIZE_BIG = 90;
        private const double LABEL_FONT_SIZE_SMALL = 62;
        private const double NAME_FONT_SIZE_BIG = 40;
        private const double NAME_FONT_SIZE_SMALL = 28;
        private DispatcherTimer _timerForDelay;
        private ILog _logger = LogManager.GetLogger("LuckyDraw2018");

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F1 || e.Key == Key.F2 || e.Key == Key.F3 || e.Key == Key.F4)
                {
                    ChangePrize(e.Key);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void ChangePrize(Key key)
        {
            switch (key)
            {
                case Key.F1:
                    _currentPrize = PrizeType.First;
                    ChangeCmbNumbers(0);
                    cmbNumbers.Visibility = Visibility.Hidden;
                    break;
                case Key.F2:
                    cmbNumbers.Visibility = Visibility.Visible;
                    _currentPrize = PrizeType.Second;
                    ChangeCmbNumbers(4);
                    break;
                case Key.F3:
                    cmbNumbers.Visibility = Visibility.Visible;
                    _currentPrize = PrizeType.Third;
                    ChangeCmbNumbers(9);
                    break;
                case Key.F4:
                    cmbNumbers.Visibility = Visibility.Hidden;
                    _currentPrize = PrizeType.Fourth;
                    Enable4thPrizeControls(true);
                    break;
            }
        }

        private void ChangeCmbNumbers(int selectedIndex)
        {
            if (cmbNumbers.SelectedIndex == selectedIndex)
            {
                cmbNumbers_SelectionChanged(cmbNumbers, null);
            }
            else
            {
                cmbNumbers.SelectedIndex = selectedIndex;
            }
        }

        private void CreateLuckyBoxes(int count)
        {
            Enable4thPrizeControls(false);
            Label lbl = null;
            if (count == 1)
            {
                lbl = CreateFixedLabel(2, 2, "Table", 2);
                grid1.Children.Add(lbl);

                lbl = CreateFixedLabel(2, 4, "Seat", 2);
                grid1.Children.Add(lbl);

                lbl = CreateLabel(3, 2, LABEL_FONT_SIZE_BIG, "lblTable0", 2);
                grid1.Children.Add(lbl);
                _currentTableList.Add(lbl);
                lbl = CreateLabel(3, 4, LABEL_FONT_SIZE_BIG, "lblSeat0", 2);
                grid1.Children.Add(lbl);
                _currentSeatList.Add(lbl);
                lbl = CreateNameLabel(4, 0, LABEL_FONT_SIZE_BIG, "lblName0", 8, 2, VerticalAlignment.Top);
                grid1.Children.Add(lbl);
                _currentNameList.Add(lbl);
            }
            else
            {
                int currentRow = 1;
                int currentColumn = 0;
                lbl = CreateFixedLabel(1, 0, "Table");
                grid1.Children.Add(lbl);

                lbl = CreateFixedLabel(1, 1, "Seat");
                grid1.Children.Add(lbl);

                lbl = CreateFixedLabel(1, 4, "Table");
                grid1.Children.Add(lbl);

                lbl = CreateFixedLabel(1, 5, "Seat");
                grid1.Children.Add(lbl);
                // < Label Grid.Column = "0" Grid.Row = "1" FontFamily = "Snap ITC" FontSize = "32" Name = "lbl4LiteralTable"
                //Foreground = "White" HorizontalContentAlignment = "Center" VerticalContentAlignment = "Center"
                //HorizontalAlignment = "Right" VerticalAlignment = "Center" > Table:</ Label >
                for (int i = 0; i < count; i++)
                {
                    if (i % 2 == 0)
                    {
                        currentRow++;
                        currentColumn = 0;
                    }
                    else
                    {
                        currentColumn = 4;
                    }
                    lbl = CreateLabel(currentRow, currentColumn, LABEL_FONT_SIZE_SMALL, "lblTable" + i);
                    grid1.Children.Add(lbl);
                    _currentTableList.Add(lbl);
                    currentColumn++;
                    lbl = CreateLabel(currentRow, currentColumn, LABEL_FONT_SIZE_SMALL, "lblSeat" + i);
                    grid1.Children.Add(lbl);
                    _currentSeatList.Add(lbl);
                    currentColumn++;
                    lbl = CreateNameLabel(currentRow, currentColumn, NAME_FONT_SIZE_SMALL, "lblName" + i, 2);
                    grid1.Children.Add(lbl);
                    _currentNameList.Add(lbl);
                }
            }
        }

        private Label CreateFixedLabel(int row, int column, string content, int columnSpan = 1)
        {
            var lbl = new Label();
            lbl.FontFamily = _fontFamily;
            lbl.Foreground = _foregroundFixed;
            lbl.FontSize = 32;
            lbl.FontWeight = FontWeights.Bold;
            Grid.SetColumn(lbl, column);
            Grid.SetRow(lbl, row);
            Grid.SetColumnSpan(lbl, columnSpan);
            lbl.Content = content;
            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbl.VerticalContentAlignment = VerticalAlignment.Bottom;
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            return lbl;
        }

        private void Enable4thPrizeControls(bool is4thPrize)
        {
            _currentTableList.Clear();
            _currentSeatList.Clear();
            _currentNameList.Clear();
            grid1.Children.Clear();
            grid1.Children.Add(imgDog);
            switch (_currentPrize)
            {
                case PrizeType.First:
                    grid1.Children.Add(lblTitle1);
                    break;
                case PrizeType.Second:
                    grid1.Children.Add(lblTitle2);
                    break;
                case PrizeType.Third:
                    grid1.Children.Add(lblTitle3);
                    break;
                case PrizeType.Fourth:
                    grid1.Children.Add(lblTitle4);
                    break;
            }
            grid1.Children.Add(btnStart);
            grid1.Children.Add(imgFireworks1);
            grid1.Children.Add(imgFireworks2);
            grid1.Children.Add(imgFireworks3);
            grid1.Children.Add(imgFireworks4);
            ShowFireworks(false);
            if (is4thPrize)
            {
                grid1.Children.Add(lbl4LiteralTable);
                grid1.Children.Add(txt4TableFrom);
                grid1.Children.Add(lbl4LiteralWave);
                grid1.Children.Add(txt4TableTo);
                grid1.Children.Add(lbl4LiteralSeat);
                grid1.Children.Add(lbl4Seat1);
                grid1.Children.Add(lbl4Seat2);
                grid1.Children.Add(lbl4Seat3);
                _currentSeatList.Add(lbl4Seat1);
                _currentSeatList.Add(lbl4Seat2);
                _currentSeatList.Add(lbl4Seat3);
            }
            else
            {
                grid1.Children.Add(cmbNumbers);
            }
        }

        private void ShowFireworks(bool show)
        {
            Visibility visivility = show ? Visibility.Visible : Visibility.Hidden;
            imgFireworks1.Visibility = visivility;
            imgFireworks2.Visibility = visivility;
            imgFireworks3.Visibility = visivility;
            imgFireworks4.Visibility = visivility;
            if (show)
            {
                grid1.Children.Insert(1, canvasShade);
            }
            else
            {
                grid1.Children.Remove(canvasShade);
            }
        }

        private Label CreateLabel(int row, int column, double fontSize, string name, int columnSpan = 1)
        {
            var lbl = new Label();
            lbl.Name = name;
            lbl.FontFamily = _fontFamily;
            lbl.Foreground = _foreground;
            lbl.FontSize = fontSize;
            lbl.FontWeight = FontWeights.Bold;
            Grid.SetColumn(lbl, column);
            Grid.SetRow(lbl, row);
            Grid.SetColumnSpan(lbl, columnSpan);
            lbl.Content = "99";
            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbl.VerticalContentAlignment = VerticalAlignment.Center;
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            return lbl;
        }


        private Label CreateNameLabel(int row, int column, double fontSize, string name, int columnSpan = 1, int rowSpan = 1, VerticalAlignment verticalContentAlignment = VerticalAlignment.Center)
        {
            var lbl = new Label();
            lbl.Name = name;
            lbl.FontFamily = _fontFamily;
            lbl.Foreground = _foreground;
            lbl.FontSize = fontSize;
            lbl.FontWeight = FontWeights.Bold;
            Grid.SetColumn(lbl, column);
            Grid.SetRow(lbl, row);
            Grid.SetColumnSpan(lbl, columnSpan);
            Grid.SetRowSpan(lbl, rowSpan);
            lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
            lbl.VerticalContentAlignment = verticalContentAlignment;
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            var txt = new TextBlock();
            txt.TextWrapping = TextWrapping.WrapWithOverflow;
            lbl.Content = txt;
            return lbl;
        }

        enum PrizeType
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4
        }

        private void cmbNumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CreateLuckyBoxes(Convert.ToInt32((cmbNumbers.SelectedItem as ComboBoxItem).Content));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _bll.LoadEmployeeData();
                ChangePrize(Key.F1);
                _timerForDelay = new DispatcherTimer();
                _timerForDelay.Tick += _timerForDelay_Tick;
                _timerForDelay.Interval = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["DelaySeconds"]));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isStarted)
                {
                    if (_currentPrize == PrizeType.Fourth)
                    {
                        _currentRoller.StopSeat();
                        string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Images\start.png");
                        (btnStart.Background as ImageBrush).ImageSource = new BitmapImage(new Uri(path));
                        _bll.AddWinners(txt4TableFrom.Text, txt4TableTo.Text, new[] { lbl4Seat1.Content.ToString(), lbl4Seat2.Content.ToString(), lbl4Seat3.Content.ToString() }.ToList());
                        _bll.SaveWinners();
                        _isStarted = false;
                        ShowFireworks(true);
                        _logger.Info($"Table from {txt4TableFrom.Text} to {txt4TableTo.Text} and seats {lbl4Seat1.Content},{lbl4Seat2.Content},{lbl4Seat3.Content} have been selected.");
                    }
                    else
                    {
                        _currentRoller.StopTable();
                        btnStart.IsEnabled = false;
                        _timerForDelay.Start();
                    }
                }
                else
                {
                    ShowFireworks(false);
                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Images\stop.png");
                    (btnStart.Background as ImageBrush).ImageSource = new BitmapImage(new Uri(path));
                    if (_currentPrize == PrizeType.Fourth)
                    {
                        _currentRoller = new Roller(grid1, null, _currentSeatList, _bll.GetSeatsForFourthPrize(), (int)_currentPrize);
                    }
                    else
                    {
                        foreach (var item in _currentNameList)
                        {
                            (item.Content as TextBlock).Text = "";
                        }
                        _currentRoller = new Roller(grid1, _currentTableList, _currentSeatList, _bll.GetAvailableEmployees(), (int)_currentPrize);
                    }
                    _currentRoller.Start();
                    _isStarted = true;
                    _logger.Info($"Lucky draw for {Enum.GetName(typeof(PrizeType), _currentPrize)} prize started.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                MessageBox.Show("Something was going wrong...");
            }
        }

        private void _timerForDelay_Tick(object sender, EventArgs e)
        {
            try
            {
                _timerForDelay.Stop();
                _currentRoller.StopSeat();
                string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Images\start.png");
                (btnStart.Background as ImageBrush).ImageSource = new BitmapImage(new Uri(path));
                btnStart.IsEnabled = true;
                _isStarted = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Below winners have been selected:");
                var winners = new List<string>();
                for (int i = 0; i < _currentTableList.Count; i++)
                {
                    if (i >= _currentRoller.SelectedValues.Count)
                    {
                        break;
                    }
                    var table = _currentRoller.SelectedValues[i].table;
                    var seat = _currentRoller.SelectedValues[i].seat;
                    (_currentNameList[i].Content as TextBlock).Text = _currentRoller.SelectedValues[i].name;
                    sb.Append($"{table}-{seat},");
                    winners.Add((string)table + "_" + (string)seat);
                }
                _bll.AddWinners(winners, (int)_currentPrize);
                _bll.SaveWinners();
                ShowFireworks(true);
                _logger.Info(sb.Remove(sb.Length - 1, 1).ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
    }
}
