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
            Util.DefaultCmdContainer.Run(new[] { "hello", "-h" });
            Util.DefaultCmdContainer.Run(new[] { "hello", "-n", "Jack" });

            var output = ConsoleAgent.ReadAllText();
            Assert.Equal($@"
Usage: dotnet cli (hi|hello) [Options]

Options:
  -n|--name   Your Name.

Hello Jack.
", output);
        }

    }
}
