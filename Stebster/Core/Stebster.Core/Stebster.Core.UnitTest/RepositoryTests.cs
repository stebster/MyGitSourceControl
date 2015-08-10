namespace Stebster.Core.UnitTest
{
    using System;
    using NUnit.Framework;
    using Repository;
    using TestClasses;

    [TestFixture]
    public class RepositoryTests
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
        public void TestXmlRepository()
        {
            var repository = new XmlRepository(@"c:\VideoLibrary\TestRepository");

            var save = repository.SaveItem(_serialisableClass);

            Assert.IsTrue(save);

            var get = repository.GetItem(_serialisableClass);

            Assert.AreEqual(get, _serialisableClass);

            var items = repository.ReadItems<TestClass>();

            Assert.IsTrue(items.Count > 0);

            var delete = repository.DeleteItem(_serialisableClass);

            Assert.IsTrue(delete);

            var missing = repository.GetItem(_serialisableClass);

            Assert.IsNull(missing);
        }

        #endregion
    }
}
