namespace JSLintNet.VisualStudio.EventControllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using EnvDTE;
    using JSLintNet.UI.ViewModels;
    using JSLintNet.UI.Views;
    using JSLintNet.VisualStudio.Errors;
    using Microsoft.VisualStudio.Shell;

    internal class MenuEventController : EventControllerBase
    {
        private IViewFactory viewFactory;

        private IMenuCommandService menuService;

        public MenuEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider)
            : this(serviceProvider, errorListProvider, new VisualStudioJSLintProvider(serviceProvider, errorListProvider), new ViewFactory())
        {
        }

        public MenuEventController(IServiceProvider serviceProvider, IJSLintErrorListProvider errorListProvider, IVisualStudioJSLintProvider visualStudioJSLintProvider, IViewFactory viewFactory)
            : base(serviceProvider, errorListProvider, visualStudioJSLintProvider)
        {
            this.viewFactory = viewFactory;
            this.menuService = serviceProvider.GetService<IMenuCommandService>();
        }

        public override void Initialize()
        {
            this.AddCommand(JSLintCommands.ErrorListClear, this.OnErrorListClear, this.OnBeforeErrorListClear);
            this.AddCommand(JSLintCommands.ItemNodeRun, this.OnItemNodeRun, this.OnBeforeItemNodeRun);
            this.AddCommand(JSLintCommands.ItemNodeIgnore, this.OnItemNodeIgnore, this.OnBeforeItemNodeIgnore);
            this.AddCommand(JSLintCommands.FolderNodeRun, this.OnFolderNodeRun, this.OnBeforeFolderNodeRun);
            this.AddCommand(JSLintCommands.FolderNodeIgnore, this.OnFolderNodeIgnore, this.OnBeforeFolderNodeIgnore);
            this.AddCommand(JSLintCommands.ProjectNodeRun, this.OnProjectNodeRun);
            this.AddCommand(JSLintCommands.ProjectNodeSettings, this.OnProjectNodeSettings);
            this.AddCommand(JSLintCommands.CodeWindowRun, this.OnCodeWindowRun, this.OnBeforeCodeWindowRun);
        }

        private void AddCommand(CommandID commandId, EventHandler invokeHandler)
        {
            this.AddCommand(commandId, invokeHandler, null);
        }

        private void AddCommand(CommandID commandId, EventHandler invokeHandler, EventHandler beforeHandler)
        {
            this.menuService.AddCommand(new OleMenuCommand(invokeHandler, null, beforeHandler, commandId));
        }

        private void SetIgnoreMenuItemState(OleMenuCommand menuCommand, SelectedItems selections)
        {
            if (menuCommand.Visible)
            {
                var projectItem = selections.Item(1).ProjectItem;
                var settings = this.VisualStudioJSLintProvider.LoadSettings(projectItem.ContainingProject);
                var ignored = settings.NormalizeIgnore();
                var ignoreState = projectItem.GetIgnoreState(ignored);

                menuCommand.Checked = ignoreState != IgnoreState.None;
                menuCommand.Enabled = ignoreState != IgnoreState.Implicit;
            }
        }

        private void ToggleIgnoreSelectedItem()
        {
            var projectItem = this.Environment.SelectedItems.Item(1).ProjectItem;
            var relativePath = projectItem.GetRelativePath();
            var project = projectItem.ContainingProject;

            var settings = this.VisualStudioJSLintProvider.LoadSettings(project);
            var ignored = settings.NormalizeIgnore() as List<string>;

            var index = ignored.FindIndex(x => relativePath.Equals(x, StringComparison.OrdinalIgnoreCase));

            if (index >= 0)
            {
                settings.Ignore.RemoveAt(index);
            }
            else
            {
                settings.Ignore.Add(relativePath);
            }

            this.VisualStudioJSLintProvider.SaveSettings(project, settings);
        }

        private void OnBeforeErrorListClear(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;

            menuCommand.Visible = this.ErrorListProvider.ErrorCount > 0;
        }

        private void OnErrorListClear(object sender, EventArgs e)
        {
            this.ErrorListProvider.ClearAllErrors();
        }

        private void OnBeforeItemNodeRun(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;

            menuCommand.Visible = this.IsSelectionLintable();
        }

        private void OnItemNodeRun(object sender, EventArgs e)
        {
            var projectItems = this.SelectedLintables();

            this.VisualStudioJSLintProvider.LintProjectItems(projectItems);
        }

        private void OnBeforeItemNodeIgnore(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;
            var selections = this.Environment.SelectedItems;

            menuCommand.Visible = selections.Count == 1 && JSLint.CanLint(selections.Item(1).Name);

            this.SetIgnoreMenuItemState(menuCommand, selections);
        }

        private void OnItemNodeIgnore(object sender, EventArgs e)
        {
            this.ToggleIgnoreSelectedItem();
        }

        private void OnBeforeFolderNodeRun(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;
            var selections = this.Environment.SelectedItems;

            menuCommand.Visible = !selections.MultiSelect;
        }

        private void OnFolderNodeRun(object sender, EventArgs e)
        {
            var projectItem = this.Environment.SelectedItems.Item(1).ProjectItem;
            var project = projectItem.ContainingProject;
            var settings = this.VisualStudioJSLintProvider.LoadSettings(project);
            var ignored = settings.NormalizeIgnore();
            var items = projectItem.ProjectItems.FindLintable(ignored);

            this.VisualStudioJSLintProvider.LintProjectItems(items, settings);
        }

        private void OnBeforeFolderNodeIgnore(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;
            var selections = this.Environment.SelectedItems;

            menuCommand.Visible = selections.Count == 1;

            this.SetIgnoreMenuItemState(menuCommand, selections);
        }

        private void OnFolderNodeIgnore(object sender, EventArgs e)
        {
            this.ToggleIgnoreSelectedItem();
        }

        private void OnProjectNodeRun(object sender, EventArgs e)
        {
            var project = this.Environment.SelectedItems.Item(1).Project;
            var settings = this.VisualStudioJSLintProvider.LoadSettings(project);
            var ignored = settings.NormalizeIgnore();
            var items = project.ProjectItems.FindLintable(ignored);

            this.VisualStudioJSLintProvider.LintProjectItems(items, settings);
        }

        private void OnProjectNodeSettings(object sender, EventArgs e)
        {
            var project = this.Environment.SelectedItems.Item(1).Project;
            var model = this.VisualStudioJSLintProvider.LoadSettings(project, false);

            using (var viewModel = new SettingsViewModel(model))
            {
                var view = this.viewFactory.CreateSettings(viewModel);
                var result = view.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    this.VisualStudioJSLintProvider.SaveSettings(project, model);
                }
            }
        }

        private void OnBeforeCodeWindowRun(object sender, EventArgs e)
        {
            var menuCommand = (OleMenuCommand)sender;
            var document = this.Environment.ActiveDocument;

            menuCommand.Visible = document != null && JSLint.CanLint(document.Name);
        }

        private void OnCodeWindowRun(object sender, EventArgs e)
        {
            var document = this.Environment.ActiveDocument;

            if (document != null && JSLint.CanLint(document.Name))
            {
                this.VisualStudioJSLintProvider.LintDocument(document);
            }
        }

        private IList<ProjectItem> SelectedLintables()
        {
            var selectedItems = this.Environment.SelectedItems;
            var projectItems = new List<ProjectItem>();

            foreach (SelectedItem selectedItem in selectedItems)
            {
                if (JSLint.CanLint(selectedItem.Name))
                {
                    var projectItem = selectedItem.ProjectItem;

                    if (projectItem != null)
                    {
                        projectItems.Add(projectItem);
                    }
                }
            }

            return projectItems;
        }

        private bool IsSelectionLintable()
        {
            var selectedItems = this.Environment.SelectedItems;

            return selectedItems.Count > 0 &&
                selectedItems
                    .OfType<SelectedItem>()
                    .All(x => JSLint.CanLint(x.Name));
        }
    }
}
