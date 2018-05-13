using AutoMapper;
using System.Text;

namespace SparkyTools.AutoMapper
{
    internal static class ExceptionHelper
    {
        public static string AddUnmappedPropertyHelperInfo(AutoMapperConfigurationException ex)
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