// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/CommandLineParser
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost;

sealed class Application : IAsyncDisposable, IServiceProvider
{
    private readonly CompositionContainer _container;
    private readonly SwarmHostCollection _swarmHosts = [];
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;
    private SystemTerminal? _terminal;

    public Application()
    {
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue(_swarmHosts);
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    public async Task StartAsync(string[] args)
    {
        if (_terminal != null)
            throw new InvalidOperationException("Application has already been started.");

        if (args.Length > 0 && GetService<CommandContext>() is { } commandContext)
        {
            await commandContext.ExecuteAsync(args, cancellationToken: default, progress: new Progress<ProgressInfo>());
        }

        _cancellationTokenSource = new();
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        await _terminal!.StartAsync(_cancellationTokenSource.Token);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        await _swarmHosts.DisposeAsync();
        _terminal = null;
        _container.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public T? GetService<T>()
    {
        return _container.GetExportedValue<T>();
    }

    public object? GetService(Type serviceType)
    {
        return _container.GetExportedValue<object?>(AttributedModelServices.GetContractName(serviceType));
    }
}
