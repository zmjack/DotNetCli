using System.Reflection;

namespace DotNetCli.Test
{
    internal static class Util
    {
        public static CmdContainer DefaultCmdContainer = new CmdContainer("cli");

        static Util()
        {
            DefaultCmdContainer.CacheCommands(Assembly.GetExecutingAssembly());
        }

    }
}
