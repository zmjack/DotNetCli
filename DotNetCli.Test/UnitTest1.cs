using NStandard;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace DotNetCli.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var output = new StringBuilder();

            using (var outputWriter = new StringWriter(output))
            using (var console = ConsoleContext.Begin())
            {
                Console.SetOut(outputWriter);
                Console.SetError(outputWriter);

                var container = Util.DefaultCmdContainer;
                container.OnException += ex =>
                {
                    var innerMostException = Any.Forward(ex, x => x.InnerException)
                        .FirstOrDefault(x => x.InnerException is null);
                    Console.Error.WriteLine(innerMostException.Message);
                };

                container.Run(["hello", "-h"]);
                container.Run(["hello", "-n", "Jack"]);
                container.Run(["hello", "-n", "Jack", "-e"]);
                container.Run(["hello", "-n", "Jack", "-e", "-f", "1", "-f", "2"]);
                container.Run(["hello", "-n", "Jack", "-e", "-f", "1", "-f", "a"]);
            }

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
", output.ToString());
        }

    }
}
