namespace JSLintNet.Specifications.Models
{
    using System.Collections.Generic;
    using IExpect;
    using JSLintNet.QualityTools;
    using Microsoft.Build.Framework;
    using Moq;
    using Xunit;

    public class TaskFileUnit
    {
        public class Constructor : UnitBase
        {
            [Fact(DisplayName = "Should define absolute and virtual paths from task item")]
            public void Spec01()
            {
                var taskItem = ConstructorHelper.CreateTaskItem(@"path\file.js");
                var actual = new TaskFile(@"D:\solution\source\project", taskItem);

                I.Expect(actual.Absolute).ToBe(@"D:\solution\source\project\path\file.js");
                I.Expect(actual.Virtual).ToBe(@"\path\file.js");
            }

            [Fact(DisplayName = "Should define absolute and virtual paths from linked task item")]
            public void Spec02()
            {
                var taskItem = ConstructorHelper.CreateTaskItem(@"..\..\packages\library\file.js", @"path\file.js");
                var actual = new TaskFile(@"D:\solution\source\project", taskItem);

                I.Expect(actual.Absolute).ToBe(@"D:\solution\source\project\..\..\packages\library\file.js");
                I.Expect(actual.Virtual).ToBe(@"\path\file.js");
            }

            [Fact(DisplayName = "Should define absolute and virtual paths from relative path")]
            public void Spec03()
            {
                var actual = new TaskFile(@"D:\solution\source\project", @"path\file.js");

                I.Expect(actual.Absolute).ToBe(@"D:\solution\source\project\path\file.js");
                I.Expect(actual.Virtual).ToBe(@"\path\file.js");
            }

            [Fact(DisplayName = "Should define absolute and virtual paths from absolute path")]
            public void Spec04()
            {
                var actual = new TaskFile(@"D:\solution\source\project", @"D:\solution\source\project\deep\path\file.js");

                I.Expect(actual.Absolute).ToBe(@"D:\solution\source\project\deep\path\file.js");
                I.Expect(actual.Virtual).ToBe(@"\deep\path\file.js");
            }

            private static class ConstructorHelper
            {
                public static ITaskItem CreateTaskItem(string file)
                {
                    return CreateTaskItem(file, null);
                }

                public static ITaskItem CreateTaskItem(string file, string link)
                {
                    return Mock.Of<ITaskItem>(x => x.ItemSpec == file && x.GetMetadata("Link") == link);
                }
            }
        }

        public class IsIgnored : UnitBase
        {
            [Fact(DisplayName = "Should ignore with a one level deep match")]
            public void Spec01()
            {
                using (var testable = new IsIgnoredTestable())
                {
                    testable.Ignored.Add(@"\path\");

                    var actual = testable.Instance.IsIgnored(testable.Ignored);

                    I.Expect(actual).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should ignore with a two level deep match")]
            public void Spec02()
            {
                using (var testable = new IsIgnoredTestable())
                {
                    testable.Ignored.Add(@"\path\to\");

                    var actual = testable.Instance.IsIgnored(testable.Ignored);

                    I.Expect(actual).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should not ignore with multiple non matches")]
            public void Spec03()
            {
                using (var testable = new IsIgnoredTestable())
                {
                    testable.Ignored.Add(@"path\to");
                    testable.Ignored.Add(@"to\path");
                    testable.Ignored.Add(@"\to\");
                    testable.Ignored.Add(@"\path\from\");

                    var actual = testable.Instance.IsIgnored(testable.Ignored);

                    I.Expect(actual).ToBeFalse();
                }
            }

            private class IsIgnoredTestable : MockTestFixture<TaskFile>
            {
                public IsIgnoredTestable()
                {
                    this.Ignored = new List<string>();
                }

                public List<string> Ignored { get; set; }

                protected override TaskFile Resolve()
                {
                    return new TaskFile(@"D:\solution\source\project", @"path\to\file.js");
                }
            }
        }
    }
}
