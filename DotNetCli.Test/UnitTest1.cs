using NStandard;
using System;
using Xunit;

namespace DotNetCli.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            using var console = ConsoleAgent.Begin();
            Util.DefaultCmdContainer.Run(new[] { "hello", "-n", "jack" });

            var output = ConsoleAgent.ReadAllText();
            Assert.Equal($"Hello jack.{Environment.NewLine}", output);
        }

    }
}
