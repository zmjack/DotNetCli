using NStandard;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DotNetCli
{
    public struct ProjectInfo
    {
        public string ProjectRoot { get; set; }
        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }
        public string RootNamespace { get; set; }
        public string TargetFramework { get; set; }
        public string CliPackagePath { get; set; }

        public static ProjectInfo GetCurrent() => Get(Directory.GetCurrentDirectory());
        public static ProjectInfo Get(string projectRootDirectory)
        {
            var projectFile = Directory.GetFiles(projectRootDirectory, "*.csproj").For(files =>
            {
                if (files.Length == 0)
                    throw new FileLoadException("The .csproj file has not found in the current directory.");
                else if (files.Length > 1)
                    throw new FileLoadException("More than one .csproj files are exist in the current directory.");
                else return files[0];
            });
            var projectName = Path.GetFileName(projectFile);

            var xml = new XmlDocument().Then(x => x.Load(projectFile));
            return new ProjectInfo
            {
                ProjectRoot = projectRootDirectory,
                ProjectName = projectName,
                AssemblyName = InnerText(xml.SelectNodes("/Project/PropertyGroup/AssemblyName")) ?? Path.GetFileNameWithoutExtension(projectName),
                RootNamespace = InnerText(xml.SelectNodes("/Project/PropertyGroup/RootNamespace")) ?? Path.GetFileNameWithoutExtension(projectName),
                TargetFramework = InnerText(xml.SelectNodes("/Project/PropertyGroup/TargetFramework")) ?? "Unknown",
                CliPackagePath = NuGet.PackageFolder(Assembly.GetCallingAssembly()),
            };
        }

        private static string InnerText(XmlNodeList @this)
        {
            return @this.For(nodeList =>
            {
                if (nodeList.Count > 0) return nodeList[0].InnerText;
                else return null;
            });
        }

    }
}
