using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTools.XmlConfig;
using SparkyTestHelpers.Scenarios;
using SparkyTools.XmlConfig.UnitTests.TestClasses;
using System.Linq;
using SparkyTestHelpers.Exceptions;
using System.Configuration;

namespace SparkyTools.XmlConfig.UnitTests
{
    [TestClass]
    public class ConfigurationSectionListDeserializerTests
    {
        [TestMethod]
        public void List_Load_should_work()
        {
            IList<Bar> bars = ConfigurationSectionListDeserializer.Load<Bar>("Bars");
            AssertBarList(bars);
        }

        [TestMethod]
        public void List_Load_should_not_throw_ConfigurationErrorsException_when_section_is_not_found_and_shouldThrowExceptionIfSectionNotFound_is_false()
        {
            IList<Bar> bars =
                ConfigurationSectionListDeserializer.Load<Bar>(
                    "Barx", 
                    shouldThrowExceptionIfSectionNotFound: false,
                    shouldAllowEmptyList: true);

            Assert.IsNull(bars);
        }

        [TestMethod]
        public void List_Load_should_throw_ConfigurationErrorsException_when_section_is_not_found_and_shouldThrowExceptionIfSectionNotFound_is_true()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessage("Error loading from the \"Barx\" section of the .config file:"
                    + " Section not found.")
                .WhenExecuting(() =>
                    ConfigurationSectionListDeserializer.Load<Bar>("Barx", shouldThrowExceptionIfSectionNotFound: true));
        }

        [TestMethod]
        public void List_Load_should_not_throw_exception_for_empty_list_when_shouldAllowEmptyList_is_true()
        {
            IList<Bar> bars = ConfigurationSectionListDeserializer.Load<Bar>("EmptyBars", shouldAllowEmptyList: true);
            Assert.AreEqual(0, bars.Count);
        }

        [TestMethod]
        public void List_Load_should_throw_exception_for_empty_list_when_shouldAllowEmptyList_is_false()
        {
            AssertExceptionThrown
                .OfType<ConfigurationErrorsException>()
                .WithMessage("Error loading from the \"EmptyBars\" section of the .config file: Item count = zero.")
                .WhenExecuting(() => 
                    ConfigurationSectionListDeserializer.Load<Bar>("EmptyBars", shouldAllowEmptyList: false));
        }

        [TestMethod]
        public void DependencyProvider_method_should_work()
        {
            var provider = ConfigurationSectionListDeserializer.DependencyProvider<Bar>("Bars");
            AssertBarList(provider.GetValue());
        }

        private void AssertBarList(IList<Bar> bars)
        {
            Assert.AreEqual(6, bars.Count);

            Enumerable.Range(0, bars.Count).TestEach(index =>
            {
                AssertBarProperties(bars.ElementAt(index), index);
            });
        }

        private static void AssertBarProperties(Bar bar, int index)
        {
            Assert.IsNotNull(bar);
            Assert.AreEqual($"Quuz{index}", bar.Quuz);
            Assert.AreEqual(index, bar.Corge);
            Assert.AreEqual(index, bar.Grault);
            Assert.AreEqual((StringComparison)index, bar.Garply);
        }
    }
}
