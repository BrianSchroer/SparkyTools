using AutoMapper;
using System;
using System.Text;

namespace SparkyTools.AutoMapper
{
    /// <summary>
    /// The "Assert" methods in this class assert mapper configuration validity with improved exception message formatting.
    /// </summary>
    public static class AutoMapperConfigurationValidity
    {
        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert()
        {
            WithExceptionHelper(Mapper.AssertConfigurationIsValid);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IMapper mapper, string profileName = null)
        {
            Assert(mapper.ConfigurationProvider, profileName);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IMapper mapper, TypeMap typeMap)
        {
            Assert(mapper.ConfigurationProvider, typeMap);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert<TProfile>(IMapper mapper) where TProfile : Profile, new()
        {
            Assert<TProfile>(mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IConfigurationProvider cfg, string profileName = null)
        {
            WithExceptionHelper(() =>
            {
                if (profileName == null)
                {
                    cfg.AssertConfigurationIsValid();
                }
                else
                {
                    cfg.AssertConfigurationIsValid(profileName);
                }
            });
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert(IConfigurationProvider cfg, TypeMap typeMap)
        {
            WithExceptionHelper(() => cfg.AssertConfigurationIsValid(typeMap));
        }

        /// <summary>
        /// Dry run all configured type maps and throw <see cref="AutoMapperConfigurationException"/> for each problem.
        /// </summary>
        public static void Assert<TProfile>(IConfigurationProvider cfg) where TProfile : Profile, new()
        {
            WithExceptionHelper(() => cfg.AssertConfigurationIsValid<TProfile>());
        }

        private static void WithExceptionHelper(Action assertConfigurationIsValid)
        {
            try
            {
                assertConfigurationIsValid();
            }
            catch (AutoMapperConfigurationException ex)
            {
                string improvedMessage = ImproveExceptionMessage(ex);
                if (improvedMessage == ex.Message)
                {
                    throw;
                }
                throw new AutoMapperConfigurationException(improvedMessage);
            }
        }

        private static string ImproveExceptionMessage(AutoMapperConfigurationException ex)
        {
            string message = ex.Message;

            try
            {

                var sb = new StringBuilder();

                foreach (AutoMapperConfigurationException.TypeMapConfigErrors errors in ex.Errors)
                {
                    string[] unmappedPropertyNames = errors.UnmappedPropertyNames;
                    if (unmappedPropertyNames.Length > 0)
                    {
                        TypeMap typeMap = errors.TypeMap;
                        string typeString = $"<{typeMap.SourceType.Name}, {typeMap.DestinationType.Name}>";

                        sb.AppendLine(
                            $"\n\tcfg.CreateMap<{typeMap.SourceType.Name}, {typeMap.DestinationType.Name}>()\n\n"
                            + $"\t\t.IgnoreMember{(unmappedPropertyNames.Length == 1 ? "" : "s")}(dest => dest."
                            + string.Join(", dest => dest.", unmappedPropertyNames)
                            + ");");
                    }
                }

                if (sb.Length > 0)
                {
                    string separator = new string('_', 100);
                    sb.Insert(0,
                        $"\n{separator}\nIf you want to ignore the properties shown above, you can cut/paste the following code stub(s):\n");
                    message = ex.Message + sb;
                }
            }
            catch
            {
            }

            return message;
        }
    }
}
