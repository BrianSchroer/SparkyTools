using System;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTestHelpers.Mapping;
using SparkyTools.AutoMapperUpgrade.UnitTests.TestClasses;

namespace SparkyTools.AutoMapperUpgrade.UnitTests
{
    [TestClass]
    public class AutoMapperUpgradeTests
    {
        private RandomValuesHelper _randomValuesHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            _randomValuesHelper = new RandomValuesHelper();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Mapper.Reset();
        }

        [TestMethod]
        public void ConvertUsing_should_work_as_expected()
        {
            CreateStaticMap(cfg => cfg.CreateMap<Source, Dest>().ConvertUsing(src => new Dest
            {
                Id = src.Id,
                Name = src.FullName,
                Address = "Address"
            }));

            var input = _randomValuesHelper.CreateInstanceWithRandomValues<Source>();

            AssertSourceToDestMapping(input, Mapper.Map<Source, Dest>(input));
        }

        [TestMethod]
        public void UseValue_is_obsolete_with_v8()
        {
            //CreateStaticMap(cfg =>
            //{
            //    cfg.CreateMap<Source, Dest>()
            //        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
            //        .ForMember(dest => dest.Address, opt => opt.UseValue("Address"));
            //});

            CreateStaticMap(cfg =>
            {
                cfg.CreateMap<Source, Dest>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(_ => "Address"));
            });

            var input = _randomValuesHelper.CreateInstanceWithRandomValues<Source>();

            AssertSourceToDestMapping(input, Mapper.Map<Source, Dest>(input));
        }

        private void CreateStaticMap(Action<IMapperConfigurationExpression> callback = null)
        {
            Mapper.Initialize(cfg =>
            {
                callback?.Invoke(cfg);
            });

            Mapper.AssertConfigurationIsValid();
        }

        private void AssertSourceToDestMapping(Source sourceInstance, Dest destInstance)
        {
            MapTester.ForMap<Source, Dest>()
                .WhereMember(dest => dest.Id).ShouldEqual(src => src.Id)
                .WhereMember(dest => dest.Name).ShouldEqual(src => src.FullName)
                .WhereMember(dest => dest.Address).ShouldEqualValue("Address")
                .AssertMappedValues(sourceInstance, destInstance);
        }
    }
}