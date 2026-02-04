using System.Reflection;

namespace DotNetCli.Test
{
    public static class Util
    {
        public static readonly CmdContainer DefaultCmdContainer = new("cli", Assembly.GetExecutingAssembly(), Project.GetFromDirectory("../../.."));

    }
}
