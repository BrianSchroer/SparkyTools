using SparkyTools.DependencyProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace SparkyTools.XmlConfig
{
    /// <summary>
    /// Generic configuration section deserializer for lists.
    /// </summary>
    /// <remarks>
    /// This class allows the addition of a .config section to load a list of any type without having
    /// to write a custom <see cref="IConfigurationSectionHandler"/> for the section.
    /// </remarks>
    /// <seealso cref="ConfigurationSectionDeserializer"/>.
    public class ConfigurationSectionListDeserializer : IConfigurationSectionHandler
    {
        /// <summary>
        /// This method is called by <see cref="ConfigurationManager.GetSection(string)"/>
        /// for a section defined in the "configSections" section of the .config file with
        /// a "type" attribute specifying this class type (<see cref="ConfigurationSectionListDeserializer"/>).
        /// </summary>
        /// <remarks>
        /// When the static <see cref="Load{T}(string, bool, bool)"/> method of this class calls
        /// <see cref="ConfigurationSectionDeserializerHelper.GetSection(string, bool)"/>,
        /// and it calls <see cref="ConfigurationManager.GetSection(string)"/>, a new instance of this
        /// class is created (because of the "configSections" type specification), and this method is called.
        /// </remarks>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">COnfiguration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            string sectionTypeName = ConfigurationSectionDeserializerHelper.GetSectionTypeName(section);
            XmlNode arrayNode = GetArrayNode(section, sectionTypeName);
            string typeName = $"System.Collections.Generic.List`1[[{sectionTypeName}]]";

            return ConfigurationSectionDeserializerHelper.DeserializeXmlNode(
                caller: this,
                sectionName: section.Name,
                node: arrayNode,
                typeName: typeName,
                shouldNodeNameMatchTypeName: false);
        }

        /// <summary>
        /// Loads a new <see cref="IList{T}"/> from a section in a .config file.
        /// </summary>
        /// <typeparam name="T">The types into which the .config XML should be deserialized.</typeparam>
        /// <param name="sectionName">The .config file section name</param>
        /// <param name="shouldThrowExceptionIfSectionNotFound">
        /// Should an exception be thrown if the section is not found? (default = <c>true</c>).
        /// </param>
        /// <param name="shouldAllowEmptyList">
        /// Should an empty list be allowed? (default = <c>false</c>.)
        /// </param>
        /// <returns>New <see cref="IList{T}"/> instance.</returns>
        /// <exception cref="ConfigurationErrorsException"> if
        ///     <para>exception is thrown by <see cref="ConfigurationManager.GetSection(string)"/></para>
        ///     <para>* unable to cast .config section to <see cref="IList{T}"/>.</para>
        ///     <para>* <paramref name="shouldAllowEmptyList"/> is <c>false</c> and the list is empty.</para>
        /// </exception>
        /// <example>
        ///     <para>.config XML:</para>
        ///     <code><![CDATA[
        ///     <configSections> 
        ///         ...
        ///         <section name="foos" type="SparkyTools.XmlConfig.ConfigurationSectionListDeserializer, SparkyTools.XmlConfig" />
        ///         ...
        ///     </configSections>
        ///     ...
        ///     <foos type="FooNamespace.Foo, FooAssemblyName">
        ///         <Foo>
        ///             <Bar>bar 1</Bar>
        ///             <Baz>baz1 value</Baz>
        ///             <Qux>qux1 value</Qux>
        ///         </Foo>
        ///         <Foo>
        ///             <Bar>bar1 value</Bar>
        ///             <Baz>baz1 value</Baz>
        ///             <Qux>qux1 value</Qux>
        ///         </Foo>
        ///         <Foo>
        ///             <Bar>bar1 value</Bar>
        ///             <Baz>baz1 value</Baz>
        ///             <Qux>qux1 value</Qux>
        ///         </Foo>
        ///     </foos>
        ///     ]]></code>
        ///     <para>C# code to load instance from .config file:</para>
        ///     <code><![CDATA[
        ///     Foo foo = ConfigurationSectionDeserializer.Load<Foo>("fooConfiguration");
        ///     ]]></code>
        /// </example>
        public static IList<T> Load<T>(
            string sectionName, 
            bool shouldThrowExceptionIfSectionNotFound = true,
            bool shouldAllowEmptyList = false)
        {
            var list = 
                ConfigurationSectionDeserializerHelper.GetSection(
                    sectionName, shouldThrowExceptionIfSectionNotFound)
                as IList<T>;

            if (list?.Count == 0 && !shouldAllowEmptyList)
            {
                throw ConfigurationSectionDeserializerHelper.NewConfigurationErrorsException(sectionName, "Item count = zero.");
            }

            return list;
        }

        /// <summary>
        /// Create <see cref="SparkyTools.DependencyProvider.DependencyProvider{TDependency}"/>
        /// with a callback to this class's static <see cref="Load{T}(string, bool, bool)"/> method,
        /// using <see cref="SparkyTools.DependencyProvider.DependencyProvider.CreateStatic{TDependency}(Func{TDependency})"/> 
        /// so we only load from .config once.
        /// </summary>
        /// <typeparam name="T">The type to be loaded.</typeparam>
        /// <param name="sectionName">Name of the .config file section.</param>
        /// <returns>New <see cref="SparkyTools.DependencyProvider.DependencyProvider{TDependency}"/> instance.</returns>
        public static DependencyProvider<IList<T>> DependencyProvider<T>(string sectionName)
        {
            return SparkyTools.DependencyProvider.DependencyProvider.CreateStatic(() => Load<T>(sectionName));
        }

        private static XmlNode GetArrayNode(XmlNode nodeFromConfig, string itemTypeFullName)
        {
            string sectionName = nodeFromConfig.Name;
            Type itemType = Type.GetType(itemTypeFullName);

            if (itemType == null)
            {
                throw ConfigurationSectionDeserializerHelper.NewConfigurationErrorsException(
                    sectionName, $"\"{itemTypeFullName}\" is not a valid type name.");
            }

            string shortName = itemType.Name;
            string arrayName = $"ArrayOf{shortName}";

            if (nodeFromConfig.Name == arrayName)
            {
                return nodeFromConfig;
            }

            if (nodeFromConfig.OwnerDocument == null)
            {
                throw ConfigurationSectionDeserializerHelper.NewConfigurationErrorsException(
                    sectionName, "OwnerDocument is null.");
            }

            XmlElement arrayNode = nodeFromConfig.OwnerDocument.CreateElement(arrayName);

            foreach (XmlNode childNode in nodeFromConfig.ChildNodes)
            {
                arrayNode.AppendChild(childNode.Clone());
            }

            return arrayNode;
        }
    }
}
