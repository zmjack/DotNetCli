using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotNetCli
{
    public static class NuGet
    {
        public static readonly string PackagesPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.nuget\packages\";

        public static string PackageFolder(Assembly assembly)
        {
            var regex = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
            var assemblyName = assembly.GetName();
            var match = regex.Match(assemblyName.Version.ToString());
            var version = new Version(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

            return PackageFolder(assemblyName.Name, version);
        }

        public static string PackageFolder(string package, Version version)
        {
            var major = version.Major >= 0 ? version.Major : 0;
            var minor = version.Minor >= 0 ? version.Minor : 0;
            var build = version.Build >= 0 ? version.Build : 0;
            var revision = version.Revision >= 0 ? version.Revision : 0;
            var nuVersion = revision == 0 ? $"{major}.{minor}.{build}" : $"{major}.{minor}.{build}.{revision}";

            return $@"{PackagesPath}{package.ToLower()}\{nuVersion}\";
        }

    }
}
