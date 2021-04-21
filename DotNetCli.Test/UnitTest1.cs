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
            var container = TestUtil.DefaultCmdContainer;
            container.Run(new[] { "hello", "-h" });
            container.Run(new[] { "hello", "-n", "Jack" });
            container.Run(new[] { "hello", "-n", "Jack", "-e" });
            container.Run(new[] { "hello", "-n", "Jack", "-e", "-f", "1", "-f", "2" });
            container.Run(new[] { "hello", "-n", "Jack", "-e", "-f", "1", "-f", "a" });

            var output = ConsoleAgent.ReadAllText();
            Assert.Equal($@"
Usage: dotnet cli (hi|hello) [Options]

Options:
  -n|--name       Your Name.
  -e|--enable     Enable?
  -f|--flags      Flags (integers).

Hello Jack. (Enable: False, Flags: null)
Hello Jack. (Enable: True, Flags: null)
Hello Jack. (Enable: True, Flags: 1|2)
The value (a) can not convert to System.Int32.
", output);
        }

    }
}
