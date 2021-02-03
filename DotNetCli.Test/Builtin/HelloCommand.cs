using System;
using System.Linq;
using System.Reflection;

namespace DotNetCli.Test.Builtin
{
    [Command("hello", Abbreviation = "hi", Description = "Say Hello.")]
    public class HelloCommand : Command
    {
        [CmdProperty("name", Abbreviation = "n", Description = "Your Name.")]
        public string InputName { get; set; }

        public HelloCommand(CmdContainer container, string[] args) : base(container, args) { }

        public override void Run()
        {
            if (Arguments["-h"].Concat(Arguments["--help"]).Any())
            {
                Console.WriteLine();
                PrintUsage();
                return;
            }

            Console.WriteLine($"Hello {InputName}.");
        }
    }
}
