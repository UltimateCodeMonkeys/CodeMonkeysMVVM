﻿using Microsoft.Extensions.Primitives;

using System;

namespace CodeMonkeys.Configuration
{
    public abstract class OptionsConsumer<TOptions>
        where TOptions : Options
    {
        protected IDisposable OptionsChangeToken;

        protected OptionsConsumer(TOptions options)
        {
            OptionsChangeToken = ChangeToken.OnChange(
                options.GetChangeToken,
                OnOptionsChanged,
                options);
        }

        protected abstract void OnOptionsChanged(TOptions options);
    }
}
