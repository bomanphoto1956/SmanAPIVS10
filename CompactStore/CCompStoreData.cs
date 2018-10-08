using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SManApi.CompactStore
{
    public class CCompStoreData
    {


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class TRANSACTION
    {

        private sbyte companyField;

        private string dateField;

        private string timeField;

        private List<TRANSACTIONORDERLINE> oRDERLINESField;

        /// <remarks/>
        public sbyte Company
        {
            get
            {
                return this.companyField;
            }
            set
            {
                this.companyField = value;
            }
        }

        /// <remarks/>
        public string Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        public string Time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ORDERLINE", IsNullable = false)]
        public List<TRANSACTIONORDERLINE> ORDERLINES
        {
            get
            {
                return this.oRDERLINESField;
            }
            set
            {
                this.oRDERLINESField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class TRANSACTIONORDERLINE
    {

        private sbyte assignmentTypeIdField;

        private string orderNoField;

        private string orderlineNoField;

        private string itemNoField;

        private string itemDescriptionField;

        private string reqQuantityField;

        private int statusField;

        /// <remarks/>
        public sbyte AssignmentTypeId
        {
            get
            {
                return this.assignmentTypeIdField;
            }
            set
            {
                this.assignmentTypeIdField = value;
            }
        }

        /// <remarks/>
        public string OrderNo
        {
            get
            {
                return this.orderNoField;
            }
            set
            {
                this.orderNoField = value;
            }
        }

        /// <remarks/>
        public string OrderlineNo
        {
            get
            {
                return this.orderlineNoField;
            }
            set
            {
                this.orderlineNoField = value;
            }
        }

        /// <remarks/>
        public string ItemNo
        {
            get
            {
                return this.itemNoField;
            }
            set
            {
                this.itemNoField = value;
            }
        }

        /// <remarks/>
        public string ItemDescription
        {
            get
            {
                return this.itemDescriptionField;
            }
            set
            {
                this.itemDescriptionField = value;
            }
        }

        /// <remarks/>
        public string ReqQuantity
        {
            get
            {
                return this.reqQuantityField;
            }
            set
            {
                this.reqQuantityField = value;
            }
        }

        /// <remarks/>
        public int Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
    }

}

    public class updateOAStorageData
    {
        public int orderArtId
        {
            get;
            set;
        }

        public Decimal stockToSend
        {
            get;
            set;
        }

        public string error
        {
            get;
            set;
        }
    }
}