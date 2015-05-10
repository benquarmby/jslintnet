namespace JSLintNet.Console.Specifications
{
    using System;
    using JSLintNet.Abstractions;
    using JSLintNet.Json;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Moq;
    using Xunit;

    public class ConsoleOptionsFactoryUnit
    {
        public class Create : UnitBase
        {
            [Fact(DisplayName = "Should assume help given null")]
            public void Spec01()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(null);

                    I.Expect(actual.Help).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should assume help given no arguments")]
            public void Spec02()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[] { });

                    I.Expect(actual.Help).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should set help from first argument")]
            public void Spec03()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[] { "/?" });

                    I.Expect(actual.Help).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should throw given empty source directory")]
            public void Spec04()
            {
                using (var testable = new CreateTestable())
                {
                    I.Expect(() =>
                    {
                        var actual = testable.Instance.Create(new string[] { string.Empty });
                    }).ToThrow<ArgumentException>();
                }
            }

            [Fact(DisplayName = "Should throw given invalid first argument")]
            public void Spec05()
            {
                using (var testable = new CreateTestable())
                {
                    I.Expect(() =>
                    {
                        var actual = testable.Instance.Create(new string[] { "-s" });
                    }).ToThrow<ArgumentException>();
                }
            }

            [Fact(DisplayName = "Should set source directory from first argument")]
            public void Spec06()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[] { "source directory" });

                    I.Expect(actual.SourceDirectory).ToBe("source directory");
                    I.Expect(actual.Help).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should set settings file from arguments")]
            public void Spec07()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[]
                    {
                        "source directory",
                        "-settingsfile",
                        "settings file"
                    });

                    I.Expect(actual.SettingsFile).ToBe("settings file");
                }
            }

            [Fact(DisplayName = "Should set report file from arguments")]
            public void Spec08()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[]
                    {
                        "source directory",
                        "-reportfile",
                        "report file"
                    });

                    I.Expect(actual.ReportFile).ToBe("report file");
                }
            }

            [Fact(DisplayName = "Should set log level from arguments")]
            public void Spec09()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[]
                    {
                        "source directory",
                        "-loglevel",
                        "silent"
                    });

                    I.Expect(actual.LogLevel).ToBe(LogLevel.Silent);
                }
            }

            [Fact(DisplayName = "Should set all from abbreviations")]
            public void Spec10()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[]
                    {
                        "source directory",
                        "/l",
                        "1",
                        "/r",
                        "report file",
                        "/s",
                        "settings file"
                    });

                    I.Expect(actual.SettingsFile).ToBe("settings file");
                    I.Expect(actual.ReportFile).ToBe("report file");
                    I.Expect(actual.LogLevel).ToBe(LogLevel.Verbose);
                }
            }

            [Fact(DisplayName = "Should throw given key without value")]
            public void Spec11()
            {
                using (var testable = new CreateTestable())
                {
                    I.Expect(() =>
                    {
                        var actual = testable.Instance.Create(new string[]
                        {
                            "source directory",
                            "/r",
                            "/s",
                            "settings file"
                        });
                    }).ToThrow<ArgumentException>();
                }
            }

            [Fact(DisplayName = "Should set settings editor to true given a JSON file")]
            public void Spec12()
            {
                using (var testable = new CreateTestable())
                {
                    var actual = testable.Instance.Create(new string[]
                    {
                        @"some\path\to\file.json"
                    });

                    I.Expect(actual.SettingsEditor).ToBe(true);
                }
            }

            private class CreateTestable : ConsoleOptionsFactoryUnitTestableBase
            {
                protected override void BeforeResolve()
                {
                    base.BeforeResolve();

                    this.FileSystemWrapperMock
                        .Setup(x => x.ResolveDirectory(It.IsAny<string>()))
                        .Returns((string x) => x);

                    this.FileSystemWrapperMock
                        .Setup(x => x.ResolveFile(It.IsAny<string>()))
                        .Returns((string x) => x);
                }
            }
        }

        private abstract class ConsoleOptionsFactoryUnitTestableBase : TestFixture<ConsoleOptionsFactory>
        {
            public ConsoleOptionsFactoryUnitTestableBase()
            {
                this.FileSystemWrapperMock = this.AutoMocker.Mock<IFileSystemWrapper>();
                this.JsonProviderMock = this.AutoMocker.Mock<IJsonProvider>();
            }

            public Mock<IFileSystemWrapper> FileSystemWrapperMock { get; set; }

            public Mock<IJsonProvider> JsonProviderMock { get; set; }
        }
    }
}
