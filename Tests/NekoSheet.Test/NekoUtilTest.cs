using NUnit.Framework;
using NekoSheet.Utils;

namespace NekoSheet.Test
{
    public class NekoUtilTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetDefaultJsonValueTest()
        {
            //TestContext.WriteLine(NekoUtil.GetDefaultValue("test", Enums.FieldType.String, Enums.FieldType.String).ToString());
            //TestContext.WriteLine(NekoUtil.GetDefaultValue("test", Enums.FieldType.Number, Enums.FieldType.String).ToString());
            //TestContext.WriteLine(NekoUtil.GetDefaultValue("test", Enums.FieldType.Boolean, Enums.FieldType.String).ToString());
            //TestContext.WriteLine(NekoUtil.GetDefaultValue("test", Enums.FieldType.Array, Enums.FieldType.String).ToString());
            //TestContext.WriteLine(NekoUtil.GetDefaultValue("test", Enums.FieldType.Array, Enums.FieldType.Number).ToString());
            //TestContext.WriteLine(NekoUtil.GetDefaultValue("test", Enums.FieldType.Array, Enums.FieldType.Boolean).ToString());
            Assert.Pass();
        }
    }
}