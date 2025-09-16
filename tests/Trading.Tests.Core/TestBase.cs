using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using Xunit.Abstractions;

namespace Trading.Tests.Core;

/// <summary>
/// Shared base for tests. Provides output, timeout helper, AutoFixture+AutoMoq,
/// and typed generators for common domain objects.
/// </summary>
public abstract class TestBase(ITestOutputHelper output) : IDisposable
{
    private readonly List<IDisposable> disposables = [];
    
    protected ITestOutputHelper Output { get; } = output ?? throw new ArgumentNullException(nameof(output));
    protected IFixture Fixture { get; } = new Fixture().Customize(new AutoMoqCustomization
    {
        ConfigureMembers = true
    });

    protected Faker Faker { get; } = new();

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