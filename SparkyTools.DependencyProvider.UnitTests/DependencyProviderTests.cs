using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTestHelpers.Exceptions;
using SparkyTestHelpers.Scenarios;
using System;
using System.Linq;

namespace SparkyTools.DependencyProvider.UnitTests
{
    [TestClass]
    public class DependencyProviderTests
    {
        [TestMethod]
        public void DependencyProvider_should_work_when_created_via_function_constructor()
        {
            int callCount = 0;

            var provider = new DependencyProvider<int>(() => 
            {
                callCount++;
                return callCount;
            });

            Enumerable.Range(1, 5).TestEach(scenarioCount => 
            {
                Assert.AreEqual(scenarioCount, provider.GetValue());
                Assert.AreEqual(scenarioCount, callCount);
            });
        }

        [TestMethod]
        public void DependencyProvider_should_work_when_created_via_value_constructor()
        {
            var provider = new DependencyProvider<int>(123);

            Enumerable.Range(1, 5).TestEach(scenarioCount => Assert.AreEqual(123, provider.GetValue()));
        }

        [TestMethod]
        public void DependencyProvider_should_work_when_created_via_Create_function_method()
        {
            int callCount = 0;

            var provider = DependencyProvider.Create(() =>
            {
                callCount++;
                return callCount;
            });

            Enumerable.Range(1, 5).TestEach(scenarioCount =>
            {
                Assert.AreEqual(scenarioCount, provider.GetValue());
                Assert.AreEqual(scenarioCount, callCount);
            });
        }

        [TestMethod]
        public void DependencyProvider_should_work_when_created_via_Create_value_method()
        {
            var provider = DependencyProvider.Create(123);

            Enumerable.Range(1, 5).TestEach(scenarioCount => Assert.AreEqual(123, provider.GetValue()));
        }

        [TestMethod]
        public void DependencyProvider_marked_Static_should_always_return_the_same_value()
        {
            int callCount = 0;

            var provider = DependencyProvider.Create(() =>
            {
                callCount++;
                return callCount;
            })
            .Static();

            Enumerable.Range(1, 5).TestEach(scenarioCount =>
            {
                Assert.AreEqual(1, provider.GetValue());
                Assert.AreEqual(1, callCount);
            });
        }

        [TestMethod]
        public void InvalidOperationException_should_be_thrown_if_Static_is_called_after_GetValue()
        {
            var provider = DependencyProvider.Create(() => "Sparky");

            Assert.AreEqual("Sparky", provider.GetValue());

            AssertExceptionThrown
                .OfType<InvalidOperationException>()
                .WithMessage("The Static() method cannot be called after GetValue() has been called.")
                .WhenExecuting(() => provider.Static());
        }

        [TestMethod]
        public void DependencyProvider_created_via_CreateStatic_should_always_return_the_same_value()
        {
            int callCount = 0;

            var provider = DependencyProvider.CreateStatic(() =>
            {
                callCount++;
                return callCount;
            });

            Enumerable.Range(1, 5).TestEach(scenarioCount =>
            {
                Assert.AreEqual(1, provider.GetValue());
                Assert.AreEqual(1, callCount);
            });
        }
   }
}
