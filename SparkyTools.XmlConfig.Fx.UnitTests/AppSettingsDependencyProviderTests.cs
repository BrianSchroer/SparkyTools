using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SparkyTools.XmlConfig.Fx.UnitTests
{
    [TestClass]
    public class AppSettingsDependencyProviderTests
    {
        [TestMethod]
        public void AppSettingsDependencyProvider_should_work()
        {
            Func<string, string> getAppSettingsValue = AppSettings.DependencyProvider().GetValue();

            Assert.AreEqual("success", getAppSettingsValue("test"));
        }
    }
}
