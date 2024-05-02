using ReactiveUI;
using System;
using System.Reactive;
using VibrometerHostApp.Models;

using MeasurementPoint = (double yaw, double pitch);

namespace VibrometerHostApp.ViewModels
{
    public class ScanningViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> GoBackToDefinitionCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToManual { get; }
        public ReactiveCommand<Unit, Unit> StartScanCommand { get; }
        public ReactiveCommand<Unit, Unit> GetPointCommand { get; }
        public ReactiveCommand<Unit, Unit> NextPointCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCommand { get; }
        public ReactiveCommand<Unit, Unit> DumpPointsCommand { get; }
        public ReactiveCommand<Unit, Unit> GetStatusCommand { get; }

        public string _lastPoint = String.Empty;
        public string LastPoint
        {
            get => _lastPoint;
            private set => this.RaiseAndSetIfChanged(ref _lastPoint, value);
        }

        public string _dumpedPoints = String.Empty;
        public string DumpedPoints
        {
            get => _dumpedPoints;
            private set => this.RaiseAndSetIfChanged(ref _dumpedPoints, value);
        }

        public string _status = String.Empty;
        public string Status
        {
            get => "Status: " + _status;
            private set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public ScanningViewModel(MainWindowViewModel parentRef)
        {
            GoBackToDefinitionCommand = ReactiveCommand.Create(() => { parentRef.MoveToDefinition(); });
            GoToManual = ReactiveCommand.Create(() => { parentRef.MoveToManualControl(); });

            StartScanCommand = ReactiveCommand.Create(() => { VibrometerConnection.Instance.StartScan(); });

            NextPointCommand = ReactiveCommand.Create(() => { VibrometerConnection.Instance.GoToNextPointScan(); });
            GetPointCommand = ReactiveCommand.Create(() => {
                MeasurementPoint point = VibrometerConnection.Instance.GetPointScan();
                LastPoint = $"Yaw: {point.yaw}, Pitch: {point.pitch}";
            });

            StopCommand = ReactiveCommand.Create( () => { VibrometerConnection.Instance.StopScan(); });
            GetStatusCommand = ReactiveCommand.Create(() => { Status = VibrometerConnection.Instance.GetStatus(); });

            DumpPointsCommand = ReactiveCommand.Create(() => { DumpedPoints = VibrometerConnection.Instance.DumpPointsFromScan(); });
        }

    }
}
