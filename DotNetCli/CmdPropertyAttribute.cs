using System;

namespace DotNetCli
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CmdPropertyAttribute : Attribute, IDecorator
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public CmdPropertyAttribute(string name)
        {
            Name = name;
        }
    }

}
