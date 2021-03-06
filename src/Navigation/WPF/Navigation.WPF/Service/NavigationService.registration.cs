﻿using CodeMonkeys.Logging;
using CodeMonkeys.MVVM;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CodeMonkeys.Navigation.WPF
{
    public partial class NavigationService
    {
        protected static readonly IList<INavigationRegistration> NavigationRegistrations =
            new List<INavigationRegistration>();


        /// <inheritdoc cref="CodeMonkeys.Navigation.INavigationService.Register(INavigationRegistration)" />
        public void Register(INavigationRegistration registration)
        {
            RegisterInternal(registration);

            if (registration.PreCreateInstance)
            {
                Task.Run(() => CreateCachedContent(registration));
            }

            Log?.Info($"Registered ViewModel of type {registration.ViewModelType.Name} to page {registration.ViewType.Name}.");
        }

        public INavigationRegistration Register<TViewModel, TView>()
            where TViewModel : IViewModel
            where TView : class
        {
            var registration = new RegistrationInfo
            {
                ViewModelType = typeof(TViewModel),
                ViewType = typeof(TView)
            };

            RegisterInternal(registration);

            if (registration.PreCreateInstance)
            {
                Task.Run(() => CreateCachedContent(registration));
            }

            Log?.Info($"Registered ViewModel of type {registration.ViewModelType.Name} to page {registration.ViewType.Name}.");

            return registration;
        }


        /// <inheritdoc cref="CodeMonkeys.Navigation.INavigationService.Unregister{TViewModel}" />
        public void Unregister<TViewModel>()

            where TViewModel : class, IViewModel
        {
            if (!TryGetRegistration(
                typeof(TViewModel),
                out var registration))
            {
                return;
            }

            RemoveRegistration(
                registration);
        }

        public void Unregister<TViewModel, TView>()

            where TViewModel : class, IViewModel
            where TView : FrameworkElement
        {
            if (!TryGetRegistration(
                typeof(TViewModel),
                typeof(TView),
                out var registration))
            {
                return;
            }


            RemoveRegistration(
                registration);
        }


        public void ResetRegistrations()
        {
            NavigationRegistrations.Clear();
        }


        private static void RegisterInternal(
            INavigationRegistration registration)
        {
            _semaphore.Wait();

            try
            {
                if (!Configuration.AllowDifferentViewTypeRegistrationForSameViewModel &&
                    TryGetRegistration(
                        registration.ViewModelType,
                        out var registrationInfo))
                {
                    NavigationRegistrations.Remove(registrationInfo);
                }

                NavigationRegistrations.Add(registration);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private static void RemoveRegistration(
            INavigationRegistration registration)
        {
            var cachedPage = ContentCache.FirstOrDefault(
                cache => cache.Type == registration.ViewType);

            if (cachedPage != null)
            {
                ContentCache.Remove(cachedPage);
            }

            NavigationRegistrations.Remove(registration);

            Log?.Info($"Unregistered views from ViewModel of type {registration.ViewModelType.Name}.");
        }


        private static bool TryGetRegistration(
            Type viewModelType,
            out INavigationRegistration registrationInfo)
        {
            if (!IsRegistered(viewModelType))
            {
                registrationInfo = null;
                return false;
            }

            registrationInfo = NavigationRegistrations.OfType<RegistrationInfo>()
                .FirstOrDefault(registration =>
                    registration.ViewModelType == viewModelType);


            return registrationInfo != null;
        }
                

        private static bool TryGetRegistration(
            Type viewModelType,
            Type viewType,
            out INavigationRegistration registrationInfo)
        {
            if (!IsRegistered(viewModelType, viewType))
            {
                registrationInfo = null;
                return false;
            }

            registrationInfo = NavigationRegistrations.FirstOrDefault(registration =>
                registration.ViewModelType == viewModelType &&
                viewType.IsAssignableFrom(registration.ViewType) &&
                registration.Condition?.Invoke() != false);

            return registrationInfo != null;
        }


        private static void ThrowIfNotRegistered<TViewModel>()
        {
            if (IsRegistered(typeof(TViewModel)))
            {
                return;
            }

            // todo: which exception type fits best?
            var notRegisteredException = new InvalidOperationException(
                $"There is no reference from viewmodel type {typeof(TViewModel).Name} to a page.");

            Log?.Error(notRegisteredException);

            throw notRegisteredException;
        }


        private static bool IsRegistered(
            Type viewModelType)
        {
            return IsRegistered(
                viewModelType,
                null);
        }

        private static bool IsRegistered(
            Type viewModelType,
            Type typeOfView)
        {
            var registrations = NavigationRegistrations.Where(
                registration => registration.ViewModelType == viewModelType);


            if (registrations == null ||
                !registrations.Any())
            {
                return false;
            }

            if (typeOfView != null && 
                !registrations.Any(registration => typeOfView.IsAssignableFrom(registration.ViewType)))
            {
                return false;
            }


            return true;
        }
    }
}