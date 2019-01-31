using System.IO;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\");
            Assert.Pass();
        }
    }
}