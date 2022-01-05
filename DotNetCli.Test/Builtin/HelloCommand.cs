using NStandard;
using System;
using System.Linq;

namespace DotNetCli.Test.Builtin
{
    [Command("hello", Abbreviation = "hi", Description = "Say Hello.")]
    public class HelloCommand : Command
    {
        [CmdProperty("name", Abbreviation = "n", Description = "Your Name.")]
        public string InputName { get; set; }

        [CmdProperty("enable", Abbreviation = "e", Description = "Enable?")]
        public bool Enable { get; set; }

        [CmdProperty("flags", Abbreviation = "f", Description = "Flags (integers).")]
        public int[] Flags { get; set; }

        public HelloCommand(CmdContainer container, string[] args) : base(container, args) { }

        public override void Run()
        {
            Console.WriteLine($"Hello {InputName}. (Enable: {Enable}, Flags: {Flags?.Select(x => x.ToString()).Join("|") ?? "null"})");
        }
    }
}
