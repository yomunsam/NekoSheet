using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NekoSheet.Test
{
    public class ParseXLSXTest
    {
        private string path;
        [SetUp]
        public void Setup()
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), "NekoSheet_Tpl.xlsx");
        }

        [Test]
        public void GetDefaultJsonValueTest()
        {
            TestContext.WriteLine("Path:" + path);

            var nksheet = new NekoSheet(path);
            var json = nksheet.CovertToJson(0, false);

            TestContext.WriteLine(json);
            Assert.Pass();
        }
    }
}
