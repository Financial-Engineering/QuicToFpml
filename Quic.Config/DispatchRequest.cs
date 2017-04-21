using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Quic.Config.Schemas
{
    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Axis
    {
        private object[] _itemsField;

        /// <remarks />
        [XmlElement("Axis", typeof (Axis))]
        [XmlElement("Boolean", typeof (bool))]
        [XmlElement("Date", typeof (DateTime))]
        [XmlElement("Double", typeof (Double))]
        [XmlElement("Integer", typeof (int))]
        [XmlElement("String", typeof (string))]
        public object[] Items
        {
            get { return _itemsField; }
            set { _itemsField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Double
    {
        private string _unitField;

        private double _valueField;

        /// <remarks />
        [XmlAttribute("unit")]
        public string Unit
        {
            get { return _unitField; }
            set { _unitField = value; }
        }

        /// <remarks />
        [XmlAttribute("value")]
        public double Value
        {
            get { return _valueField; }
            set { _valueField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Grid
    {
        private Axis[] _gridAxesField;
        private GridAxis[] _gridAxisField;

        private GridType _typeField;

        private string _unitField;

        /// <remarks />
        [XmlElement("GridAxis")]
        public GridAxis[] GridAxis
        {
            get { return _gridAxisField; }
            set { _gridAxisField = value; }
        }

        /// <remarks />
        [XmlArrayItem("Axis", IsNullable = false)]
        public Axis[] GridAxes
        {
            get { return _gridAxesField; }
            set { _gridAxesField = value; }
        }

        /// <remarks />
        [XmlAttribute("type")]
        public GridType Type
        {
            get { return _typeField; }
            set { _typeField = value; }
        }

        /// <remarks />
        [XmlAttribute("unit")]
        public string Unit
        {
            get { return _unitField; }
            set { _unitField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class GridAxis
    {
        private string _axisNameField;

        private uint _lengthField;

        /// <remarks />
        public string AxisName
        {
            get { return _axisNameField; }
            set { _axisNameField = value; }
        }

        /// <remarks />
        public uint Length
        {
            get { return _lengthField; }
            set { _lengthField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [XmlType(AnonymousType = true)]
    public enum GridType
    {
        /// <remarks />
        Double,

        /// <remarks />
        Integer,

        /// <remarks />
        Boolean,

        /// <remarks />
        Date,

        /// <remarks />
        String,
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class GridAxes
    {
        private Axis[] _axisField;

        /// <remarks />
        [XmlElement("Axis")]
        public Axis[] Axis
        {
            get { return _axisField; }
            set { _axisField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Map
    {
        private MapEntry[] _mapEntryField;

        /// <remarks />
        [XmlElement("MapEntry")]
        public MapEntry[] MapEntry
        {
            get { return _mapEntryField; }
            set { _mapEntryField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class MapEntry
    {
        private string _keyField;
        private Variant _variantField;

        /// <remarks />
        public Variant Variant
        {
            get { return _variantField; }
            set { _variantField = value; }
        }

        /// <remarks />
        [XmlAttribute("key")]
        public string Key
        {
            get { return _keyField; }
            set { _keyField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Variant
    {
        private object _itemField;

        /// <remarks />
        [XmlElement("Boolean", typeof (bool))]
        [XmlElement("Date", typeof (DateTime))]
        [XmlElement("Double", typeof (Double))]
        [XmlElement("Grid", typeof (Grid))]
        [XmlElement("Integer", typeof (int))]
        [XmlElement("Map", typeof (Map))]
        [XmlElement("String", typeof (string))]
        [XmlElement("Vector", typeof (Vector))]
        public object Item
        {
            get { return _itemField; }
            set { _itemField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Vector
    {
        private Variant[] _variantField;

        /// <remarks />
        [XmlElement("Variant")]
        public Variant[] Variant
        {
            get { return _variantField; }
            set { _variantField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Function
    {
        private Variant[] _argumentsField;

        private string _nameField;

        /// <remarks />
        [XmlArrayItem("Variant", IsNullable = false)]
        public Variant[] Arguments
        {
            get { return _argumentsField; }
            set { _argumentsField = value; }
        }

        /// <remarks />
        [XmlAttribute("name")]
        public string Name
        {
            get { return _nameField; }
            set { _nameField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Arguments
    {
        private Variant[] _variantField;

        /// <remarks />
        [XmlElement("Variant")]
        public Variant[] Variant
        {
            get { return _variantField; }
            set { _variantField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class DispatchRequest
    {
        private Function _functionField;

        /// <remarks />
        public Function Function
        {
            get { return _functionField; }
            set { _functionField = value; }
        }
    }

    /// <remarks />
    [GeneratedCode("xsd", "4.0.30319.17929")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class DispatchResult
    {
        private Variant _variantField;

        /// <remarks />
        public Variant Variant
        {
            get { return _variantField; }
            set { _variantField = value; }
        }
    }
}