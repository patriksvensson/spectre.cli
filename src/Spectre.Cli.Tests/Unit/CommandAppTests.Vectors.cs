using Shouldly;
using Spectre.Cli.Exceptions;
using Spectre.Cli.Testing;
using Spectre.Cli.Testing.Data.Commands;
using Spectre.Cli.Testing.Data.Settings;
using Xunit;

namespace Spectre.Cli.Tests
{
    public sealed partial class CommandAppTests
    {
        public sealed class Vectors
        {
            [Fact]
            public void Should_Throw_If_A_Single_Command_Has_Multiple_Argument_Vectors()
            {
                // Given
                var app = new CommandApp();
                app.Configure(config =>
                {
                    config.PropagateExceptions();
                    config.AddCommand<GenericCommand<MultipleArgumentVectorSettings>>("multi");
                });

                // When
                var result = Record.Exception(() => app.Run(new[] { "multi", "a", "b", "c" }));

                // Then
                result.ShouldBeOfType<ConfigurationException>().And(ex =>
                {
                    ex.Message.ShouldBe("The command 'multi' specifies more than one vector argument.");
                });
            }

            [Fact]
            public void Should_Throw_If_An_Argument_Vector_Is_Not_Specified_Last()
            {
                // Given
                var app = new CommandApp();
                app.Configure(config =>
                {
                    config.PropagateExceptions();
                    config.AddCommand<GenericCommand<MultipleArgumentVectorSpecifiedFirstSettings>>("multi");
                });

                // When
                var result = Record.Exception(() => app.Run(new[] { "multi", "a", "b", "c" }));

                // Then
                result.ShouldBeOfType<ConfigurationException>().And(ex =>
                {
                    ex.Message.ShouldBe("The command 'multi' specifies an argument vector that is not the last argument.");
                });
            }

            [Fact]
            public void Should_Assign_Values_To_Argument_Vector()
            {
                // Given
                var app = new CommandAppFixture();
                app.Configure(config =>
                {
                    config.PropagateExceptions();
                    config.AddCommand<GenericCommand<ArgumentVectorSettings>>("multi");
                });

                // When
                var (result, _, _, settings) = app.Run(new[]
                {
                    "multi", "a", "b", "c",
                });

                // Then
                result.ShouldBe(0);
                settings.ShouldBeOfType<ArgumentVectorSettings>().And(vec =>
                {
                    vec.Foo.Length.ShouldBe(3);
                    vec.Foo[0].ShouldBe("a");
                    vec.Foo[1].ShouldBe("b");
                    vec.Foo[2].ShouldBe("c");
                });
            }

            [Fact]
            public void Should_Assign_Values_To_Option_Vector()
            {
                // Given
                var app = new CommandAppFixture();
                app.Configure(config =>
                {
                    config.PropagateExceptions();
                    config.AddCommand<OptionVectorCommand>("cmd");
                });

                // When
                var (result, _, _, settings) = app.Run(new[]
                {
                    "cmd", "--foo", "red",
                    "--bar", "4", "--foo", "blue",
                });

                // Then
                result.ShouldBe(0);
                settings.ShouldBeOfType<OptionVectorSettings>().And(vec =>
                {
                    vec.Foo.ShouldBe(new string[] { "red", "blue" });
                    vec.Bar.ShouldBe(new int[] { 4 });
                });
            }
        }
    }
}
