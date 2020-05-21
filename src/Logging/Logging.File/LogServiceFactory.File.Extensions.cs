﻿using CodeMonkeys.Logging.File;

using static CodeMonkeys.Argument;

using System;

namespace CodeMonkeys.Logging
{
    public static partial class LogServiceFactoryExtensions
    {
        /// <summary>
        /// Makes the <see cref="FileLogServiceProvider"/> known to the factory and uses the default configuration.
        /// </summary>
        public static void AddFile(this ILogServiceFactory _this)
        {
            _this.AddFile(new FileLogOptions());
        }

        /// <summary>
        /// Makes the <see cref="FileLogServiceProvider"/> known to the factory and uses the given configuration.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="FileLogOptions.FileNamePrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="FileLogOptions.Extension"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FileLogOptions.FileNamePrefix"/> is empty or whitespace</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FileLogOptions.Extension"/> is empty or whitespace</exception>
        public static void AddFile(
            this ILogServiceFactory _this,
            FileLogOptions options)
        {
            NotNull(
                options,
                nameof(options));

            NotEmptyOrWhiteSpace(
                options.FileNamePrefix,
                nameof(options.FileNamePrefix));

            NotEmptyOrWhiteSpace(
                options.Extension,
                nameof(options.Extension));

            var provider = new FileLogServiceProvider(options);

            _this.AddProvider(provider);
        }

        /// <summary>
        /// Makes the <see cref="FileLogServiceProvider"/> known to the factory and uses the given <paramref name="optionsFactory"/> to configure itself.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsFactory"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="optionsFactory"/> produces a null value.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="FileLogOptions.FileNamePrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="FileLogOptions.Extension"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FileLogOptions.FileNamePrefix"/> is empty or whitespace</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="FileLogOptions.Extension"/> is empty or whitespace</exception>
        public static void AddFile(
            this ILogServiceFactory _this,
            Func<FileLogOptions> optionsFactory)
        {
            NotNull(
                optionsFactory,
                nameof(optionsFactory));

            var options = optionsFactory() ??
                throw new InvalidOperationException($"The {nameof(optionsFactory)} produced a null value.");

            _this.AddFile(options);
        }
    }
}
