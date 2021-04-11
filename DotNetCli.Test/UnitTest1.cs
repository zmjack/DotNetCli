using NStandard;
using Xunit;

namespace DotNetCli.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            using var console = ConsoleAgent.Begin();
            var container = Util.DefaultCmdContainer;
            container.Run(new[] { "hello", "-h" });
            container.Run(new[] { "hello", "-n", "Jack" });
            container.Run(new[] { "hello", "-n", "Jack", "-e" });

            var output = ConsoleAgent.ReadAllText();
            Assert.Equal($@"
Usage: dotnet cli (hi|hello) [Options]

Options:
  -n|--name       Your Name.
  -e|--enable     Enable?

Hello Jack. (Enable: False)
Hello Jack. (Enable: True)
", output);
        }

    }
}
