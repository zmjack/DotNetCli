
using Ink;
using NStandard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCli
{
    public abstract class Command : IDecorator
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public ConArgs Arguments { get; }

        public CmdContainer Container { get; }

        protected Command(CmdContainer container, string[] args)
        {
            Container = container;
            Arguments = new ConArgs(args, "-");

            var props = GetType().GetProperties()
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttributes(typeof(CmdPropertyAttribute), false).FirstOrDefault() as CmdPropertyAttribute,
                })
                .Where(p => p.Attribute is not null);

            foreach (var prop in props)
            {
                IEnumerable<string> values = new string[0];

                if (!prop.Attribute.Abbreviation.IsNullOrWhiteSpace())
                {
                    values = values.Concat(Arguments[$"-{prop.Attribute.Abbreviation}"]);
                }
                if (!prop.Attribute.Name.IsNullOrWhiteSpace())
                {
                    values = values.Concat(Arguments[$"--{prop.Attribute.Name}"]);
                }

                if (values.Any())
                {
                    var propertyType = prop.Property.PropertyType;
                    if (propertyType.IsArray && propertyType.GetElementType() == typeof(string))
                        prop.Property.SetValue(this, values.ToArray());
                    else prop.Property.SetValue(this, values.FirstOrDefault());
                }
            }
        }

        public abstract void Run();

        public void PrintUsage()
        {
            var props = GetType().GetProperties()
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttributes(typeof(CmdPropertyAttribute), false).FirstOrDefault() as CmdPropertyAttribute,
                })
                .Where(p => p.Attribute is not null);

            if (!props.Any())
            {
                Console.WriteLine($"Usage: dotnet {Container.CliName} ({Abbreviation}|{Name})");
                return;
            }
            else
            {
                var nameLength = props.Max(p => $"  {$"-{p.Attribute.Abbreviation}|--{p.Attribute.Name}"}").For(x => x.Length + 4 - x.Length % 4);
                Console.WriteLine($"Usage: dotnet {Container.CliName} ({Abbreviation}|{Name}) [Options]");
                Console.WriteLine();
                Console.WriteLine($"Options:");

                foreach (var prop in props)
                {
                    Console.WriteLine($"  {$"-{prop.Attribute.Abbreviation}|--{prop.Attribute.Name}".PadRightA(nameLength)}{prop.Attribute.Description}");
                }
                Console.WriteLine();
            }
        }
    }

}
