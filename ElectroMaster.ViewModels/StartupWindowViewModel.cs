using ElectroMaster.Core.Services.Application;
using ElectroMaster.Services.Application;
using ElectroMaster.ViewModels.Base;
using ElectroMaster.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        private bool _isInitializeError;
        public bool IsInitializeError
        {
            get => _isInitializeError;
            private set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _isInitializeError = value;
                    base.NotifyPropertyChanged("LoadingTextVisibility");
                    base.NotifyPropertyChanged("ErrorActionsVisibility");
                    base.NotifyPropertyChanged("ErrorActionsRecountText");
                });
            }
        }

        public Visibility LoadingTextVisibility => (!_isInitializeError) ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ErrorActionsVisibility => (_isInitializeError) ? Visibility.Visible : Visibility.Collapsed;

        private short _errorActionsRecount;
        protected short ErrorActionsRecount
        {
            get => _errorActionsRecount;
            private set
            {
                _errorActionsRecount = value;
                base.NotifyPropertyChanged("ErrorActionsRecountText");
            }
        }
        public string ErrorActionsRecountText => ErrorActionsRecount.ToString() + " second" + ((ErrorActionsRecount != 1) ? "s" : "").ToString();

        private bool _isInitializeSetup;
        private object _initializeSetupLock;
        private object _errorActionsCountDownLock;

        public StartupWindowViewModel()
        {
            _loadingText = "Please wait...";

            _isInitializeError = false;
            _isInitializeSetup = false;

            _errorActionsRecount = 10;

            _initializeSetupLock = new object();
            _errorActionsCountDownLock = new object();

            InitializeCommands();

            InitializeSetup();
        }

        private void InitializeSetup()
        {
            lock (_initializeSetupLock)
            {
                if (!_isInitializeSetup)
                {
                    _isInitializeSetup = true;
                    IsInitializeError = false;
                    LoadingText = "Loading...";

                    Thread thread = new Thread(async () =>
                    {
                        //Creating funcs
                        ICollection<Func<Task<bool>>> funcs = new List<Func<Task<bool>>>()
                        {
                            //Get Current Date Time
                            async () =>
                            {
                                LoadingText = "Getting current time...";
                                return await DateTimeService.SetApplicationDateTime();
                            }
                        };


                        //Executing funcs
                        bool task = false;
                        foreach (Func<Task<bool>> func in funcs)
                        {
                            task = await func();

                            if (!task)
                            {
                                ErrorActionsCountDown();
                                break;
                            }
                        }
                    });

                    thread.Start();
                }
            }
        }

        private void ErrorActionsCountDown()
        {
            lock (_errorActionsCountDownLock)
            {
                if (_isInitializeSetup)
                {
                    _isInitializeSetup = false;
                    ErrorActionsRecount = 10;
                    IsInitializeError = true;

                    IBackgroundService backgroundService = new BackgroundService();
                    backgroundService.Start(() =>
                    {
                        if (ErrorActionsRecount > 1 && !_isInitializeSetup)
                        {
                            ErrorActionsRecount--;
                        }
                        else
                        {
                            RetryInitialize();
                            backgroundService.Stop();
                        }
                    }, 1000);
                }
            }
        }


        private void RetryInitialize()
        {
            if (!_isInitializeSetup)
            {
                LoadingText = "Please wait...";
                ErrorActionsRecount = 10;
                InitializeSetup();
            }
        }

        private void InitializeCommands()
        {
            CloseApplcationCommand = new RelayCommand(CloseApplcation, (obj) => { return ErrorActionsRecount > 1; });
            RetryInitializeCommand = new RelayCommand(RetryInitialize, (obj) => { return ErrorActionsRecount > 1; });
        }

        public RelayCommand CloseApplcationCommand { get; private set; }

        private void CloseApplcation(object parameter)
        {
            if (ErrorActionsRecount > 1 && parameter != null)
            {
                (parameter as Window).Close();
            }
        }

        public RelayCommand RetryInitializeCommand { get; private set; }

        private void RetryInitialize(object parameter)
        {
            RetryInitialize();
        }

    }
}
