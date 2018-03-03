using ElectroMaster.Services.Application;
using ElectroMaster.ViewModels.Base;
using System.Threading;
using System.Windows;

namespace ElectroMaster.ViewModels
{
    public class StartupWindowViewModel : BaseViewModel
    {
        private string _loadingText;
        public string LoadingText
        {
            get => _loadingText;
            private set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this._loadingText = value;
                    base.NotifyPropertyChanged("LoadingText");
                });
            }
        }

        public StartupWindowViewModel()
        {
            LoadingText = "Loading...";

            InitialSetup();
        }

        private void InitialSetup()
        {
            Thread thread = new Thread(async () =>
            {
                LoadingText = "Getting current time...";
                await DateTimeService.SetApplicationDateTime();
                LoadingText = DateTimeService.GetCurrentTimeInDateTime().ToString();
            });

            thread.Start();
        }
    }
}
