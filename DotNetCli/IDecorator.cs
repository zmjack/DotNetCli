namespace DotNetCli
{
    public interface IDecorator
    {
        string Name { get; }
        string Abbreviation { get; }
        string Description { get; }
    }
}
