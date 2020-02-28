﻿using Microsoft.Extensions.Primitives;

using System.Threading;

namespace CodeMonkeys.Core.Configuration
{
    public abstract class Options
    {
        private OptionsChangeToken _token = new OptionsChangeToken();

        public IChangeToken GetChangeToken() => _token;

        protected void SetValue<T>(
            ref T field, 
            T value, 
            bool causeReload = true)
        {
            field = value;

            if (causeReload)
                CauseReload();
        }

        private void CauseReload()
        {
            var previousToken = Interlocked.Exchange(ref _token, new OptionsChangeToken());
            previousToken.OnReload();
        }
    }
}
