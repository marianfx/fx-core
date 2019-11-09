using FluentAssertions;
using FX.Core.Logging.NLog.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using System;
using System.IO;

namespace FX.Core.Tests.Logging
{
    [TestClass]
    public class ExtendedLoggerTests
    {
        private NLog.Config.LoggingConfiguration _config;

        [TestInitialize]
        public void Initialize()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");
            _config = LoggerFactory.LoadConfigFromFile(path);
        }

        [TestMethod]
        public void Given_ConsoleLogger_When_ItsInitializedWithConfig_Then_TheInstance_Should_BeValid()
        {
            ILogger logger = null;
            try
            {
                logger = LoggerFactory.CreateConsoleLogger(_config);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }

            Assert.IsTrue(logger != null);
        }

        [TestMethod]
        public void Given_ConsoleLogger_When_ItsInitializedWithConfig_Then_TheInstance_Should_WriteALogToConsole()
        {
            var stream = new StringWriter();
            var text = "An error message";
            Console.SetOut(stream);
            ILogger logger = null;
            try
            {
                logger = LoggerFactory.CreateConsoleLogger(_config);
                logger.Info(text);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception: " + ex.Message);
            }

            var outputText = stream.ToString();
            outputText.Should().Contain(text);

        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }
}
