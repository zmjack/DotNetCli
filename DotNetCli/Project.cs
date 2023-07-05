using System.IO;
using System.Reflection;
using System.Xml;

namespace DotNetCli
{
    public struct Project
    {
        public string ProjectRoot { get; set; }
        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }
        public string RootNamespace { get; set; }
        public string TargetFramework { get; set; }
        public string CliPackagePath { get; set; }
        public string Sdk { get; set; }

        public static Project GetFromDirectory(string directory)
        {
            var files = Directory.GetFiles(directory, "*.csproj");

            if (files.Length == 0) throw new FileLoadException("The .csproj file has not found in the current directory.");
            else if (files.Length > 1) throw new FileLoadException("More than one .csproj files are exist in the current directory.");
            else return GetFromFile(files[0]);
        }

        public static Project GetFromFile(string csprojFile)
        {
            var projectName = Path.GetFileName(csprojFile);

            var xml = new XmlDocument();
            xml.Load(csprojFile);

            return new Project
            {
                ProjectRoot = Path.GetDirectoryName(csprojFile),
                ProjectName = projectName,
                AssemblyName = xml.SelectSingleNode("/Project/PropertyGroup/AssemblyName")?.InnerText ?? Path.GetFileNameWithoutExtension(projectName),
                RootNamespace = xml.SelectSingleNode("/Project/PropertyGroup/RootNamespace")?.InnerText ?? Path.GetFileNameWithoutExtension(projectName),
                TargetFramework = xml.SelectSingleNode("/Project/PropertyGroup/TargetFramework")?.InnerText ?? "Unknown",
                CliPackagePath = NuGet.PackageFolder(Assembly.GetCallingAssembly()),
                Sdk = xml.SelectSingleNode("/Project")?.Attributes["Sdk"]?.InnerText,
            };
        }

    }
}
