namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using System;
    using EnvDTE;
    using Moq;

    public class PropertiesFake : EnumerableFake<Property>, Properties
    {
        public object Application { get; set; }

        public object Parent { get; set; }

        public Mock<Property> AddProperty(string key, object value)
        {
            var mock = new Mock<Property>();

            mock
                .SetupGet(x => x.Name)
                .Returns(key);

            mock
                .SetupGet(x => x.Value)
                .Returns(value);

            this.AddItem(key, mock.Object);

            return mock;
        }

        public Mock<Property> AddProperty(string key, Func<object> valueFunction)
        {
            var mock = new Mock<Property>();

            mock
                .SetupGet(x => x.Name)
                .Returns(key);

            mock
                .SetupGet(x => x.Value)
                .Returns(valueFunction);

            this.AddItem(key, mock.Object);

            return mock;
        }
    }
}
