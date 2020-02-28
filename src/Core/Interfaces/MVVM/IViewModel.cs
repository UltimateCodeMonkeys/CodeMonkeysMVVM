﻿using System.Threading.Tasks;

namespace CodeMonkeys.Core.Interfaces.MVVM
{
    public interface IViewModel
    {
        /// <summary>
        /// A flag indicating whether this viewmodel has been initialized using the <see cref="CodeMonkeys.Core.Interfaces.MVVM.IViewModel.InitializeAsync" /> method
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Method that automatically gets called when the instance was built using the ViewModelFactory or the <see cref="CodeMonkeys.Core.Interfaces.Navigation.IViewModelNavigationService" />
        /// </summary>
        Task InitializeAsync();
    }
}