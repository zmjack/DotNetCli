
namespace DotNetCli
{
    public interface ICommand
    {
        void Run(string[] args);
        void PrintUsage();
    }
}
