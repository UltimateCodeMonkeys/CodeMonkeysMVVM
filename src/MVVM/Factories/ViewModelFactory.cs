﻿using CodeMonkeys.DependencyInjection;
using CodeMonkeys.Logging;
using CodeMonkeys.Navigation;

using System;
using System.Threading.Tasks;

namespace CodeMonkeys.MVVM.Factories
{
    /// <summary>
    /// Factory to create and initialize ViewModel instances using the DI container
    /// </summary>
    [Obsolete("Please use CodeMonkeys.MVVM.ViewModelFactory instead.")]
    public static class ViewModelFactory
    {
        private static ILogService log;
        private static IDependencyResolver iocContainer;

        private static INavigationService navigationServiceInstance;

        /// <summary>
        /// Sets up the factory for further usage by passing the DI container instance and an optional logger
        /// </summary>
        /// <param name="typeResolver">DI container</param>
        /// <param name="logService">LogService to use for internal logging (optional)</param>
        public static void Configure(
            IDependencyResolver typeResolver,
            ILogService logService = null)
        {
            iocContainer = typeResolver;
            log = logService;
        }


        internal static INavigationService TryResolveNavigationServiceInstance()
        {
            try
            {
                return navigationServiceInstance ??
                       (navigationServiceInstance = iocContainer.Resolve<INavigationService>());
            }
            catch (Exception innerException)
            {
                string errorMessage =
                    $"Unable to resolve instance of type {typeof(INavigationService)} --- did you register an implementation?";

                log?.Error(
                    errorMessage,
                    innerException);

                throw new TypeLoadException(
                    errorMessage,
                    innerException);
            }
        }

        /// <summary>
        /// Creates a new ViewModel instance and returns it
        /// InitializeAsync is invoked in the background
        /// </summary>
        /// <typeparam name="TViewModelInterface">Type of the ViewModel to create</typeparam>
        /// <param name="initialize">Should InitializeAsync() get called after getting the instance? <see cref="IViewModel"/></param>
        /// <returns>ViewModel instance of the given type</returns>
        public static TViewModelInterface Resolve<TViewModelInterface>(
            bool initialize = true)
            where TViewModelInterface : class, IViewModel
        {
            try
            {
                var instance = iocContainer.Resolve<TViewModelInterface>();

                if (initialize)
                    TaskHelper.RunSync(instance.InitializeAsync);

                return instance;
            }
            catch (Exception innerException)
            {
                string errorMessage = $"ViewModel of type {typeof(TViewModelInterface).Name} cannot be resolved --- is it registered?";

                log?.Critical(
                    errorMessage,
                    innerException);

                throw new TypeLoadException(
                    errorMessage,
                    innerException);
            }
        }

        /// <summary>
        /// Creates a new ViewModel instance, invokes the InitializeAsync method and returns the initialized instance
        /// </summary>
        /// <typeparam name="TViewModelInterface">Type of the ViewModel to create</typeparam>
        /// <param name="initialize">Should InitializeAsync() get called after getting the instance? <see cref="IViewModel"/></param>
        /// <returns>ViewModel instance of the given type</returns>
        public static async Task<TViewModelInterface> ResolveAsync<TViewModelInterface>(
            bool initialize = true)
            where TViewModelInterface : class, IViewModel
        {
            try
            {
                var instance = iocContainer.Resolve<TViewModelInterface>();

                if (initialize)
                    await instance.InitializeAsync();

                return instance;
            }
            catch (Exception innerException)
            {
                string errorMessage = $"ViewModel of type {typeof(TViewModelInterface).Name} cannot be resolved --- is it registered?";

                log?.Critical(
                    errorMessage,
                    innerException);

                throw new TypeLoadException(
                    errorMessage,
                    innerException);
            }
        }


        /// <summary>
        /// Creates a new ViewModel instance, invokes the InitializeAsync method using the parameter and returns the initialized instance
        /// </summary>
        /// <typeparam name="TViewModelInterface">Type of the ViewModel to create</typeparam>
        /// <typeparam name="TModel">Type of the parameter that will be used for initialization</typeparam>
        /// <returns>ViewModel instance of the given type</returns>
        public static async Task<TViewModelInterface> ResolveAsync<TViewModelInterface, TModel>(
            TModel model)
            where TViewModelInterface : class, IViewModel<TModel>
        {
            try
            {
                var instance = iocContainer.Resolve<TViewModelInterface>();

                await instance.InitializeAsync(
                    model);

                return instance;
            }
            catch (Exception innerException)
            {
                string errorMessage = $"ViewModel of type {typeof(TViewModelInterface).Name} cannot be resolved --- is it registered?";

                log?.Critical(
                    errorMessage,
                    innerException);

                throw new TypeLoadException(
                    errorMessage,
                    innerException);
            }
        }
    }
}