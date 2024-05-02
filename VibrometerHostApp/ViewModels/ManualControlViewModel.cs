using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace VibrometerHostApp.ViewModels
{
    public class ManualControlViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> GoToDefinition { get; }
        public ReactiveCommand<Unit, Unit> GoToScanning { get; }

        public ManualControlViewModel(MainWindowViewModel parentRef)
        {
            GoToDefinition = ReactiveCommand.Create(() => { parentRef.MoveToDefinition(); });
            GoToScanning = ReactiveCommand.Create(() => { parentRef.MoveToScanning(); });
        }
    }
}
