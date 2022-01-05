using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNetCli.Test
{
    public static class Util
    {
        public static CmdContainer DefaultCmdContainer = new("cli", Assembly.GetExecutingAssembly(), ProjectInfo.GetFromDirectory("../../.."));

    }
}
