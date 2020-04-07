﻿using CodeMonkeys.Core.MVVM;
using CodeMonkeys.Core.Navigation;
using CodeMonkeys.Core.Navigation.ViewModels;
using CodeMonkeys.Logging;
using CodeMonkeys.MVVM;

using System.Linq;
using System.Threading.Tasks;

namespace CodeMonkeys.Navigation.Xamarin.Forms
{
    public partial class ViewModelNavigationService :
        IViewModelNavigationService
    {
        /// <inheritdoc cref="CodeMonkeys.Core.Interfaces.Navigation.IViewModelNavigationService.CloseAsync{TViewModelInterface}" />
        public virtual async Task CloseAsync<TViewModelInterface>()

            where TViewModelInterface : class, IViewModel
        {
            var viewType = ViewModelToViewMap[typeof(TViewModelInterface)];

            if (!Navigation.NavigationStack.Any(
                page => page.GetType() == viewType))
            {
                return;
            }

            ThrowIfNotRegistered<TViewModelInterface>(
                viewType);

            var view = Navigation.NavigationStack.First(
                page => page.GetType() == viewType);

            await CloseCurrentPage();
        }

        /// <inheritdoc cref="CodeMonkeys.Core.Interfaces.Navigation.IViewModelNavigationService.CloseAsync{TViewModelInterface, TParentViewModelInterface}" />
        public virtual async Task CloseAsync<TViewModelInterface, TParentViewModelInterface>()

            where TViewModelInterface : class, IViewModel
            where TParentViewModelInterface : class, IViewModel, IListenToChildViewModelClosing
        {
            ThrowIfNotRegistered<TViewModelInterface>(
                CurrentPage.GetType());

            var viewType = ViewModelToViewMap[typeof(TViewModelInterface)];

            if (Navigation.NavigationStack.Last().GetType() == viewType)
            {
                return;
            }

            await ResolveAndInformParent<TParentViewModelInterface>();

            await CloseCurrentPage();
        }

        /// <inheritdoc cref="CodeMonkeys.Core.Interfaces.Navigation.IViewModelNavigationService.CloseAsync{TViewModelInterface, TParentViewModelInterface, TResult}(TResult)" />
        public virtual async Task CloseAsync<TViewModelInterface, TParentViewModelInterface, TResult>(
            TResult result)

            where TViewModelInterface : class, IViewModel
            where TParentViewModelInterface : class, IViewModel, IListenToChildViewModelClosing<TResult>
        {
            ThrowIfNotRegistered<TViewModelInterface>(
                CurrentPage.GetType());

            var viewType = ViewModelToViewMap[typeof(TViewModelInterface)];

            if (Navigation.NavigationStack.Last().GetType() == viewType)
            {
                return;
            }

            await ResolveAndInformParent<TParentViewModelInterface, TResult>(
                result);

            await CloseCurrentPage();
        }

        private async Task CloseCurrentPage()
        {
            await Navigation.PopAsync(
                animated: Configuration.UseAnimations);

            LogService?.Info(
                "Page has been removed from Xamarin navigation stack.");
        }


        private async Task ResolveAndInformParent<TParentViewModelInterface>()

            where TParentViewModelInterface : class, IListenToChildViewModelClosing
        {
            var parentViewModel = dependencyResolver.Resolve<TParentViewModelInterface>();

            LogService?.Info(
                $"ViewModelInstance for type {typeof(TParentViewModelInterface).Name} has been resolved.");

            await parentViewModel.OnChildViewModelClosingAsync();
        }

        private async Task ResolveAndInformParent<TParentViewModelInterface, TResult>(
            TResult result)

            where TParentViewModelInterface : class, IListenToChildViewModelClosing<TResult>
        {
            var parentViewModel = dependencyResolver.Resolve<TParentViewModelInterface>();

            LogService?.Info(
                $"ViewModelInstance for type {typeof(TParentViewModelInterface).Name} has been resolved.");

            await parentViewModel.OnChildViewModelClosingAsync(
                result);
        }
    }
}