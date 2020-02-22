﻿using NStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetCli
{
    public class CommandContainer
    {
        public readonly ProjectInfo ProjectInfo;
        public readonly string CliCommandName;

        public readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();
        public readonly List<CommandAttribute> CommandAttributes = new List<CommandAttribute>();

        public CommandContainer(ProjectInfo projectInfo, string cliCommandName)
        {
            ProjectInfo = projectInfo;
            CliCommandName = cliCommandName;
        }

        public virtual void CacheCommands(Assembly assembly)
        {
            var types = assembly.GetTypesWhichMarkedAs<CommandAttribute>();
            foreach (var type in types)
            {
                var command = Activator.CreateInstance(type) as ICommand;
                var attr = type.GetCustomAttribute<CommandAttribute>();
                CommandAttributes.Add(attr);

                if (!attr.Name.IsNullOrWhiteSpace())
                    Commands[attr.Name.Trim().ToLower()] = command;
                if (!attr.ShortName.IsNullOrWhiteSpace())
                    Commands[attr.ShortName.Trim().ToLower()] = command;
            }
        }

        public virtual void PrintUsage()
        {
            Console.WriteLine($@"
Usage: dotnet {CliCommandName} [command]

Commands:
  {"Name".PadRight(20)}{"\t"}{"ShortName".PadRight(10)}{"\t"}{"Description"}");

            foreach (var attr in CommandAttributes)
                Console.WriteLine($"  {attr.Name.PadRight(20)}\t{attr.ShortName.PadRight(10)}\t{attr.Description}");
            Console.WriteLine();
        }

        public virtual void Run(string[] args)
        {
            if (!args.Any())
            {
                PrintUsage();
                return;
            }

            Console.CursorVisible = false;
            try
            {
                if (Commands.ContainsKey(args[0]))
                    Commands[args[0].ToLower()].Run(args);
                else Console.WriteLine($"Unkown command: {args[0]}");
            }
            finally
            {
                Console.CursorVisible = true;
                Console.WriteLine("Completed.");
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
