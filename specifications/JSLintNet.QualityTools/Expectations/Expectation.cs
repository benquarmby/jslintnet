namespace JSLintNet.QualityTools.Expectations
{
    /// <summary>
    /// An expectation that can be reversed to a negative state.
    /// </summary>
    /// <typeparam name="TActual">Any type.</typeparam>
    public class Expectation<TActual> : IReversibleExpectation<TActual>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation{TActual}" /> class.
        /// </summary>
        /// <param name="actual">The value.</param>
        public Expectation(TActual actual)
        {
            this.Actual = actual;
            this.Positive = true;
        }

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        /// <value>
        /// The actual value.
        /// </value>
        public TActual Actual { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Expectation{TActual}" /> is positive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if positive; otherwise, <c>false</c>.
        /// </value>
        public bool Positive { get; private set; }

        /// <summary>
        /// Gets the negative expectation.
        /// </summary>
        /// <value>
        /// The negative expectation.
        /// </value>
        public IExpectation<TActual> Not
        {
            get
            {
                this.Positive = false;
                return this;
            }
        }

        /// <summary>
        /// Matcher that determines whether the actual value is null.
        /// </summary>
        public void ToBeNull()
        {
            ExpectationHelper.PassFail(this.Actual == null, this, null);
        }

        /// <summary>
        /// Matcher that determines whether the actual value type is the same as the given type.
        /// </summary>
        /// <typeparam name="TExpected">The type to compare.</typeparam>
        public void ToBeOfType<TExpected>()
        {
            var expectedType = typeof(TExpected);

            ExpectationHelper.PassFail(this.Actual.GetType() == expectedType, this, expectedType.ToString());
        }
    }
}
