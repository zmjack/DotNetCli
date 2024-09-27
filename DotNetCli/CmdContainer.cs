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
        private readonly InvalidOperationException NoProjectInfoException = new("No project setted.");
        public Project? Project { get; set; }
        public string CliName { get; }
        public event Action<Exception> OnException;

        public readonly Dictionary<string, Type> Commands = [];
        public readonly Dictionary<string, CommandAttribute> CommandAttributes = [];

        public CmdContainer(string cliName, Assembly cliAssembly)
        {
            CliName = cliName;

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

        public CmdContainer(string cliName, Assembly cliAssembly, Project projectInfo) : this(cliName, cliAssembly)
        {
            Project = projectInfo;
        }

        public void ClearExceptionHandler()
        {
            OnException = null;
        }

        public virtual void PrintUsage()
        {
            if (Project is null) throw NoProjectInfoException;

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
            if (Project is null) throw NoProjectInfoException;

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

                    var command = Activator.CreateInstance(cmdType, [this, args]) as Command;
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
            if (Project is null) throw NoProjectInfoException;

            if (Project is not null)
            {
                Console.WriteLine($@"
* {nameof(Project.Value.ProjectName)}:        {Project.Value.ProjectName}
* {nameof(Project.Value.AssemblyName)}:       {Project.Value.AssemblyName}
* {nameof(Project.Value.RootNamespace)}:      {Project.Value.RootNamespace}
* {nameof(Project.Value.TargetFramework)}:    {Project.Value.TargetFramework}");
                Console.WriteLine();
            }
        }

    }
}
