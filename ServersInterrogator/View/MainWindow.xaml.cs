using ServersInterrogator.Configuration;
using ServersInterrogator.Model;
using ServersInterrogator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServersInterrogator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private  CancellationTokenSource _cancelTokenSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ServerInfoViewModel();
            _cancelTokenSource = new CancellationTokenSource();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
        //это всё во вьюмодели делается. Она же есть у тебя.
            Logger.Logger.Log.Debug(MethodBase.GetCurrentMethod().Name);

            var viewModel = DataContext as ServerInfoViewModel;
            viewModel.ServerInfos.Clear();//на такие операции ObservableCollection небезопасна, насколько я помню.

            var loader = new ConfigurationLoader();
            var config = loader.LoadConfig();
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;

            var task = new Task(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var items = config.Settings.SelectMany(set => GetRows(set.Url, set.Interval, set.Threads)).ToList();//они всё время создаются?
                   
                    App.Current.Dispatcher.Invoke((Action)delegate 
                    {
                        items.ForEach(item => viewModel.ServerInfos.Add(item));
                    });//в ряде случаев (здесь всё равно) лучше инвок делать максимально ближе к критичному методу, чтобы интерфейс меньше подвисал.

                    foreach (var item in items)
                        await item.RequestAsync();//await Task.WhenAll(items.Select(item=>item.RequestAsync())); Сюда бы твой токен тоже неплохо было бы передать, чтобы и вызовы можно было отменять.
                }
                Logger.Logger.Log.Debug("Операция прервана");
            });

             task.Start();
        }


        private void escButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Logger.Log.Debug(MethodBase.GetCurrentMethod().Name);
            _cancelTokenSource.Cancel();
        }

        private IEnumerable<ServerInfo> GetRows(string url, int interval, int threads)
        {
            return Enumerable.Range(1, threads).Select(n => new ServerInfo(url, interval));
        }

    }
}
