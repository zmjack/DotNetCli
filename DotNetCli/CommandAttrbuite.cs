﻿using System;

namespace DotNetCli
{
    public class CommandAttribute : Attribute, IDecorator
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
