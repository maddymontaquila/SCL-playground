using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Spectre.Console;

namespace SCL_playground
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Create the root command "aspire"
            var rootCommand = new RootCommand("Aspire command line interface")
            {
                Name = "aspire"
            };

            // Create the "dev" subcommand
            var devCommand = new Command("dev", "Start the development dashboard");
            devCommand.SetHandler(() =>
            {
                AnsiConsole.MarkupLine("[green]Starting Aspire development dashboard...[/]");
                AnsiConsole.Status()
                    .Start("Starting services...", ctx => 
                    {
                        ctx.Spinner(Spectre.Console.Spinner.Known.Dots);
                        Thread.Sleep(2000);
                        AnsiConsole.MarkupLine("[bold green]Dashboard is running at http://localhost:5000[/]");
                    });
            });

            // Create the "new" subcommand with options
            var newCommand = new Command("new", "Create a new Aspire project");
            var templateOption = new Option<string>("--template", "The template to use");
            var outputOption = new Option<string>("--output", "The output directory");
            
            newCommand.AddOption(templateOption);
            newCommand.AddOption(outputOption);
            
            newCommand.SetHandler((string template, string output) =>
            {
                AnsiConsole.MarkupLine($"[blue]Creating new Aspire project with template:[/] [yellow]{template}[/]");
                AnsiConsole.MarkupLine($"[blue]Output directory:[/] [yellow]{output}[/]");
                
                AnsiConsole.Progress()
                    .Start(ctx => 
                    {
                        var task = ctx.AddTask("[green]Creating project[/]");
                        for (int i = 0; i <= 100; i += 10)
                        {
                            task.Value = i;
                            Thread.Sleep(200);
                        }
                    });
                
                AnsiConsole.MarkupLine("[bold green]Project created successfully![/]");
            }, templateOption, outputOption);

            // Create the "add" subcommand with argument
            var addCommand = new Command("add", "Add a project or service to the solution");
            var projectArgument = new Argument<string>("project", "The project to add");
            
            addCommand.AddArgument(projectArgument);
            
            addCommand.SetHandler((string project) =>
            {
                var table = new Table();
                table.AddColumn("Project");
                table.AddColumn("Status");
                
                table.AddRow(project, "[green]Added successfully[/]");
                
                AnsiConsole.Write(new Rule("[yellow]Adding Project[/]").RuleStyle("grey"));
                AnsiConsole.Write(table);
                AnsiConsole.Write(new Rule().RuleStyle("grey"));
            }, projectArgument);

            // Add subcommands to the root command
            rootCommand.AddCommand(devCommand);
            rootCommand.AddCommand(newCommand);
            rootCommand.AddCommand(addCommand);

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
