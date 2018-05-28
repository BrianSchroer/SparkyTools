using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace SparkyTools.XmlConfig.Fx
{
    /// <summary>
    /// Helper methods for
    /// <see cref="ConfigurationSectionDeserializer"/> and
    /// <see cref="ConfigurationSectionListDeserializer"/>.
    /// </summary>
    internal static class ConfigurationSectionDeserializerHelper
    {
        /// <summary>
        /// Get object from .config section.
        /// </summary>
        /// <remarks>
        /// <see cref="ConfigurationManager.GetSection"/> calls back to the calling
        /// <see cref="ConfigurationSectionDeserializer"/> or <see cref="ConfigurationSectionListDeserializer"/>
        /// instance's <see cref="IConfigurationSectionHandler.Create"/> method.
        /// </remarks>
        /// <param name="sectionName">The .config sectionname</param>
        /// <param name="shouldThrowExceptionIfNotFound">Should exception be thrown
        /// if the section is not found?</param>
        /// <returns>The object.</returns>
        public static object GetSection(string sectionName, bool shouldThrowExceptionIfNotFound)
        {
            object section;

            try
            {
                section = ConfigurationManager.GetSection(sectionName);
            }
            catch (Exception ex)
            {
                throw NewConfigurationErrorsException(ex, sectionName);
            }

            if (shouldThrowExceptionIfNotFound && section == null)
            {
                throw NewConfigurationErrorsException(sectionName, "Section not found.");
            }

            return section;
        }

        public static string GetSectionTypeName(XmlNode section)
        {
            XPathNavigator navigator = section.CreateNavigator();

            string evaluatedType = navigator.Evaluate("string(@type)").ToString();

            if (string.IsNullOrEmpty(evaluatedType))
            {
                throw new ConfigurationErrorsException("The \"type\" attribute is not present.");
            }

            return evaluatedType;
        }

        public static object DeserializeXmlNode(
            IConfigurationSectionHandler caller,
            string sectionName,
            XmlNode node,
            string typeName,
            bool shouldNodeNameMatchTypeName = true)
        {
            Type typeToDeserialize = Type.GetType(typeName);

            if (typeToDeserialize == null)
            {
                throw NewConfigurationErrorsException(sectionName, $"\"{typeName}\" is not a recognized type name.");
            }

            string xmlString = node.OuterXml;
            string nodeName = node.Name;
            string shortTypeName = typeToDeserialize.Name;

            if (shouldNodeNameMatchTypeName && nodeName != shortTypeName)
            {
                xmlString = xmlString
                    .Replace($"<{nodeName}>", $"<{shortTypeName}>")
                    .Replace($"<{nodeName} ", $"<{shortTypeName} ")
                    .Replace($"</{nodeName}>", $"</{shortTypeName}>");
            }

            var xmlSerializer = new XmlSerializer(typeToDeserialize);

            try
            {
                using (var sr = new StringReader(xmlString))
                {
                    return xmlSerializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "There is an error in the XML document.")
                {
                    Exception inner = ex.InnerException;
                    if (inner != null && inner.Message.ToLowerInvariant().Contains(sectionName.ToLowerInvariant()))
                    {
                        throw new ConfigurationErrorsException(
                            $"Unable to load from the \"{sectionName} .config section using {caller.GetType().FullName}."
                                + " Please check the \"configSections\" definition for this section.",
                            ex
                        );
                    }
                }

                throw;
            }
        }

        public static string ExceptionMessagePrefix(string sectionName) => 
            $"Error loading from the \"{sectionName}\" section of the .config file: ";

        public static ConfigurationErrorsException NewConfigurationErrorsException(Exception ex, string sectionName)
        {
            var sb = new StringBuilder(ExceptionMessagePrefix(sectionName));
            sb.AppendLine(ex.Message);

            Exception innerException = ex.InnerException;

            while (innerException != null)
            {
                sb.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }

            return new ConfigurationErrorsException(sb.ToString(), ex);
        }

        internal static ConfigurationErrorsException NewConfigurationErrorsException(
            string sectionName, string message) =>
                new ConfigurationErrorsException($"{ExceptionMessagePrefix(sectionName)}{message}");
    }
}
