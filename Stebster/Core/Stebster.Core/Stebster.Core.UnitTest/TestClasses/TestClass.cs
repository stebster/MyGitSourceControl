namespace Stebster.Core.UnitTest.TestClasses
{
    using System;

    /// <summary>
    ///     Class for testing extension methods for objects and XElements
    ///     
    ///     NB this class is marked as serialisable and supposes IEquatable of T
    /// </summary>
    [Serializable]
    public class TestClass : IEquatable<TestClass>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public bool Equals(TestClass other)
        {
            return Id == other.Id;
        }
    }
}
