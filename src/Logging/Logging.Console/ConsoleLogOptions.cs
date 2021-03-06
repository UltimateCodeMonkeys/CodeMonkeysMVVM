﻿namespace CodeMonkeys.Logging.Console
{
    public class ConsoleLogOptions : LogOptions
    {
        private const bool DEFAULT_COLORIZEOUTPUT = true;

        /// <summary>
        /// Flag which indicates if the console output should be colorized depending on the log level.
        /// <para>Default value: <see langword="true"/>.</para>
        /// </summary>
        public bool ColorizeOutput
        {
            get => GetValue(DEFAULT_COLORIZEOUTPUT);
            set => SetValue(value);
        }
    }
}
