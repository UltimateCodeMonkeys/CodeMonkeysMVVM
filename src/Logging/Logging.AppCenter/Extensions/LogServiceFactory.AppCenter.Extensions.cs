﻿using CodeMonkeys.Logging.AppCenter;

namespace CodeMonkeys.Logging
{
    public static partial class LogServiceFactory
    {
        /// <summary>
        /// Method for adding the <see cref="ServiceProvider"/> to the <see cref="LogService"/>.
        /// </summary>
        public static void AddAppCenter(this ILogServiceFactory @this)
        {
            Argument.NotNull(
                @this,
                nameof(@this));

            var provider = new ServiceProvider();
            @this.AddProvider(provider);
        }
    }
}
