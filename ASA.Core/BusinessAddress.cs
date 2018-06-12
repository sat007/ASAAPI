using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    public class BusinessAddress
    {
        [Key, ForeignKey("Business")]
        public int BusinessAddressId { get; set; }

        private string _line1 = "";
        private string _line2 = "";
        private string _line3 = "";
        private string _line4 = "";
        private string _postcode = "";
        private string _country = "";

        public virtual Business Business { get; set; } //one to one relationship prop 
        public string Line1
        {
            get { return this._line1; }
            set { this._line1 = value; }
        }

        public string Line2
        {
            get { return this._line2; }
            set { this._line2 = value; }
        }

        public string Line3
        {
            get { return this._line3; }
            set { this._line3 = value; }
        }

        public string Line4
        {
            get { return this._line4; }
            set { this._line4 = value; }
        }

        public string Postcode
        {
            get { return this._postcode; }
            set { this._postcode = value; }
        }

        public string Country
        {
            get { return this._country; }
            set { this._country = value; }
        }

        public BusinessAddress()
        {
        }

        public BusinessAddress(string line1, string line2, string line3, string line4, string postcode, string country)
        {
            this._line1 = line1;
            this._line2 = line2;
            this._line3 = line3;
            this._line4 = line4;
            this._postcode = postcode;
            this._country = country;
        }
    }

}

