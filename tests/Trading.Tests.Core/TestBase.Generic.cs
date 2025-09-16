using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using Xunit.Abstractions;
using Trading.Tests.Core.Generators;

namespace Trading.Tests.Core;

/// <summary>
/// Generic base for tests focused on a single generator.
/// </summary>
/// <typeparam name="TGenerator">Concrete generator implementing <see cref="IGenerator{TModel}"/> and parameterless ctor.</typeparam>
/// <typeparam name="TModel">Generated model type.</typeparam>
public abstract class TestBase<TGenerator, TModel>(ITestOutputHelper output) : IDisposable
    where TGenerator : IGenerator<TModel>, new()
{
    private readonly List<IDisposable> disposables = new();

    protected ITestOutputHelper Output { get; } = output ?? throw new ArgumentNullException(nameof(output));

    protected IFixture Fixture { get; } = new Fixture().Customize(new AutoMoqCustomization
    {
        ConfigureMembers = true
    });

    protected TGenerator Generator { get; } = new();

    protected CancellationToken TestTimeout(int seconds = 10)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
        disposables.Add(cts);
        return cts.Token;
    }

    public void Dispose()
    {
        for (var i = disposables.Count - 1; i >= 0; i--)
        {
            try
            {
                disposables[i].Dispose();
            }
            catch
            {
                 /* best-effort */
            }
        }
        disposables.Clear();
        GC.SuppressFinalize(this);
    }
}