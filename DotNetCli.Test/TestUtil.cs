using System.Reflection;

namespace DotNetCli.Test
{
    internal static class TestUtil
    {
        public static CmdContainer DefaultCmdContainer = new CmdContainer("cli", ProjectInfo.Get("../../.."));

        static TestUtil()
        {
            DefaultCmdContainer.CacheCommands(Assembly.GetExecutingAssembly());
        }

    }
}
