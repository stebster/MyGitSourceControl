namespace Stebster.Core.UnitTest
{
    using System;
    using System.Xml.Linq;
    using Extension;
    using NUnit.Framework;
    using TestClasses;

    [TestFixture]
    public class ExtentionTests
    {
        #region Private Members

        private TestClass _serialisableClass;

        #endregion

        #region Setup/TearDown

        [SetUp]
        public void SetUp()
        {
            _serialisableClass = new TestClass
            {
                Id = 1,
                Description = "Test Class",
                Start = new DateTime(1968, 02, 16)
            };
        }

        [TearDown]
        public void TearDown()
        {

        }

        #endregion

        #region Tests

        [Test]
        public void TestObjectToXelementExtensions()
        {
            var xelement =  _serialisableClass.ToXElement<TestClass>();

            Assert.IsInstanceOf<XElement>(xelement);

            var backToObject = xelement.ToObjectOfType<TestClass>();

            Assert.IsInstanceOf<TestClass>(backToObject);
        }

        #endregion
    }
}
