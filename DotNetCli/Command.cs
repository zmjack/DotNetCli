
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

        public string[] Arguments { get; }

        public Command(string[] args)
        {
            Arguments = args;

            var conArgs = new ConArgs(args, "-");
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
                    values = values.Concat(conArgs[$"-{prop.Attribute.Abbreviation}"]);
                }
                if (!prop.Attribute.Name.IsNullOrWhiteSpace())
                {
                    values = values.Concat(conArgs[$"--{prop.Attribute.Name}"]);
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
                Console.WriteLine($"{Environment.NewLine}Usage: dotnet ts ({Abbreviation}|{Name})");
                return;
            }
            else
            {
                var nameLength = props.Max(p => p.Attribute.Name).For(x => x.Length + 8 - x.Length % 8);
                Console.WriteLine($"{Environment.NewLine}Usage: dotnet ts ({Abbreviation}|{Name}) [Options]");
                Console.WriteLine($"{Environment.NewLine}Options:");

                foreach (var prop in props)
                {
                    Console.WriteLine($"{Environment.NewLine}Options:");
                    Console.WriteLine($"{$"-{prop.Attribute.Abbreviation}|--{prop.Attribute.Name}".PadRightA(nameLength)}{prop.Attribute.Description}");
                }
            }
        }
    }

}
