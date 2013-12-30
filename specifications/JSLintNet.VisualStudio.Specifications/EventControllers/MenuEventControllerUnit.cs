namespace JSLintNet.VisualStudio.Specifications.EventControllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using EnvDTE;
    using EnvDTE80;
    using JSLintNet.QualityTools;
    using JSLintNet.QualityTools.Expectations;
    using JSLintNet.QualityTools.Helpers;
    using JSLintNet.VisualStudio.Errors;
    using JSLintNet.VisualStudio.EventControllers;
    using JSLintNet.VisualStudio.Specifications.Fakes;
    using Microsoft.VisualStudio.Shell;
    using Moq;
    using Xunit;

    public class MenuEventControllerUnit
    {
        public class OnBeforeErrorListClear : UnitBase
        {
            [Fact(DisplayName = "Should set to hidden when no errors exist")]
            public void Spec01()
            {
                using (var testable = new OnBeforeErrorListClearTestable())
                {
                    testable.Init();
                    testable.BeforeHandler(testable.MenuCommand, null);

                    testable.GetMock<IJSLintErrorListProvider>().VerifyGet(x => x.ErrorCount);
                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should set to visible when errors exist")]
            public void Spec02()
            {
                using (var testable = new OnBeforeErrorListClearTestable())
                {
                    testable.GetMock<IJSLintErrorListProvider>()
                        .SetupGet(x => x.ErrorCount)
                        .Returns(10);

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    testable.VerifyGet<IJSLintErrorListProvider, int>(x => x.ErrorCount);
                    I.Expect(testable.MenuCommand.Visible).ToBeTrue();
                }
            }

            private class OnBeforeErrorListClearTestable : MenuEventControllerTestableBase
            {
                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.ErrorListClear;
                    }
                }
            }
        }

        public class OnErrorListClear : UnitBase
        {
            [Fact(DisplayName = "Should clear all tasks")]
            public void Spec01()
            {
                using (var testable = new OnErrorListClearTestable())
                {
                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    testable.Verify<IJSLintErrorListProvider>(x => x.ClearAllErrors());
                }
            }

            private class OnErrorListClearTestable : MenuEventControllerTestableBase
            {
                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.ErrorListClear;
                    }
                }
            }
        }

        public class OnBeforeItemNodeRun : UnitBase
        {
            [Fact(DisplayName = "Should set to hidden when selection contains any unlintables")]
            public void Spec01()
            {
                using (var testable = new OnBeforeItemNodeRunTestable())
                {
                    testable.AddSelectedItem("lintable.js");
                    testable.AddSelectedItem("notlintable.txt");
                    testable.AddSelectedItem("lintable.json");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should set to hidden when selection is empty")]
            public void Spec02()
            {
                using (var testable = new OnBeforeItemNodeRunTestable())
                {
                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should set to visible when selection only contains lintables")]
            public void Spec03()
            {
                using (var testable = new OnBeforeItemNodeRunTestable())
                {
                    testable.AddSelectedItem("lintable.js");
                    testable.AddSelectedItem("lintable.json");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeTrue();
                }
            }

            private class OnBeforeItemNodeRunTestable : ItemNodeTestableBase
            {
                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.ItemNodeRun;
                    }
                }
            }
        }

        public class OnItemNodeRun : UnitBase
        {
            [Fact(DisplayName = "Should process all selected lintables")]
            public void Spec01()
            {
                using (var testable = new OnItemNodeRunTestable())
                {
                    testable.AddSelectedItem("lintable.js");
                    testable.AddSelectedItem("notlintable.txt");
                    testable.AddSelectedItem("lintable.json");

                    IList<ProjectItem> projectItems = null;

                    testable.GetMock<IVisualStudioJSLintProvider>()
                        .Setup(x => x.LintProjectItems(It.IsAny<IList<ProjectItem>>()))
                        .Callback((IList<ProjectItem> x) => projectItems = x);

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(projectItems).Not.ToBeNull();
                    I.Expect(projectItems.Count).ToBe(2);
                }
            }

            private class OnItemNodeRunTestable : ItemNodeTestableBase
            {
                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.ItemNodeRun;
                    }
                }
            }
        }

        public class OnBeforeCodeWindowRun : UnitBase
        {
            [Fact(DisplayName = "Should set to hidden when active document is unlintable")]
            public void Spec01()
            {
                using (var testable = new OnBeforeCodeWindowRunTestable())
                {
                    testable.DocumentName = "unlintable.txt";

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    testable.EnvironmentMock.VerifyGet(x => x.ActiveDocument);
                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should set to visible when active document is lintable")]
            public void Spec02()
            {
                using (var testable = new OnBeforeCodeWindowRunTestable())
                {
                    testable.DocumentName = "lintable.json";

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    testable.EnvironmentMock.VerifyGet(x => x.ActiveDocument);
                    I.Expect(testable.MenuCommand.Visible).ToBeTrue();
                }
            }

            private class OnBeforeCodeWindowRunTestable : CodeWindowRunTestableBase
            {
            }
        }

        public class OnCodeWindowRun : UnitBase
        {
            [Fact(DisplayName = "Should not process active document if unlintable")]
            public void Spec01()
            {
                using (var testable = new OnCodeWindowRunTestable())
                {
                    testable.DocumentName = "script.coffee";

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    testable.EnvironmentMock.VerifyGet(x => x.ActiveDocument);
                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LintDocument(testable.DocumentMock.Object), Times.Never());
                }
            }

            [Fact(DisplayName = "Should process active document if lintable")]
            public void Spec02()
            {
                using (var testable = new OnCodeWindowRunTestable())
                {
                    testable.DocumentName = "script.js";

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    testable.EnvironmentMock.VerifyGet(x => x.ActiveDocument);
                    testable.Verify<IVisualStudioJSLintProvider>(x => x.LintDocument(testable.DocumentMock.Object));
                }
            }

            private class OnCodeWindowRunTestable : CodeWindowRunTestableBase
            {
            }
        }

        public class OnBeforeItemNodeIgnore : UnitBase
        {
            [Fact(DisplayName = "Should hide with multiple selections")]
            public void Spec01()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\file.js");
                    testable.AddSelectedItem(@"data.json");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should hide with no selections")]
            public void Spec02()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should hide with one non lintable")]
            public void Spec03()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\non.lintable");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should show with one lintable")]
            public void Spec04()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\file.js");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should check and disable when lintable is implicitly ignored")]
            public void Spec05()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\file.js");
                    testable.Settings.Ignore.Add(@"some");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Enabled).ToBeFalse();
                    I.Expect(testable.MenuCommand.Checked).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should check and enable when lintable is explictly ignored")]
            public void Spec06()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\file.js");
                    testable.Settings.Ignore.Add(@"some\path\to\file.js");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Enabled).ToBeTrue();
                    I.Expect(testable.MenuCommand.Checked).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should enable but not check when lintable is not ignored")]
            public void Spec07()
            {
                using (var testable = new OnBeforeItemNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\file.js");
                    testable.Settings.Ignore.Add(@"other\path");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Enabled).ToBeTrue();
                    I.Expect(testable.MenuCommand.Checked).ToBeFalse();
                }
            }

            private class OnBeforeItemNodeIgnoreTestable : ItemNodeTestableBase
            {
                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.ItemNodeIgnore;
                    }
                }
            }
        }

        public class OnItemNodeIgnore : UnitBase
        {
            [Fact(DisplayName = "Should remove same path with different case")]
            public void Spec01()
            {
                using (var testable = new OnItemNodeIgnoreTestable())
                {
                    testable.Settings.Ignore.Add(@"SOME\PATH\TO\FILE.JS");

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(testable.Settings.Ignore).ToBeEmpty();
                }
            }

            [Fact(DisplayName = "Should remove same path with different separators")]
            public void Spec02()
            {
                using (var testable = new OnItemNodeIgnoreTestable())
                {
                    testable.Settings.Ignore.Add(@"/SOME/PATH/TO/FILE.JS");

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(testable.Settings.Ignore).ToBeEmpty();
                }
            }

            [Fact(DisplayName = "Should add new ignore path")]
            public void Spec03()
            {
                using (var testable = new OnItemNodeIgnoreTestable())
                {
                    testable.Settings.Ignore.Add(@"/SOME/PATH/");

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(testable.Settings.Ignore).ToContain(@"\some\path\to\file.js");
                }
            }

            [Fact(DisplayName = "Should always save changes")]
            public void Spec04()
            {
                using (var testable = new OnItemNodeIgnoreTestable())
                {
                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.SaveSettings(testable.ProjectMock.Object, testable.Settings));
                }
            }

            private class OnItemNodeIgnoreTestable : ItemNodeTestableBase
            {
                public OnItemNodeIgnoreTestable()
                {
                    this.BeforeInit += this.OnBeforeInit;
                }

                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.ItemNodeIgnore;
                    }
                }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.AddSelectedItem(@"some\path\to\file.js");
                }
            }
        }

        public class OnBeforeFolderNodeIgnore : UnitBase
        {
            [Fact(DisplayName = "Should hide with multiple selections")]
            public void Spec01()
            {
                using (var testable = new OnBeforeFolderNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\");
                    testable.AddSelectedItem(@"data\");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should hide with no selections")]
            public void Spec02()
            {
                using (var testable = new OnBeforeFolderNodeIgnoreTestable())
                {
                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeFalse();
                }
            }

            [Fact(DisplayName = "Should show with one selection")]
            public void Spec03()
            {
                using (var testable = new OnBeforeFolderNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Visible).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should check and disable when folder is implicitly ignored")]
            public void Spec04()
            {
                using (var testable = new OnBeforeFolderNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\");
                    testable.Settings.Ignore.Add(@"some");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Enabled).ToBeFalse();
                    I.Expect(testable.MenuCommand.Checked).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should check and enable when folder is explictly ignored")]
            public void Spec05()
            {
                using (var testable = new OnBeforeFolderNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\");
                    testable.Settings.Ignore.Add(@"some\path\to\");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Enabled).ToBeTrue();
                    I.Expect(testable.MenuCommand.Checked).ToBeTrue();
                }
            }

            [Fact(DisplayName = "Should enable but not check when folder is not ignored")]
            public void Spec06()
            {
                using (var testable = new OnBeforeFolderNodeIgnoreTestable())
                {
                    testable.AddSelectedItem(@"some\path\to\");
                    testable.Settings.Ignore.Add(@"other\path");

                    testable.Init();
                    testable.BeforeHandler.Invoke(testable.MenuCommand, null);

                    I.Expect(testable.MenuCommand.Enabled).ToBeTrue();
                    I.Expect(testable.MenuCommand.Checked).ToBeFalse();
                }
            }

            private class OnBeforeFolderNodeIgnoreTestable : ItemNodeTestableBase
            {
                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.FolderNodeIgnore;
                    }
                }
            }
        }

        public class OnFolderNodeIgnore : UnitBase
        {
            [Fact(DisplayName = "Should remove same path with different case")]
            public void Spec01()
            {
                using (var testable = new OnFolderNodeIgnoreTestable())
                {
                    testable.Settings.Ignore.Add(@"SOME\PATH\TO\");

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(testable.Settings.Ignore).Not.ToContain(@"SOME\PATH\TO\");
                }
            }

            [Fact(DisplayName = "Should remove same path with different separators")]
            public void Spec02()
            {
                using (var testable = new OnFolderNodeIgnoreTestable())
                {
                    testable.Settings.Ignore.Add(@"/SOME/PATH/TO/");

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(testable.Settings.Ignore).Not.ToContain(@"/SOME/PATH/TO/");
                }
            }

            [Fact(DisplayName = "Should add new ignore path")]
            public void Spec03()
            {
                using (var testable = new OnFolderNodeIgnoreTestable())
                {
                    testable.Settings.Ignore.Add(@"/SOME/PATH/");

                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    I.Expect(testable.Settings.Ignore).ToContain(@"\some\path\to\");
                }
            }

            [Fact(DisplayName = "Should always save changes")]
            public void Spec04()
            {
                using (var testable = new OnFolderNodeIgnoreTestable())
                {
                    testable.Init();
                    testable.MenuCommand.Invoke(testable.MenuCommand);

                    testable.Verify<IVisualStudioJSLintProvider>(x => x.SaveSettings(testable.ProjectMock.Object, testable.Settings));
                }
            }

            private class OnFolderNodeIgnoreTestable : ItemNodeTestableBase
            {
                public OnFolderNodeIgnoreTestable()
                {
                    this.BeforeInit += this.OnBeforeInit;
                }

                public override CommandID CommandID
                {
                    get
                    {
                        return JSLintCommands.FolderNodeIgnore;
                    }
                }

                private void OnBeforeInit(object sender, EventArgs e)
                {
                    this.AddSelectedItem(@"some\path\to\");
                }
            }
        }

        private abstract class MenuEventControllerTestableBase : EventControllerTestableBase<MenuEventController>
        {
            public MenuEventControllerTestableBase()
            {
                this.MenuCommandServiceMock = new Mock<IMenuCommandService>();

                this.BeforeInit += this.OnBeforeInit;

                this.AfterInit += this.OnAfterInit;
            }

            public Mock<IMenuCommandService> MenuCommandServiceMock { get; set; }

            public abstract CommandID CommandID { get; }

            public OleMenuCommand MenuCommand { get; set; }

            public EventHandler BeforeHandler { get; set; }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                this.GetMock<IServiceProvider>()
                    .Setup(x => x.GetService(typeof(IMenuCommandService)))
                    .Returns(this.MenuCommandServiceMock.Object);

                this.MenuCommandServiceMock
                    .Setup(x => x.AddCommand(It.Is<MenuCommand>(y => y.CommandID == this.CommandID)))
                    .Callback((MenuCommand x) => this.MenuCommand = (OleMenuCommand)x);
            }

            private void OnAfterInit(object sender, EventArgs e)
            {
                this.BeforeHandler = ReflectionHelper.GetFieldValue<EventHandler>(this.MenuCommand, "beforeQueryStatusHandler");
            }
        }

        private abstract class ItemNodeTestableBase : MenuEventControllerTestableBase
        {
            private List<UIHierarchyItem> selectedItems = new List<UIHierarchyItem>();

            public ItemNodeTestableBase()
            {
                this.ProjectMock = new Mock<Project>();
                this.ProjectFullName = @"c:\\full\path\to\project.csproj";
                this.Settings = new JSLintNetSettings();

                this.BeforeInit += this.OnBeforeInit;
            }

            internal JSLintNetSettings Settings { get; set; }

            internal Mock<Project> ProjectMock { get; set; }

            protected string ProjectFullName { get; set; }

            public Mock<UIHierarchyItem> AddSelectedItem(string fileName, bool isLink = false)
            {
                var hierarchyItemMock = new Mock<UIHierarchyItem>();
                var projectItemMock = new Mock<ProjectItem>();
                var name = Path.GetFileName(fileName);
                var propertiesFake = new PropertiesFake();
                var path = Path.GetDirectoryName(this.ProjectFullName);

                propertiesFake.AddProperty("IsLink", isLink);

                projectItemMock
                    .Setup(x => x.get_FileNames(0))
                    .Returns(Path.Combine(path, fileName));

                projectItemMock
                    .SetupGet(x => x.Name)
                    .Returns(name);

                projectItemMock
                    .SetupGet(x => x.Properties)
                    .Returns(propertiesFake);

                projectItemMock
                    .SetupGet(x => x.ContainingProject)
                    .Returns(this.ProjectMock.Object);

                hierarchyItemMock
                    .SetupGet(x => x.Name)
                    .Returns(name);

                hierarchyItemMock
                    .SetupGet(x => x.Object)
                    .Returns(projectItemMock.Object);

                this.selectedItems.Add(hierarchyItemMock.Object);

                return hierarchyItemMock;
            }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                var solutionExplorerWindowMock = new Mock<UIHierarchy>();
                var toolWindowsMock = new Mock<ToolWindows>();
                var propertiesFake = new PropertiesFake();

                propertiesFake.AddProperty("FullPath", Path.GetDirectoryName(this.ProjectFullName) + @"\");

                this.ProjectMock
                    .SetupGet(x => x.FullName)
                    .Returns(() => this.ProjectFullName);

                this.ProjectMock
                    .SetupGet(x => x.Properties)
                    .Returns(propertiesFake);

                solutionExplorerWindowMock
                    .SetupGet(x => x.SelectedItems)
                    .Returns(() => this.selectedItems.ToArray());

                toolWindowsMock
                    .Setup(x => x.GetToolWindow(Constants.vsWindowKindSolutionExplorer))
                    .Returns(solutionExplorerWindowMock.Object);

                this.EnvironmentMock
                    .SetupGet(x => x.ToolWindows)
                    .Returns(toolWindowsMock.Object);

                this.GetMock<IVisualStudioJSLintProvider>()
                    .Setup(x => x.LoadSettings(It.IsAny<Project>()))
                    .Returns(this.Settings);
            }
        }

        private abstract class CodeWindowRunTestableBase : MenuEventControllerTestableBase
        {
            public CodeWindowRunTestableBase()
            {
                this.DocumentMock = new Mock<Document>();

                this.BeforeInit += this.OnBeforeInit;
            }

            public Mock<Document> DocumentMock { get; set; }

            public string DocumentName { get; set; }

            public override CommandID CommandID
            {
                get
                {
                    return JSLintCommands.CodeWindowRun;
                }
            }

            private void OnBeforeInit(object sender, EventArgs e)
            {
                this.DocumentMock
                    .SetupGet(x => x.Name)
                    .Returns(() => this.DocumentName);

                this.EnvironmentMock
                    .SetupGet(x => x.ActiveDocument)
                    .Returns(this.DocumentMock.Object);
            }
        }
    }
}
