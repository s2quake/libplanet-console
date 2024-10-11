using JSSoft.Commands;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console.")]
internal sealed class RunCommand : CommandAsyncBase, ICustomCommandDescriptor
{
    private readonly ApplicationSettingsCollection _settingsCollection = new();
    private readonly Dictionary<CommandMemberDescriptor, object> _descriptorByInstance;
    private readonly CommandMemberDescriptorCollection _descriptors;
    private readonly ApplicationSettings _applicationSettings;

    public RunCommand()
    {
        _descriptorByInstance = GetDescriptors([.. _settingsCollection]);
        _descriptors = new(GetType(), _descriptorByInstance.Keys);
        _applicationSettings = _settingsCollection.Peek<ApplicationSettings>();
    }

    CommandMemberDescriptorCollection ICustomCommandDescriptor.Members => _descriptors;

    object ICustomCommandDescriptor.GetMemberOwner(CommandMemberDescriptor memberDescriptor)
        => _descriptorByInstance[memberDescriptor];

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            // var serviceCollection = new ApplicationServiceCollection(_settingsCollection);
            // var applicationOptions = _applicationSettings.ToOptions();

            // serviceCollection.AddNode(applicationOptions);
            // serviceCollection.AddApplication(applicationOptions);

            // await using var serviceProvider = serviceCollection.BuildServiceProvider();
            // var @out = Console.Out;
            // var application = serviceProvider.GetRequiredService<Application>();
            // await @out.WriteLineAsync();
            // await application.RunAsync();
            // await @out.WriteLineAsync("\u001b0");
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }

    private static Dictionary<CommandMemberDescriptor, object> GetDescriptors(object[] options)
    {
        var itemList = new List<KeyValuePair<CommandMemberDescriptor, object>>();
        for (var i = 0; i < options.Length; i++)
        {
            var option = options[i];
            var descriptors = CommandDescriptor.GetMemberDescriptors(option)
                .Select(item => KeyValuePair.Create(item, option));
            itemList.AddRange(descriptors);
        }

        return new(itemList);
    }
}
