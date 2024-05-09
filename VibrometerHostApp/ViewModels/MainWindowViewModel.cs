using ReactiveUI;
using System;
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

        public ViewModelBase? _contentViewModel = null;
        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel ?? throw new Exception("Empty Main screen");
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        public MainWindowViewModel()
        {
            ConnView = new ConnectionViewModel(this);
            ConfView = new ConfigureViewModel(this);
            ScanView = new ScanningViewModel(this);
            ManualView = new ManualControlViewModel(this);

            ContentViewModel = ConnView;
        }

        public void Connect(string port)
        {
            var _connection = VibrometerConnection.Instance;

            if (_connection.IsConnected)
            {
                if(_connection.Port == port)
                {
                    ContentViewModel = ConfView;
                    return;
                }
                else
                {
                    _connection.Disconnect();
                }
            }

            _connection.Port = port;
            _connection.Connect();

            if (!_connection.GetScanHelp().Contains("Subcommands"))
            {
                _connection.Disconnect();
                throw new VibrometerException("This isn't Vibrometer Controller!");
            }

            ContentViewModel = ConfView;
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
            ManualView.CheckScanning();
            ContentViewModel = ManualView;           
        }

        public void MoveToConnectionControl()
        {
            ContentViewModel = ConnView;
        }
    }
}
