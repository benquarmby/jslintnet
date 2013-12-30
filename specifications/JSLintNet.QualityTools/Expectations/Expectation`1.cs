namespace JSLintNet.QualityTools.Expectations
{
    /// <summary>
    /// An expectation that can be reversed to a negative state.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public class Expectation<T> : IReversibleExpectation<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation{T}" /> class.
        /// </summary>
        /// <param name="actual">The value.</param>
        public Expectation(T actual)
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
        public T Actual { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Expectation{T}" /> is positive.
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
        public IExpectation<T> Not
        {
            get
            {
                this.Positive = false;
                return this;
            }
        }

        /// <summary>
        /// Matcher that determines whether the actual and expected values are equal.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public void ToBe(T expected)
        {
            var pass = this.Actual == null ? expected == null : this.Actual.Equals(expected);

            ExpectationHelper.PassFail(pass, this, expected);
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
        /// <typeparam name="S">The type to compare.</typeparam>
        public void ToBeOfType<S>()
        {
            var typeOfS = typeof(S);

            ExpectationHelper.PassFail(this.Actual.GetType() == typeOfS, this, typeOfS.ToString());
        }
    }
}
