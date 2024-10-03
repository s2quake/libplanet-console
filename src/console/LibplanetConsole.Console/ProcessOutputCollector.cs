using System.Diagnostics;
using System.Text;

namespace LibplanetConsole.Console;

public sealed class ProcessOutputCollector : IDisposable
{
    private readonly ProcessBase _process;
    private readonly StringBuilder _outputBuilder = new();
    private bool _isDisposed = false;

    public ProcessOutputCollector(ProcessBase process)
    {
        _process = process;
        _process.OutputDataReceived += Process_OutputDataReceived;
    }

    public string Output => _outputBuilder.ToString();

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _process.OutputDataReceived -= Process_OutputDataReceived;
            _isDisposed = true;
        }
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is string text)
        {
            if (_outputBuilder.Length > 0)
            {
                _outputBuilder.AppendLine();
            }

            _outputBuilder.Append(text);
        }
    }
}
