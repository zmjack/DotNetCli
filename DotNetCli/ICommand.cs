using NEcho;

namespace DotNetCli
{
    public interface ICommand
    {
        void Run(ConArgs cargs);
        void PrintUsage();
    }
}
