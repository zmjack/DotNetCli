using Ink;
using NStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetCli
{
    public class CmdContainer
    {
        public readonly ProjectInfo ProjectInfo;
        public readonly string CliName;
        public event Action<Exception> OnException;

        public readonly Dictionary<string, Type> Commands = new();
        public readonly Dictionary<string, CommandAttribute> CommandAttributes = new();

        public CmdContainer(string cliName, Assembly cliAssembly, ProjectInfo projectInfo)
        {
            CliName = cliName;
            ProjectInfo = projectInfo;

            var types = cliAssembly.GetTypesWhichMarkedAs<CommandAttribute>();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<CommandAttribute>();
                if (!attr.Name.IsNullOrWhiteSpace())
                {
                    Commands[attr.Name.Trim().ToLower()] = type;
                    CommandAttributes[attr.Name.Trim().ToLower()] = attr;
                }
                if (!attr.Abbreviation.IsNullOrWhiteSpace())
                {
                    Commands[attr.Abbreviation.Trim().ToLower()] = type;
                    CommandAttributes[attr.Abbreviation.Trim().ToLower()] = attr;
                }
            }
        }

        public void ClearExceptionHandler()
        {
            OnException = null;
        }

        public virtual void PrintUsage()
        {
            var entry = Assembly.GetEntryAssembly().GetName();

            Console.WriteLine($@"{entry.Name} v{entry.Version}

Usage: dotnet {CliName} [command]

Commands:");
            Echo.NoBorderTable(CommandAttributes.Select(x => new
            {
                x.Value.Name,
                x.Value.Abbreviation,
                x.Value.Description,
            })).Line();
        }

        public virtual void Run(string[] args)
        {
            if (!args.Any())
            {
                PrintUsage();
                return;
            }

            try
            {
                try { Console.CursorVisible = false; }
                catch { }

                var cmdName = args[0].ToLower();
                if (Commands.ContainsKey(cmdName))
                {
                    var cmdType = Commands[cmdName];
                    var cmdAttr = CommandAttributes[cmdName];

                    var command = Activator.CreateInstance(cmdType, new object[] { this, args }) as Command;
                    command.Name = cmdAttr.Name;
                    command.Abbreviation = cmdAttr.Abbreviation;
                    command.Description = cmdAttr.Description;
                    command.InternalRun();
                }
                else PrintUsage();
            }
            catch (Exception ex)
            {
                if (OnException is not null) OnException?.Invoke(ex);
                else throw;
            }
            finally
            {
                try { Console.CursorVisible = true; }
                catch { }
            }
        }

        public virtual void PrintProjectInfo()
        {
            Console.WriteLine($@"
* {nameof(ProjectInfo.ProjectName)}:        {ProjectInfo.ProjectName}
* {nameof(ProjectInfo.AssemblyName)}:       {ProjectInfo.AssemblyName}
* {nameof(ProjectInfo.RootNamespace)}:      {ProjectInfo.RootNamespace}
* {nameof(ProjectInfo.TargetFramework)}:    {ProjectInfo.TargetFramework}");
            Console.WriteLine();
        }

    }
}
