using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WkXamarinToolBox.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected Shell Shell => Shell.Current;

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

        private string _tryAgainBtnText = "TENTAR NOVAMENTE";
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

        #region [ ALERTS ]

        //Loaded when I come to the new page
        public virtual Task InitAsync(object args = null) =>
            Task.CompletedTask;

        //Loaded when I came back to the page
        public virtual Task BackAsync(object args = null) =>
            Task.CompletedTask;

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
            if (MainThread.IsMainThread) return App.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
            else
            {
                TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await App.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);
                    tcs.TrySetResult(result);
                });

                return tcs.Task;
            }
        }

        #endregion
    }
}
