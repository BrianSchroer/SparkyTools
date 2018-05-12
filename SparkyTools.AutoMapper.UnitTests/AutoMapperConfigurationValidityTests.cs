using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTestHelpers.Exceptions;
using SparkyTools.AutoMapper.UnitTests.TestClasses;
using System;

namespace SparkyTools.AutoMapper.UnitTests
{
    /// <summary>
    /// <see cref="AutoMapperConfigurationValidity"/> tests.
    /// </summary>
    [TestClass]
    public class AutoMapperConfigurationValidityTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            Mapper.Reset();
        }

        [TestMethod]
        public void AutoMapperConfigurationValidity_Assert_should_work_as_expected_with_IConfigurationProvider()
        {
            WithExceptionTest(() =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Source, BadDest>();
                    cfg.CreateMap<Dest, BadDest>();
                });

                AutoMapperConfigurationValidity.Assert(config);
            });
        }

        [TestMethod]
        public void AutoMapperConfigurationValidity_Assert_should_work_as_expected_with_IMapper()
        {
            WithExceptionTest(() =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Source, BadDest>();
                });

                AutoMapperConfigurationValidity.Assert(config.CreateMapper());
            });
        }

        [TestMethod]
        public void AutoMapperConfigurationValidity_Assert_should_work_as_expected_with_static_Mapper()
        {
            WithExceptionTest(() =>
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Source, BadDest>();
                });

                AutoMapperConfigurationValidity.Assert();
            });
        }

        private static void WithExceptionTest(Action assertConfigurationIsValid)
        {
            AssertExceptionThrown.OfType<AutoMapperConfigurationException>()
                .WithMessageContaining("If you want to ignore")
                .WhenExecuting(assertConfigurationIsValid);
        }
    }
}
