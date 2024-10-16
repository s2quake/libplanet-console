using JSSoft.Commands;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console.")]
[CommandExample("run --end-point localhost:5000 --node-count 4 --client-count 2")]
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
            var applicationOptions = _applicationSettings.ToOptions();
            var application = new Application(applicationOptions, [.. _settingsCollection]);
            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(System.Console.Out);
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
