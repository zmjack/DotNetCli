using System;

namespace DotNetCli.Test.Builtin
{
    [Command("hello", Abbreviation = "hi", Description = "Say Hello.")]
    public class HelloCommand : Command
    {
        [CmdProperty("name", Abbreviation = "n", Description = "")]
        public string YourName { get; set; }

        public HelloCommand(string[] args) : base(args) { }

        public override void Run()
        {
            Console.WriteLine($"Hello {YourName}.");
        }

    }
}
