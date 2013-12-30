namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using EnvDTE;
    using Moq;

    public class DocumentMockContainer
    {
        public DocumentMockContainer(string source)
        {
            this.DocumentMock = new Mock<Document>();
            this.TextDocumentMock = new Mock<TextDocument>();
            this.StartPointMock = new Mock<TextPoint>();
            this.EndPointMock = new Mock<TextPoint>();
            this.EditPointMock = new Mock<EditPoint>();

            this.DocumentMock
                .Setup(x => x.Object("TextDocument"))
                .Returns(this.TextDocumentMock.Object);

            this.TextDocumentMock
                .SetupGet(x => x.StartPoint)
                .Returns(this.StartPointMock.Object);

            this.TextDocumentMock
                .SetupGet(x => x.EndPoint)
                .Returns(this.EndPointMock.Object);

            this.StartPointMock
                .Setup(x => x.CreateEditPoint())
                .Returns(this.EditPointMock.Object);

            this.EditPointMock
                .Setup(x => x.GetText(this.EndPointMock.Object))
                .Returns(source);
        }

        public Mock<Document> DocumentMock { get; set; }

        public Mock<TextDocument> TextDocumentMock { get; set; }

        public Mock<TextPoint> StartPointMock { get; set; }

        public Mock<TextPoint> EndPointMock { get; set; }

        public Mock<EditPoint> EditPointMock { get; set; }
    }
}
