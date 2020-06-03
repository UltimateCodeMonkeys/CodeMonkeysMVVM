﻿using CodeMonkeys.Configuration;

namespace CodeMonkeys.Dialogs
{
    public class DialogOptions : Options
    {
        /// <summary>
        /// <para>Defaults to <c>'OK'</c></para>
        /// <para>Value changes of this property are monitored and applied dynamically at runtime.</para>
        /// </summary>
        public string DefaultCloseLabel
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// <para>Defaults to <c>'OK'</c></para>
        /// <para>Value changes of this property are monitored and applied dynamically at runtime.</para>
        /// </summary>
        public string DefaultConfirmLabel
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// <para>Defaults to <c>'Cancel'</c></para>
        /// <para>Value changes of this property are monitored and applied dynamically at runtime.</para>
        /// </summary>
        public string DefaultDeclineLabel
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public DialogOptions()
        {
            DefaultCloseLabel = "OK";
            DefaultConfirmLabel = "OK";
            DefaultDeclineLabel = "Cancel";
        }
    }
}
