namespace ConsoleAdapter.Tests;
using QuickCLI;
using FluentAssertions;
using System.ComponentModel;

public partial class ConsoleAdapterShould
{
    [Fact]
    public void Verify_NamedParameters_NotMixed()
    {
        var adapter = ConsoleAdapter.For((string one, string two, bool three) => string.Empty);
        adapter
            .Invoking(a => a.Execute(
                new string[]
                {
                    "oneval", "-two", "twoval","true"
                }))
            .Should()
            .Throw<AdapterException>();
    }

    [Fact]
    public void Verify_Parameters_Exist()
    {
        var adapter = ConsoleAdapter.For((string one, string two, bool three) => string.Empty);
        adapter
            .Invoking(a => a.Execute(
                new string[]
                {
                    "oneval", "twoval","--nonexisting"
                }))
            .Should()
            .Throw<AdapterException>();
    }


    [Fact]
    public void Ensure_RequiredParameters_AreSet()
    {
        var adapter = ConsoleAdapter.For((string one, string two, bool three) => string.Empty);
        adapter
            .Invoking(a => a.Execute(
                new string[]
                {
                    "oneval", "--three"
                }))
            .Should()
            .Throw<AdapterException>();
    }

    [Fact]
    public void NotRequire_Booleans()
    {
        var adapter = ConsoleAdapter.For((string one, string two, bool three) => string.Empty);
        adapter
            .Invoking(a => a.Execute(
                new string[]
                {
                    "oneval", "two"
                }))
            .Should()
            .NotThrow();
    }


    [Fact]
    public void NotRequire_WithDefault()
    {
        void testDelegate(string one, string? two = null) { }
        var adapter = ConsoleAdapter.For(testDelegate);
        adapter
            .Invoking(a => a.Execute(
                new string[]
                {
                    "oneval"
                }))
            .Should()
            .NotThrow();
    }

    [Theory]
    [InlineData("oneval twoval yes")]
    [InlineData("oneval twoval true")]
    [InlineData("oneval twoval -three")]
    [InlineData("oneval -two twoval -three")]
    [InlineData("-one oneval -two twoval -three")]
    public void ParseParameters_WithAndWithout_Name(string arguments)
    {
        bool validated = false;
        var adapter = ConsoleAdapter.For((string one, string two, bool three) =>
        {
            one.Should().Be("oneval");
            two.Should().Be("twoval");
            three.Should().Be(true);
            validated = true;
        })
        .Execute(arguments.Split(' '));

        validated.Should().BeTrue();
    }

    [Theory]
    [InlineData("upper some", "SOME")]
    [InlineData("lower SOME", "some")]
    public void Run_MappedCommands(string cmd, string expected)
    {
        ConsoleAdapter
            .Map("upper", (string par) => par.ToUpper())
            .Map("lower", (string par) => par.ToLower())
            .Execute(cmd.Split(' '))
            .Should()
            .Be(expected);
    }


    [Fact]
    public void ReturnHelp_ForMappedCommands()
    {
        var help = ConsoleAdapter
            .Map("upper", (string par) => par.ToUpper())
            .Map("lower", (string par) => par.ToLower())
            .GetHelp(Array.Empty<string>());
    }

    [Fact]
    public void ReturnCommandHelp_ForAllParams()
    {
        void testDelegate(
            string one,
            string? two = null,
            bool three = false)
        { }

        var help = ConsoleAdapter
            .For(testDelegate)
            .GetHelp(Array.Empty<string>());

        help.Should().ContainAll(new[] { "one", "two", "three" });
    }

    [Fact]
    public void ReturnDescriptionValue_ForAnotatedParams()
    {
        void testDelegate(
            string one,
            [Description("XXXX")] string? two = null,
            bool three = false)
        { }

        var help = ConsoleAdapter.For(testDelegate).GetHelp(Array.Empty<string>());
        help.Should().Contain("XXXX");
    }
}
