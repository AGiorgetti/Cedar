﻿namespace Cedar.Logging
{
    using System;
    using Cedar.Logging.LogProviders;
    using Xunit;

    public class LogProviderTests : IDisposable
    {
        public void Dispose()
        {
            NLogLogProvider.ProviderIsAvailableOverride = true;
            Log4NetLogProvider.ProviderIsAvailableOverride = true;
        }

        [Fact]
        public void When_NLog_is_available_Then_should_get_NLogLogger()
        {
            LogProvider.SetCurrentLogProvider(null);
            NLogLogProvider.ProviderIsAvailableOverride = true;
            Log4NetLogProvider.ProviderIsAvailableOverride = true;
            ILog logger = LogProvider.GetCurrentClassLogger();
            Assert.IsType<NLogLogProvider.NLogLogger>(((LoggerExecutionWrapper) logger).WrappedLogger);
        }

        [Fact]
        public void When_Log4Net_is_available_Then_should_get_Log4NetLogger()
        {
            LogProvider.SetCurrentLogProvider(null);
            NLogLogProvider.ProviderIsAvailableOverride = false;
            Log4NetLogProvider.ProviderIsAvailableOverride = true;
            ILog logger = LogProvider.GetLogger(GetType());
            Assert.IsType<Log4NetLogProvider.Log4NetLogger>(((LoggerExecutionWrapper) logger).WrappedLogger);
        }

        [Fact]
        public void When_neither_NLog_or_Log4Net_is_available_Then_should_get_NoOpLogger()
        {
            LogProvider.SetCurrentLogProvider(null);
            NLogLogProvider.ProviderIsAvailableOverride = false;
            Log4NetLogProvider.ProviderIsAvailableOverride = false;
            ILog logger = LogProvider.GetLogger(GetType());
            Assert.IsType<LogProvider.NoOpLogger>(logger);
        }
    }
}