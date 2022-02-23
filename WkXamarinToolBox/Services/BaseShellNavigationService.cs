using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WkXamarinToolBox.ViewModels;
using Xamarin.Forms;

namespace WkXamarinToolBox.Services
{
    public class NavigationService
    {
        private static Lazy<NavigationService> navigationLazy = new(() => new NavigationService());
        public static NavigationService Current => navigationLazy.Value;
        private Shell shell => Shell.Current;
        private Page currentPage => shell.CurrentPage;

        private readonly Dictionary<string, Type> _viewModelsMapping = new Dictionary<string, Type>();
        private readonly Dictionary<string, Type> _shellTabBarPagesMapping = new Dictionary<string, Type>();


        #region [ SHELL TABBAR PAGES ]

        private void RegisterShellTabBarPageViewModel(string pageName, Type viewModelType) =>
            _shellTabBarPagesMapping.Add(pageName, viewModelType);

        private Type GetViewModelTypeFromTabBarMapping(in string pageName)
        {
            if (!_shellTabBarPagesMapping.ContainsKey(pageName))
                throw new Exception($"No map for {pageName} was found on navigation mappings!");

            return _shellTabBarPagesMapping[pageName];
        }

        #endregion

        #region [ NORMAL PAGES ]

        private void RegisterViewModelMap(string viewModelName, Type viewModelType) =>
            _viewModelsMapping.Add(viewModelName, viewModelType);

        private void RegisterRoutes(string viewModelName, Type pageType) =>
            Routing.RegisterRoute(viewModelName, pageType);

        public async Task GoToAsync(string uri, object args = null)
        {
            try
            {
                await shell.GoToAsync(uri);

                if (uri.Contains(".."))
                {
                    await (currentPage.BindingContext as BaseViewModel).BackAsync(args);

                    return;
                }

                await StartViewModel(uri, args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ValueTask StartViewModel(in string uri, object args)
        {
            var viewModel = CreateViewModel(uri);

            currentPage.BindingContext = viewModel;

            return new ValueTask(viewModel.InitAsync(args));
        }

        private BaseViewModel CreateViewModel(in string uri)
        {
            var viewModelType = GetViewModelTypeFromViewModelsMapping(uri);

            var viewModel = (BaseViewModel)Activator.CreateInstance(viewModelType);

            return viewModel;
        }

        private Type GetViewModelTypeFromViewModelsMapping(in string uri)
        {
            if (!_viewModelsMapping.ContainsKey(uri))
                throw new Exception($"No map for {uri} was found on navigation mappings!");

            return _viewModelsMapping[uri];
        }

        #endregion
    }
}
