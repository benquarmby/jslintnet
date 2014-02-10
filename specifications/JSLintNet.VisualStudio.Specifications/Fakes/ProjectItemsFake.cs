namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using System;
    using System.IO;
    using EnvDTE;
    using Moq;

    public class ProjectItemsFake : EnumerableFake<ProjectItem>, ProjectItems
    {
        public ProjectItemsFake()
        {
        }

        public ProjectItemsFake(Project containingProject)
        {
            this.ContainingProject = containingProject;
        }

        public string Kind { get; set; }

        public object Parent { get; set; }

        public Project ContainingProject { get; set; }

        public Mock<ProjectItem> AddProjectItem(string fileName)
        {
            return this.AddProjectItem(fileName, false);
        }

        public Mock<ProjectItem> AddProjectItem(string fileName, bool isLink)
        {
            var mock = new Mock<ProjectItem>();
            var properties = new PropertiesFake();

            properties.AddProperty("IsLink", isLink);

            mock
                .SetupGet(x => x.Name)
                .Returns(Path.GetFileName(fileName));

            mock
                .SetupGet(x => x.ContainingProject)
                .Returns(() => this.ContainingProject);

            mock
                .Setup(x => x.get_FileNames(It.IsAny<short>()))
                .Returns(fileName);

            mock
                .SetupGet(x => x.Properties)
                .Returns(properties);

            this.AddItem(mock.Object);

            return mock;
        }

        #region Not Implemented

        public ProjectItem AddFolder(string name, string kind = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}")
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromFileCopy(string filePath)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromTemplate(string fileName, string name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
