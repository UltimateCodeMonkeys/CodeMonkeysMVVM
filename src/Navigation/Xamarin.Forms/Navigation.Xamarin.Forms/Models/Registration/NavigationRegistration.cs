﻿using System;

namespace CodeMonkeys.Navigation.Xamarin.Forms
{
    public class NavigationRegistration :
        INavigationRegistration
    {
        public virtual Type ViewModelType { get; internal set; }
        public virtual Type ViewType { get; internal set; }


        public virtual Type InterestedType { get; internal set; }



        /// <inheritdoc cref="INavigationRegistration.ResolveViewUsingDependencyInjection" />
        public bool ResolveViewUsingDependencyInjection { get; set; }
        public bool PreCreateInstance { get; set; }


        public DevicePlatforms Platform { get; set; } = DevicePlatforms.All;


        private Func<bool> condition = () => true;
        public Func<bool> Condition
        {
            get => condition;
            set
            {
                condition = value ?? (() => true);
            }
        }



        public override bool Equals(
            object other)
        {
            if (!(other is NavigationRegistration registration))
            {
                return false;
            }

            if (ViewModelType != registration.ViewModelType)
            {
                return false;
            }

            if (ViewType != registration.ViewType)
            {
                return false;
            }

            if (Platform != registration.Platform)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ViewModelType,
                ViewType,
                Platform);
        }
    }
}