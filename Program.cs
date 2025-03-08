using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

// Create the root command "aspire"
var rootCommand = new RootCommand("Aspire command line interface");

// Create the "dev" subcommand
var devCommand = new Command("dev", "Run the Aspire app host");
devCommand.SetAction(action => {
    AnsiConsole.MarkupLine("[green]Starting Aspire...[/]");
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
var templateOption = new Option<string>("--template");
newCommand.Add(templateOption);

newCommand.SetAction(template => {
    AnsiConsole.MarkupLine($"[blue]Creating new Aspire project with template:[/] [yellow]{template}[/]");                
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
});

// Create the "add" command with subcommands
var addCommand = new Command("add", "Add a project or integration to the solution");

// Create "project" subcommand
var projectCommand = new Command("project", "Add a project to the solution");
var projectArgument = new Argument<string>("project");
projectCommand.Add(projectArgument);

projectCommand.SetAction(project => {
    var table = new Table();
    table.AddColumn("Project")
         .AddColumn("Status")
         .AddRow("[green]Added successfully[/]");
    
    AnsiConsole.Write(new Rule("[yellow]Adding Project[/]").RuleStyle("grey"));
    AnsiConsole.Write(table);
    AnsiConsole.Write(new Rule().RuleStyle("grey"));
});

// Create "integration" subcommand
var integrationCommand = new Command("integration", "Add an integration to the solution");
var integrationArgument = new Argument<string?>("integration");
integrationArgument.Arity = ArgumentArity.ZeroOrMore; // Allow zero or more arguments
integrationCommand.Add(integrationArgument);

// Show integrations in a searchable list
integrationCommand.SetAction(integration => {
    var filteredIntegrations = GetIntegrations(integration?.GetResult(integrationArgument)?.ToString());
    var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an integration to add:")
                .EnableSearch()
                .AddChoices(filteredIntegrations)
    );                
    AnsiConsole.Write(new Rule("[yellow]Adding Integration[/]").RuleStyle("grey"));
    AnsiConsole.Write(selection);
    AnsiConsole.Write(new Rule().RuleStyle("grey"));
});

// Add the subcommands to the add command
addCommand.Add(projectCommand);
addCommand.Add(integrationCommand);

// Add subcommands to the root command
rootCommand.Add(devCommand);
rootCommand.Add(newCommand);
rootCommand.Add(addCommand);

// Parse the incoming args and invoke the command "aspire"
await rootCommand.Parse(args).InvokeAsync();

// Get a filtered list of integrations based on the argument passed
static List<string> GetIntegrations(string? integration) {
    var list = new List<string> {
        "Aspire.Hosting.Redis",
        "Aspire.Hosting.SqlServer",
        "Aspire.Hosting.MongoDB",
        "Aspire.Hosting.MySQL",
        "Aspire.Hosting.PostgreSQL",
        "Aspire.Hosting.Azure",
        "Aspire.Hosting.AWS",
    };
    var filteredList = list.Where(i => i.Contains(integration ?? "", StringComparison.OrdinalIgnoreCase)).ToList();
    return filteredList;
}