using System;
using System.Configuration;
using System.Xml;

namespace SparkyTools.XmlConfig
{
    /// <summary>
    /// Generic configuration section deserializer.
    /// </summary>
    /// <remarks>
    /// This class allows the addition of a .config section to load any type without having
    /// to write a custom <see cref="IConfigurationSectionHandler"/> for the section.
    /// </remarks>
    /// <seealso cref="ConfigurationSectionListDeserializer"/>.
    public class ConfigurationSectionDeserializer : IConfigurationSectionHandler
    {
        /// <summary>
        /// This method is called by <see cref="ConfigurationManager.GetSection(string)"/>
        /// for a section defined in the "configSections" section of the .config file with
        /// a "type" attribute specifying this class type (<see cref="ConfigurationSectionDeserializer"/>).
        /// </summary>
        /// <remarks>
        /// When the static <see cref="Load{T}(string, bool)"/> method of this class calls
        /// <see cref="ConfigurationSectionDeserializerHelper.GetSection(string, bool)"/>,
        /// and it calls <see cref="ConfigurationManager.GetSection(string)"/>, a new instance of this
        /// class is created (because of the "configSections" type specification), and this method is called.
        /// </remarks>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">COnfiguration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section) => 
            ConfigurationSectionDeserializerHelper.DeserializeXmlNode(
                caller: this,
                sectionName: section.Name,
                node: section,
                typeName: ConfigurationSectionDeserializerHelper.GetSectionTypeName(section));

        /// <summary>
        /// Loads a new instance of the specified type from a section in a .config file.
        /// </summary>
        /// <typeparam name="T">The type into which the .config XML should be deserialized.</typeparam>
        /// <param name="sectionName">The .config file section name</param>
        /// <param name="shouldThrowExceptionIfSectionNotFound">
        /// Should an exception be thrown if the section is not found? (default = <c>true</c>).
        /// </param>
        /// <returns>New instance of the specified type.</returns>
        /// <exception cref="ConfigurationErrorsException"> if
        ///     <para>exception is thrown by <see cref="ConfigurationManager.GetSection(string)"/></para>
        ///     <para>* unable to cast .config section to <typeparamref name="T"/>.</para>
        /// </exception>
        /// <example>
        ///     <para>.config XML:</para>
        ///     <code><![CDATA[
        ///     <configSections> 
        ///         ...
        ///         <section name="fooConfiguration" type="SparkyTools.XmlConfig.ConfigurationSectionDeserializer, SparkyTools.XmlConfig" />
        ///         ...
        ///     </configSections>
        ///     ...
        ///     <fooConfiguration type="FooNamespace.Foo, FooAssemblyName">
        ///         <Bar>bar value</Bar>
        ///         <Baz>baz value</Baz>
        ///         <Qux>qux value</Qux>
        ///     </fooConfiguration>
        ///     ]]></code>
        ///     <para>C# code to load instance from .config file:</para>
        ///     <code><![CDATA[
        ///     Foo foo = ConfigurationSectionDeserializer.Load<Foo>("fooConfiguration");
        ///     ]]></code>
        /// <example>
        public static T Load<T>(string sectionName, bool shouldThrowExceptionIfSectionNotFound = true)
        {
            try
            {
                return (T)ConfigurationSectionDeserializerHelper.GetSection(
                    sectionName, shouldThrowExceptionIfSectionNotFound);
            }
            catch (InvalidCastException ex)
            {
                throw ConfigurationSectionDeserializerHelper.NewConfigurationErrorsException(ex, sectionName);
            }
        }
    }
}
