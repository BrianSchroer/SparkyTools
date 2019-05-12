using System;
using System.Threading;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTestHelpers.AutoMapper;
using SparkyTestHelpers.Mapping;
using SparkyTools.AutoMapper;
using SparkyTools.AutoMapperV1.UnitTests.TestClasses;

namespace SparkyTools.AutoMapperV1.UnitTests
{
    [TestClass]
    public class AutoMapperToolsTests
    {
        private MapTester<Source, Dest> _mapTester;
        private Source _source;
        private RandomValuesHelper _randomValuesHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            _randomValuesHelper = new RandomValuesHelper();
            _source = _randomValuesHelper.CreateInstanceWithRandomValues<Source>();
            _mapTester = MapTester.ForMap<Source, Dest>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Mapper.Reset();
        }

        [TestMethod]
        public void MappedTo_should_map_to_destination_type()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = _source.MappedTo<Dest>();

            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void MappedTo_with_afterMap_should_work()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = _source.MappedTo<Dest>(d => d.Id = 666);

            Assert.AreEqual(dest.DateTime, _source.DateTime);
            Assert.AreEqual(dest.Id, 666);
        }

        [TestMethod]
        public void MappedTo_should_return_null_for_null_input()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            Source source = null;

            var dest = source.MappedTo<Dest>();

            Assert.IsNull(dest);
        }

        [TestMethod]
        public void MappedTo_with_IMapper_should_map_to_destination_type()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = _source.MappedTo<Dest>(mapper);

            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void MappedTo_with_IMapper_and_afterMap_should_work()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = _source.MappedTo<Dest>(mapper, d => d.Id = 666);

            Assert.AreEqual(dest.DateTime, _source.DateTime);
            Assert.AreEqual(dest.Id, 666);
        }

        [TestMethod]
        public void MappedTo_with_IMapper_should_return_null_for_null_input()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            Source source = null;

            var dest = source.MappedTo<Dest>(mapper);

            Assert.IsNull(dest);
        }

        [TestMethod]
        public void MappedToArrayOf_should_map_to_array_of_destination_type()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArrayOf<Dest>();

            Assert.AreEqual(3, dest.Length);
            _mapTester.IgnoringAllOtherMembers().AssertMappedValues(_source, dest[0]);
        }

        [TestMethod]
        public void MappedToArrayOf_with_afterMap_should_work()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArrayOf<Dest>(d => d.Id = 666);

            Assert.AreEqual(3, dest.Length);

            for (int i = 0; i < dest.Length; i++)
            {
                Assert.AreEqual(dest[i].DateTime, _source.DateTime);
                Assert.AreEqual(dest[i].Id, 666);
            }
        }

        [TestMethod]
        public void MappedToArray_with_afterMap_using_source_and_dest_should_work()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArray<Source, Dest>((s, d) => d.Id = s.Id + 1);

            Assert.AreEqual(3, dest.Length);

            for (int i = 0; i < dest.Length; i++)
            {
                Assert.AreEqual(dest[i].DateTime, _source.DateTime);
                Assert.AreEqual(dest[i].Id, _source.Id + 1);
            }
        }

        [TestMethod]
        public void MappedToArrayOf_should_return_empty_array_for_null_input()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            Source[] sourceArray = null;

            var dest = sourceArray.MappedToArrayOf<Dest>();

            Assert.AreEqual(0, dest.Length);
        }

        [TestMethod]
        public void MappedToArrayOf_with_IMapper_should_map_to_array_of_destination_type()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArrayOf<Dest>(mapper, d => d.Id = 666);

            Assert.AreEqual(3, dest.Length);

            for (int i = 0; i < dest.Length; i++)
            {
                Assert.AreEqual(dest[i].DateTime, _source.DateTime);
                Assert.AreEqual(dest[i].Id, 666);
            }
        }

        [TestMethod]
        public void MappedToArray_with_IMapper_and_afterMap_using_source_and_dest_should_work()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = (new[] { _source, _source, _source }).MappedToArray<Source, Dest>(mapper, (s, d) => d.Id = s.Id + 1);

            Assert.AreEqual(3, dest.Length);

            for (int i = 0; i < dest.Length; i++)
            {
                Assert.AreEqual(dest[i].DateTime, _source.DateTime);
                Assert.AreEqual(dest[i].Id, _source.Id + 1);
            }
        }

        [TestMethod]
        public void MappedToArrayOf_with_IMapper_should_return_empty_array_for_null_input()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            Source[] sourceArray = null;

            var dest = sourceArray.MappedToArrayOf<Dest>(mapper);

            Assert.AreEqual(0, dest.Length);
        }

        [TestMethod]
        public void CopyValuesFrom_should_copy_source_values_to_result_values()
        {
            CreateStaticMap(map => map.IgnoreMember(x => x.Address));

            var dest = _randomValuesHelper.CreateInstanceWithRandomValues<Dest>();

            var dest2 = dest.CopyValuesFrom(_source);

            Assert.AreSame(dest, dest2);

            _mapTester.IgnoringMember(d => d.Address).AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void CopyValuesFrom_should_copy_source_values_to_result_values_with_instance_mapper()
        {
            IMapper mapper = CreateInstanceMap(map => map.IgnoreMember(x => x.Address));

            var dest = _randomValuesHelper.CreateInstanceWithRandomValues<Dest>();

            var dest2 = dest.CopyValuesFrom(_source, mapper);

            Assert.AreSame(dest, dest2);

            _mapTester.IgnoringMember(d => d.Address).AssertMappedValues(_source, dest);
        }

        [TestMethod]
        public void IgnoreMember_should_ignore_specified_member()
        {
            CreateStaticMap(map => map
            .IgnoreMember(x => x.Name)
            .IgnoreMember(x => x.Address));

            Dest dest = _mapTester
                .IgnoringMember(x => x.Name)
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);

            Assert.IsNull(dest.Name);
        }

        [TestMethod]
        public void IgnoreMembers_should_work_as_expected()
        {
            CreateStaticMap(map => map.IgnoreMembers(x => x.Name, x => x.Address));

            Dest dest = _mapTester.IgnoringMembers(x => x.Name, x => x.Address).AssertAutoMappedValues(_source);

            Assert.IsNull(dest.Name);
        }

        [TestMethod]
        public void MapFrom_should_use_specified_source_callback()
        {
            CreateStaticMap(map => map
                .IgnoreMember(x => x.Address)
                .ForMember(x => x.Name).MapFrom(src => CombineNames(src)));

            _mapTester
                .WhereMember(x => x.Name).ShouldEqualValue(CombineNames(_source))
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void NullSubstitute_should_use_specified_substitute()
        {
            CreateStaticMap(map => map
                .ForMember(x => x.Name).NullSubstitute("Sparky")
                .IgnoreMember(x => x.Address));

            _source.Name = "Brian";
            _mapTester
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);

            _source.Name = null;
            Dest dest = _source.MappedTo<Dest>();
            Assert.AreEqual("Sparky", dest.Name);
            _mapTester
                .WhereMember(x => x.Name).ShouldEqualValue("Sparky")
                .IgnoringMember(x => x.Address)
                .AssertAutoMappedValues(_source);
        }

        [TestMethod]
        public void UseValue_should_call_specified_callback_function()
        {
            DateTime startTime = DateTime.Now;

            CreateStaticMap(map => map
                .ForMember(x => x.DateTime).UseValue(() => DateTime.Now)
                .IgnoreMember(x => x.Address));

            Dest dest = _source.MappedTo<Dest>();

            Assert.IsTrue(dest.DateTime > startTime);

            Dest dest2 = _source.MappedTo<Dest>();
            Assert.IsTrue(dest2.DateTime > dest.DateTime);
        }

        [TestMethod]
        public void UseValue_should_use_specified_value()
        {
            DateTime startTime = DateTime.Now;

            CreateStaticMap(map => map
                .ForMember(x => x.DateTime).UseValue(DateTime.Now)
                .IgnoreMember(x => x.Address));

            Dest dest = _source.MappedTo<Dest>();
            Assert.IsTrue(dest.DateTime > startTime);

            Thread.Sleep(100);

            Dest dest2 = _source.MappedTo<Dest>();
            Assert.IsTrue(dest2.DateTime > startTime);
            Assert.AreNotEqual(dest.DateTime, dest2.DateTime);
        }

        private string CombineNames(Source source)
        {
            return $"{source.FirstName} {source.LastName}";
        }

        private void CreateStaticMap(Action<IMappingExpression<Source, Dest>> callback = null)
        {
            Mapper.Initialize(cfg =>
            {
                IMappingExpression<Source, Dest> mappingExpression = cfg.CreateMap<Source, Dest>();
                callback?.Invoke(mappingExpression);
            });

            Mapper.AssertConfigurationIsValid();
        }

        private IMapper CreateInstanceMap(Action<IMappingExpression<Source, Dest>> callback = null)
        {
            var config = new MapperConfiguration(cfg => {
                IMappingExpression<Source, Dest> mappingExpression = cfg.CreateMap<Source, Dest>();
                callback?.Invoke(mappingExpression);
            });

            IMapper mapper = config.CreateMapper();

            return mapper;
        }
    }
}
