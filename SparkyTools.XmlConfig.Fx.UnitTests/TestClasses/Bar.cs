using System;
using System.Xml.Serialization;

namespace SparkyTools.XmlConfig.Fx.UnitTests.TestClasses
{
    public class Bar
    {
        public string Quuz { get; set; }

        public int Corge { get; set; }

        public double Grault { get; set; }

        public StringComparison Garply { get; set; }
    }
}
