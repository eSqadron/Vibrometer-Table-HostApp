using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibrometerHostApp.Models;

namespace VibrometerHostApp.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {
        public ConnectionViewModel()
        {
            PortList = new ObservableCollection<string>();
        }
        public ObservableCollection<string> PortList {  get; }

        public void UpdateList()
        {
            PortList.Clear();
            PortList.AddRange(VibrometerConnection.GetPortNames());
        }

        public void Connect()
        {
        }
    }
}
