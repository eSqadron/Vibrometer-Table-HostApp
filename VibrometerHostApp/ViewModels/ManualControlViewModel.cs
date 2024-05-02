using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using VibrometerHostApp.Models;

namespace VibrometerHostApp.ViewModels
{
    public class ManualControlViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> GoToDefinition { get; }
        public ReactiveCommand<Unit, Unit> GoToScanning { get; }

        public ReactiveCommand<Unit, Unit> CH0GoToPoint { get; }
        public ReactiveCommand<Unit, Unit> CH1GoToPoint { get; }

        public ReactiveCommand<Unit, Unit> CH0GetPoint { get; }
        public ReactiveCommand<Unit, Unit> CH1GetPoint { get; }

        public ReactiveCommand<Unit, Unit> CH0StartMotor { get; }
        public ReactiveCommand<Unit, Unit> CH1StartMotor { get; }

        public ReactiveCommand<Unit, Unit> CH0StopMotor { get; }
        public ReactiveCommand<Unit, Unit> CH1StopMotor { get; }

        public ReactiveCommand<Unit, Unit> CH0MotorGetStatus { get; }
        public ReactiveCommand<Unit, Unit> CH1MotorGetStatus { get; }

        public double? CH0SetPointValue { set; get; }
        public double? CH1SetPointValue { set; get; }


        private string _ch1GetPointValue = String.Empty;
        public string CH1GetPointValue
        {
            get => _ch1GetPointValue;
            private set => this.RaiseAndSetIfChanged(ref _ch1GetPointValue, value);
        }


        private string _ch0GetPointValue = String.Empty;
        public string CH0GetPointValue
        {
            get => _ch0GetPointValue;
            private set => this.RaiseAndSetIfChanged(ref _ch0GetPointValue, value);
        }

        private string _ch0MotorStatus = String.Empty;
        public string CH0MotorStatus
        {
            get => _ch0MotorStatus;
            private set => this.RaiseAndSetIfChanged(ref _ch0MotorStatus, value);
        }

        private string _ch1MotorStatus = String.Empty;
        public string CH1MotorStatus
        {
            get => _ch1MotorStatus;
            private set => this.RaiseAndSetIfChanged(ref _ch1MotorStatus, value);
        }

        private bool _canGoToScanning = true;
        public bool CanGoToScanning
        {
            get => _canGoToScanning;
            set => this.RaiseAndSetIfChanged(ref _canGoToScanning, value);
        }

        public ManualControlViewModel(MainWindowViewModel parentRef)
        {
            VibrometerConnection v_conn = VibrometerConnection.Instance;

            GoToDefinition = ReactiveCommand.Create(() => { parentRef.MoveToDefinition(); });
            GoToScanning = ReactiveCommand.Create(() => { parentRef.MoveToScanning(); });

            CH0GoToPoint = ReactiveCommand.Create(() => { v_conn.SetPosition(Channel.CH0, CH0SetPointValue ?? throw new Exception()); });
            CH1GoToPoint = ReactiveCommand.Create(() => { v_conn.SetPosition(Channel.CH1, CH1SetPointValue ?? throw new Exception()); });

            CH0GetPoint = ReactiveCommand.Create(() => { CH0GetPointValue = v_conn.GetPosition(Channel.CH0); });
            CH1GetPoint = ReactiveCommand.Create(() => { CH1GetPointValue = v_conn.GetPosition(Channel.CH1); });

            CH0StartMotor = ReactiveCommand.Create(() => { v_conn.MotorStart(Channel.CH0); });
            CH1StartMotor = ReactiveCommand.Create(() => { v_conn.MotorStart(Channel.CH1); });

            CH0StopMotor = ReactiveCommand.Create(() => { v_conn.MotorStop(Channel.CH0); });
            CH1StopMotor = ReactiveCommand.Create(() => { v_conn.MotorStop(Channel.CH1); });

            CH0MotorGetStatus = ReactiveCommand.Create(() => { CH0MotorStatus = v_conn.MotorGetStatus(Channel.CH0); });
            CH1MotorGetStatus = ReactiveCommand.Create(() => { CH1MotorStatus = v_conn.MotorGetStatus(Channel.CH1); });
        }

        public void CheckScanning()
        {
            VibrometerConnection v_conn = VibrometerConnection.Instance;

            CanGoToScanning = v_conn.GetStatus() != "Uninitialised";
        }
    }
}
