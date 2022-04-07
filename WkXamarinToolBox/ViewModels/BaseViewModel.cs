using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WkXamarinToolBox.Services.DeviceInfos;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WkXamarinToolBox.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected Shell Shell => Shell.Current;

        protected virtual string TryAgainLaterOrContactSupportMessage =>
            "Please, try again later. If the error persists contact the support.";

        public string Key { get; } = Guid.NewGuid().ToString();

        #region [ BUSY STATE ]

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                SetProperty(ref _isNotBusy, !value);
            }
        }

        private bool _isNotBusy = true;
        public bool IsNotBusy
        {
            get => _isNotBusy;
            private set => SetProperty(ref _isNotBusy, value);
        }

        #endregion

        #region [ ERROR STATE ]

        private bool _isShowingError;
        public bool IsShowingError
        {
            get => _isShowingError;
            set => SetProperty(ref _isShowingError, value);
        }

        private string _lastErrorMsg;
        public string LastErrorMsg
        {
            get => _lastErrorMsg;
            set => SetProperty(ref _lastErrorMsg, value);
        }

        private string _tryAgainBtnText = "Try Again";
        public string TryAgainBtnText
        {
            get => _tryAgainBtnText;
            set => SetProperty(ref _tryAgainBtnText, value);
        }

        private Command _tryAgainCommand;
        public Command TryAgainCommand
        {
            get => _tryAgainCommand;
            set => SetProperty(ref _tryAgainCommand, value);
        }

        #endregion

        #region [ INTERNET STATE ]

        private bool _hasInternetConnection = true;
        public bool HasInternetConnection
        {
            get => _hasInternetConnection;
            set
            {
                SetProperty(ref _hasInternetConnection, value);

                if (HasInternetConnectionChanged is not null)
                    HasInternetConnectionChanged.Invoke(this, value);
            }
        }

        protected event EventHandler<bool> HasInternetConnectionChanged;

        private AsyncCommand _updateInternetStateCommand;
        public virtual AsyncCommand UpdateInternetStateCommand => _updateInternetStateCommand ??= new(UpdateInternetStateCommandExecute);

        private async Task UpdateInternetStateCommandExecute()
        {
            var has = DeviceInfosService.HasInternetConnection();

            if (has)
                await DisplayAlert("SUCCESS", "Your internet connection was restored!", "OK");
            else
                await DisplayAlert("FAIL", "The application didn't connect to the internet...", "OK");

            HasInternetConnection = has;
        }

        protected virtual void AtualizarEstadoInternet() =>
            HasInternetConnection = DeviceInfosService.HasInternetConnection();

        protected virtual Task MostrarMensagemSemInternet() =>
            DisplayAlert("WARNING", "The application needs an internet connection to access that function!", "OK");

        #endregion

        #region [ PROP WATCHER ]

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;

            backingStore = value;

            OnPropertyChanged(propertyName);

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public virtual Task InitAsync(object args = null) =>
            Task.CompletedTask;

        public virtual Task ReceiveDataBackAsync(object args = null) =>
            Task.CompletedTask;

        #region [ ALERTS ]

        public Task DisplayAlert(string title, string message, string cancel) =>
            MainThread.IsMainThread ?
            Application.Current.MainPage.DisplayAlert(title, message, cancel) :
            MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.DisplayAlert(title, message, cancel));

        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            if (MainThread.IsMainThread)
                return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

                tcs.TrySetResult(result);
            });

            return tcs.Task;
        }

        public Task<String> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            if (MainThread.IsMainThread) return Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
            else
            {
                TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
                    tcs.TrySetResult(result);
                });

                return tcs.Task;
            }
        }

        #endregion
    }
}
