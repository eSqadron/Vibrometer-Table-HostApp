using ReactiveUI;
using System.Reactive.Linq;
using VibrometerHostApp.Models;
using VibrometerHostApp.Views;

namespace VibrometerHostApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ConnectionViewModel ConnView;
        public ConfigureViewModel ConfView;
        public ScanningViewModel ScanView;
        public ManualControlViewModel ManualView;

        public ViewModelBase _contentViewModel;
        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        private VibrometerConnection? _connection;

        public MainWindowViewModel()
        {
            ConnView = new ConnectionViewModel(this);
            ConfView = new ConfigureViewModel(this);
            ScanView = new ScanningViewModel(this);
            ManualView = new ManualControlViewModel(this);

            ContentViewModel = ScanView;
        }

        public string Connect(string port)
        {
            var _connection = VibrometerConnection.Instance;
            _connection.Port = port;
            _connection.Connect();

            ContentViewModel = ConfView;

            return _connection.GetScanHelp();
        }

        public void MoveToScanning()
        {
            ContentViewModel = ScanView;
        }

        public void MoveToDefinition()
        {
            ContentViewModel = ConfView;
        }

        public void MoveToManualControl()
        {
            ContentViewModel = ManualView;
        }

        public void MoveToConnectionControl()
        {
            ContentViewModel = ConnView;
        }
    }
}
