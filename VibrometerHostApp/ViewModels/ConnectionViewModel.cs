using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using VibrometerHostApp.Models;

namespace VibrometerHostApp.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ConnectionViewModel(MainWindowViewModel parentRef)
        {
            PortList = new ObservableCollection<string>();

            ConnectCommand = ReactiveCommand.Create(() => { ReturnString = parentRef.Connect(SelectedPort); });
        }

        public ObservableCollection<string> PortList {  get; }

        public string SelectedPort { set; get; } = String.Empty;

        private string _returnString = String.Empty;
        public string ReturnString
        {
            get => _returnString;
            private set => this.RaiseAndSetIfChanged(ref _returnString, value);
        }

        public void UpdateList()
        {
            PortList.Clear();
            PortList.AddRange(VibrometerConnection.GetPortNames());
        }
    }
}
