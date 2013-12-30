namespace JSLintNet.Console.Specifications
{
    using System;
    using System.Linq;
    using JSLintNet.Console.Abstractions;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using Moq;
    using Xunit;

    public class ConsoleWriterUnit
    {
        public class Write : UnitBase
        {
            [Fact(DisplayName = "Should split text into lines")]
            public void Spec01()
            {
                using (var testable = new WriteTestable())
                {
                    testable.Instance.Write("This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end.");

                    var lines = testable.LastWrite.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    I.Expect(lines.All(x => x.Length <= 80)).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should split text into lines with indentation")]
            public void Spec02()
            {
                using (var testable = new WriteTestable())
                {
                    testable.Instance.Write(8, "This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end. This is quite a long sentence with some words and spaces, just one comma and a full stop at the end.");

                    var lines = testable.LastWrite.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    I.Expect(lines.All(x => x.Length <= 80)).ToBeTrue();
                    I.Expect(lines.All(x => x.StartsWith("        "))).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should escape format characters")]
            public void Spec03()
            {
                using (var testable = new WriteTestable())
                {
                    testable.Instance.Write("{0} {1}", "format", "error with {braces}");

                    I.Expect(testable.LastWrite).ToBe("format error with {{braces}}");
                }
            }

            [Fact(DisplayName = "Should throw given too long indentation")]
            public void Spec04()
            {
                using (var testable = new WriteTestable())
                {
                    I.Expect(() =>
                    {
                        testable.Instance.Write(41, string.Empty);
                    }).ToThrow<ArgumentException>();
                }
            }

            private class WriteTestable : ConsoleWriterTestableBase
            {
            }
        }

        private abstract class ConsoleWriterTestableBase : TestableBase<ConsoleWriter>
        {
            public ConsoleWriterTestableBase()
            {
                this.ConsoleWrapperMock = this.AutoMocker.Mock<IConsoleWrapper>();
                this.BufferWidth = 80;

                this.BeforeInit += this.OnBeforeInit;
            }

            public int BufferWidth { get; set; }

            public string LastWrite { get; set; }

            public Mock<IConsoleWrapper> ConsoleWrapperMock { get; set; }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                this.ConsoleWrapperMock
                    .SetupGet(x => x.BufferWidth)
                    .Returns(() => this.BufferWidth);

                this.ConsoleWrapperMock
                    .Setup(x => x.Write(It.IsAny<string>(), It.IsAny<object[]>()))
                    .Callback((string x, object[] y) => this.LastWrite = x);
            }
        }
    }
}
