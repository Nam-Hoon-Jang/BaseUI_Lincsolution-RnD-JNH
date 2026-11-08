using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;

namespace BaseUI
{
    public interface INavigationService : INotifyPropertyChanged
    {
        void NavigateTo<TViewModel>(object? parameter = null, bool forceRefresh = false) where TViewModel : class;
        void NavigateTo<TViewModel>(Func<TViewModel> factory, object? parameter = null, bool forceRefresh = false) where TViewModel : class;
        void GoBack(object? parameter = null);
        void ClearHistory();
        object CurrentViewModel
        {
            get;
        }
    }
    public interface INavigationNotice
    {
        void OnNavigatedTo(object? parameter);
        void OnNavigatedFrom();
    }
    public partial class NavigationService : ObservableObject, INavigationService
    {
        private readonly IServiceProvider _provider;
        private readonly Stack<object> _history = new();

        [ObservableProperty] private object _CurrentViewModel;

        public NavigationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void NavigateTo<TViewModel>(object? parameter = null, bool forceRefresh = false) where TViewModel : class
        {
            var vm = _provider.GetService(typeof(TViewModel)) as TViewModel;
            if(vm == null)
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel)} not registered");
            //NavigateTo(vm);
            NavigateCore(vm, parameter, forceRefresh);
        }

        public void NavigateTo<TViewModel>(Func<TViewModel> factory, object? parameter = null, bool forceRefresh = false) where TViewModel : class
        {
            var vm = factory();
            //NavigateTo(vm);
            NavigateCore(vm!, parameter, forceRefresh);
        }

        private void NavigateTo(object viewModel)
        {

            if(viewModel == CurrentViewModel)
            {
                return;
            }
            if(CurrentViewModel != null)
                _history.Push(CurrentViewModel);

            CurrentViewModel = viewModel;
        }

        public void GoBack(object? parameter = null)
        {
            if(_history.Count <= 0)
            {
                return;
            }
            //if(_history.Count > 0) CurrentViewModel = _history.Pop();
            if(CurrentViewModel is INavigationNotice fromVm)
            {
                fromVm.OnNavigatedFrom();
            }
            CurrentViewModel = _history.Pop();
            if(CurrentViewModel is INavigationNotice toVm)
            {
                toVm.OnNavigatedTo(parameter);
            }
        }

        public void ClearHistory() => _history.Clear();

        private void NavigateCore(object viewModel, object? parameter, bool forceRefresh)
        {
            if(!forceRefresh && ReferenceEquals(viewModel, CurrentViewModel))
            {
                return;
            }
            if(CurrentViewModel is INavigationNotice fromVm)
            {
                fromVm.OnNavigatedFrom();
            }
            if(CurrentViewModel != null && ReferenceEquals(viewModel, CurrentViewModel))
            {
                _history.Push(CurrentViewModel);
            }
            CurrentViewModel = viewModel;

            if(CurrentViewModel is INavigationNotice toVm)
            {
                toVm.OnNavigatedTo(parameter);
            }
        }
    }
    public class DummyNavigationService : ObservableObject, INavigationService
    {
        private readonly Stack<object> _history = new();
        public object CurrentViewModel => null;
        public void NavigateTo<TViewModel>(object? parameter = null, bool forceRefresh = false) where TViewModel : class
        {

        }
        public void NavigateTo<TViewModel>(Func<TViewModel> factory, object? parameter = null, bool forceRefresh = false) where TViewModel : class
        {
            var vm = factory();
            NavigateTo(vm);
        }

        private void NavigateTo(object viewModel)
        {
            _history.Push(viewModel);
        }

        public void GoBack(object? parameter = null)
        {
            if(_history.Count > 0)
                _history.Pop();
        }

        public void ClearHistory() => _history.Clear();
    }
    public class DummyMessenger : IMessenger
    {
        
        public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            return false;
        }


        public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler)
            where TRecipient : class
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
        
        }


        public void UnregisterAll(object recipient)
        {
        
        }


        public void UnregisterAll<TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
        
        }


        public void Unregister<TMessage, TToken>(object recipient, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
        
        }


        public TMessage Send<TMessage, TToken>(TMessage message, TToken token)
            where TMessage : class
            where TToken : IEquatable<TToken>
        {
            return null;
        }


        public void Cleanup()
        {
        
        }


        public void Reset()
        {
        
        }
    }
}
