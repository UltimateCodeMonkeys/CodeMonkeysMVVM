﻿using System;

namespace CodeMonkeys.Logging.AppCenter
{
    public class ServiceProvider : ILogServiceProvider
    {
        public IScopedLogService Create(string context)
        {
            Argument.NotEmptyOrWhiteSpace(
                context,
                nameof(context));

            Configure();

            return new AppCenterLogService(
                context);
        }

        private void Configure()
        {
            var iosApiKey = AppCenterLogService.Options.IOSApiKey;
            var androidApiKey = AppCenterLogService.Options.IOSApiKey;
            var uwpApiKey = AppCenterLogService.Options.IOSApiKey;

            if (string.IsNullOrWhiteSpace(iosApiKey) &&
                string.IsNullOrWhiteSpace(androidApiKey) &&
                string.IsNullOrWhiteSpace(uwpApiKey))
            {
                throw new ArgumentException("No api keys are configured for logging to Microsoft App Center!");
            }

            Microsoft.AppCenter.AppCenter.Configure(
                $"ios={iosApiKey};" +
                $"android={androidApiKey};" +
                $"uwp={uwpApiKey}");
        }
    }
}
