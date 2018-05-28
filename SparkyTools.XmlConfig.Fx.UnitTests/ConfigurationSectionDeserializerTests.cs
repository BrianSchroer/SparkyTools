using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTestHelpers.Exceptions;
using System;
using SparkyTools.XmlConfig.Fx.UnitTests.TestClasses;
using System.Configuration;

namespace SparkyTools.XmlConfig.Fx.UnitTests
{
    [TestClass]
    public class ConfigurationSectionDeserializerTests
    {
        [TestMethod]
        public void Load_should_work()
        {
            Bar bar = ConfigurationSectionDeserializer.Load<Bar>("Bar");
            AssertBarProperties(bar);
        }

        [TestMethod]
        public void Load_of_hierarchial_type_should_work()
        {
            Foo foo = ConfigurationSectionDeserializer.Load<Foo>("Foo");
            AssertFooProperties(foo);
        }

        [TestMethod]
        public void Load_should_work_when_section_name_doesnt_match_type_name()
        {
            Bar bar = ConfigurationSectionDeserializer.Load<Bar>("Bar2");
            AssertBarProperties(bar);
        }

        [TestMethod]
        public void Load_should_not_throw_ConfigurationErrorsException_when_section_is_not_found_and_shouldThrowExceptionIfSectionNotFound_is_false()
        {
            Bar bar =
                ConfigurationSectionDeserializer.Load<Bar>("Barx", shouldThrowExceptionIfSectionNotFound: false);

            Assert.IsNull(bar);
        }

        [TestMethod]
        public void Load_should_throw_ConfigurationErrorsException_when_section_is_not_found_and_shouldThrowExceptionIfSectionNotFound_is_true()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessage("Error loading from the \"Barx\" section of the .config file:"
                    + " Section not found.")
                .WhenExecuting(() =>
                    ConfigurationSectionDeserializer.Load<Bar>("Barx", shouldThrowExceptionIfSectionNotFound: true));
        }

        [TestMethod]
        public void Load_should_throw_ConfigurationErrorsException_when_section_type_attribute_is_missing()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessageContaining("Error loading from the \"Bar3\" section of the .config file:"
                    + " The \"type\" attribute is not present.")
                .WhenExecuting(() => ConfigurationSectionDeserializer.Load<Bar>("Bar3"));
        }

        [TestMethod]
        public void Load_should_throw_ConfigurationErrorsException_when_section_type_cannot_be_cast()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessageContaining("Error loading from the \"Bar4\" section of the .config file:"
                    + " Unable to cast object of type 'SparkyTools.XmlConfig.Fx.UnitTests.TestClasses.Foo' to type 'SparkyTools.XmlConfig.Fx.UnitTests.TestClasses.Bar'.")
                .WhenExecuting(() => ConfigurationSectionDeserializer.Load<Bar>("Bar4"));
        }

        [TestMethod]
        public void Load_should_throw_ConfigurationErrorsException_when_section_type_does_not_exist()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessageContaining("Error loading from the \"Bar5\" section of the .config file:"
                    + " \"SparkyTools.XmlConfig.Fx.UnitTests.TestClasses.Bark, SparkyTools.XmlConfig.Fx.UnitTests\" is not a recognized type name.")
                .WhenExecuting(() => ConfigurationSectionDeserializer.Load<Bar>("Bar5"));
        }

        [TestMethod]
        public void Load_should_throw_ConfigurationErrorsException_when_section_contains_XML_error()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessageContaining("Error loading from the \"Foo2\" section of the .config file:"
                    + " An error occurred creating the configuration section handler for Foo2: There is an error in XML document")
                .WhenExecuting(() => ConfigurationSectionDeserializer.Load<Bar>("Foo2"));
        }

        [TestMethod]
        public void DependencyProvider_method_should_work()
        {
            var provider = ConfigurationSectionDeserializer.DependencyProvider<Bar>("Bar");
            AssertBarProperties(provider.GetValue());
        }

        private static void AssertFooProperties(Foo foo)
        {
            AssertBarProperties(foo.Bar);
            Assert.AreEqual("baz from config", foo.Baz);
            Assert.AreEqual(666, foo.Qux);
            Assert.AreEqual("12/31/2018", foo.Quux.ToString("MM/dd/yyyy"));
        }

        private static void AssertBarProperties(Bar bar)
        {
            Assert.IsNotNull(bar);
            Assert.AreEqual("quuz from config", bar.Quuz);
            Assert.AreEqual(1212, bar.Corge);
            Assert.AreEqual(123.45, bar.Grault);
            Assert.AreEqual(StringComparison.CurrentCulture, bar.Garply);
        }
    }
}
