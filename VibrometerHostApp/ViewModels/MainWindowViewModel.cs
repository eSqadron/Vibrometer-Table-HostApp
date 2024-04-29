using ReactiveUI;
using VibrometerHostApp.Views;

namespace VibrometerHostApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _contentViewModel;
        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        public MainWindowViewModel()
        {
            _contentViewModel = new ConnectionViewModel();
        }
    }
}
