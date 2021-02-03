# DotNetCli

Simple way to create a **CLI** tool.

<br/>

## Install

- **Package Manager**

  ```
  Install-Package DotNetCli
  ```

- **.NET CLI**

  ```
  dotnet add package DotNetCli
  ```

- **PackageReference**

  ```
  <PackageReference Include="DotNetCli" Version="*" />
  ```

<br/>

## Usage

1. Create a simple **Command**.

```csharp
[Command("hello", Abbreviation = "hi", Description = "Say Hello.")]
public class HelloCommand : Command
{
    [CmdProperty("name", Abbreviation = "n", Description = "Your Name.")]
    public string InputName { get; set; }

    public HelloCommand(CmdContainer container, string[] args) : base(container, args) { }

    public override void Run()
    {
        if (Arguments["-h"].Concat(Arguments["--help"]).Any())
        {
            Console.WriteLine();
            PrintUsage();
            return;
        }

        Console.WriteLine($"Hello {InputName}.");
    }
}
```

2. Create a **CmdContainer** and cache all commands of executing assembly.

```csharp
static void Main(string[] args)
{
    var cmdContainer = new CmdContainer("cli");
    cmdContainer.CacheCommands(Assembly.GetExecutingAssembly());
}
```

3. Call from outside.

```powershell
dotnet cli hi -n Jack
dotnet cli hi --name Jack
dotnet cli hello -n Jack
dotnet cli hello --name Jack
```

The output is:

```powershell
Usage: dotnet cli (hi|hello) [Options]

Options:
  -n|--name   Your Name.

Hello Jack.
```

