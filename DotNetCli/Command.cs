
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

            var props = from prop in GetType().GetProperties()
                        let attr = prop.GetCustomAttributes(typeof(CmdPropertyAttribute), false).FirstOrDefault() as CmdPropertyAttribute
                        where attr is not null
                        select new { Property = prop, Attribute = attr };

            foreach (var prop in props)
            {
                var valueList = new List<string>();
                if (!prop.Attribute.Abbreviation.IsNullOrWhiteSpace()) valueList.AddRange(Arguments[$"-{prop.Attribute.Abbreviation}"]);
                if (!prop.Attribute.Name.IsNullOrWhiteSpace()) valueList.AddRange(Arguments[$"--{prop.Attribute.Name}"]);

                if (valueList.Any())
                {
                    var propertyType = prop.Property.PropertyType;
                    static object ConvertTo(string value, Type type)
                    {
                        if (type == typeof(string)) return value;
                        else if (type == typeof(bool) && value.IsNullOrWhiteSpace()) return true;
                        else
                        {
                            try { return ConvertEx.ChangeType(value, type); }
                            catch { throw new ArgumentException($"The value ({value}) can not convert to {type.FullName}."); }
                        }
                    }

                    if (propertyType.IsArray)
                    {
                        var elementType = propertyType.GetElementType();
                        var array = Array.CreateInstance(elementType, valueList.Count);
                        for (int i = 0; i < valueList.Count; i++)
                            array.SetValue(ConvertTo(valueList[i], elementType), i);
                        prop.Property.SetValue(this, array);
                    }
                    else prop.Property.SetValue(this, ConvertTo(valueList.FirstOrDefault(), propertyType));
                }
            }
        }

        internal void InternalRun()
        {
            if (Arguments["-h"].Concat(Arguments["--help"]).Any())
            {
                Console.WriteLine();
                PrintUsage();
                return;
            }
            Run();
        }

        public abstract void Run();

        public void PrintUsage()
        {
            var props =
                from prop in GetType().GetProperties()
                let attribute = prop.GetCustomAttributes(typeof(CmdPropertyAttribute), false).FirstOrDefault() as CmdPropertyAttribute
                where attribute is not null
                select new
                {
                    Property = prop,
                    Attribute = attribute,
                };

            if (!props.Any())
            {
                Console.WriteLine($"Usage: dotnet {Container.CliName} ({Abbreviation}|{Name})");
                return;
            }
            else
            {
                var nameLength = props.Max(p => $"  {$"-{p.Attribute.Abbreviation}|--{p.Attribute.Name}"}").For(x => (x.Length + 1) + 4 - (x.Length + 1) % 4);
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
