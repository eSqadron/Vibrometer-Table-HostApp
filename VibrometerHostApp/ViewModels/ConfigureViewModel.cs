using ReactiveUI;
using System;
using System.Reactive;
using VibrometerHostApp.Models;
namespace VibrometerHostApp.ViewModels
{
    public class ConfigureViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> DefineYawCommand { get; }
        public ReactiveCommand<Unit, Unit> DefinePitchCommand { get; }
        public ReactiveCommand<Unit, Unit> ReadyCommand { get; }

        public ReactiveCommand<Unit, Unit> GoToManual { get; }

        public ReactiveCommand<Unit, Unit> GoBackToConnectionCommand { get; }

        //private ScannerAxes _scannerAxesDef;
        //public ScannerAxes ScannerAxesDef
        //{
        //    get => _scannerAxesDef;
        //    private set => this.RaiseAndSetIfChanged(ref _scannerAxesDef, value);
        //}

        public uint? YawChannel { set; get; } = 0;
        public uint? PitchChannel { set; get; } = 1;

        public double? YawMin { set; get; } = null;
        public double? YawMax { set; get; } = null;
        public double? YawDelta { set; get; } = null;

        public double? PitchMin { set; get; } = null;
        public double? PitchMax { set; get; } = null;
        public double? PitchDelta { set; get; } = null;

        private string _returnString = String.Empty;
        public string ReturnString
        {
            get => _returnString;
            private set => this.RaiseAndSetIfChanged(ref _returnString, value);
        }

        public ConfigureViewModel(MainWindowViewModel parentRef)
        {
            //ScannerAxesDef = new ScannerAxes()
            //{
            //    Yaw = new ScannerChannelDefinition() { Channel = 0 },
            //    Pitch = new ScannerChannelDefinition() { Channel = 1 },
            //};

            VibrometerConnection _connection = VibrometerConnection.Instance;

            DefineYawCommand = ReactiveCommand.Create(() => { try { ReturnString = _connection.DefineYaw(new ScannerChannelDefinition(YawChannel, YawMin, YawMax, YawDelta)); } catch (Exception) { ReturnString = "Define All Fields!"; } });
            DefinePitchCommand = ReactiveCommand.Create(() => { try { ReturnString = _connection.DefinePitch(new ScannerChannelDefinition(PitchChannel, PitchMin, PitchMax, PitchDelta)); } catch (Exception) { ReturnString = "Define All Fields!"; } });
            ReadyCommand = ReactiveCommand.Create(() => { try { ReturnString = _connection.ReadyScanner(); parentRef.MoveToScanning(); } catch (Exception) { ReturnString = "Scanner NOT Ready!"; } });


            GoToManual = ReactiveCommand.Create(() => { parentRef.MoveToManualControl(); });
            GoBackToConnectionCommand = ReactiveCommand.Create(() => { parentRef.MoveToConnectionControl(); });
        }
    }
}
