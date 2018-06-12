using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASA.Core
{
    [Table("ReturnsSenderInfo")]
    public class Sender
    {
        #region Properties 
        private string _title;
        private string _forName1;
        private string _forName2;
        private string _surName;
        private string _telephone;
        private string _mobile;
        private string _email;
        //private string _company;
        private string _addressLine1;
        private string _addressLine2;
        private string _addressLine3;
        //private string _addressLine4;
        private string _postCode;
        private string _country;
       // private string _senderId;
        private string _senderPassword;

        private string _hmrcUserId;
        private string _hmrcPassword;

        private SenderType _type;
        //private bool _isTestCredentials;
        //private ArrayList _alTestCredentials;

        //[ForeignKey("Business")]
        //public int BusinessId { get; set; }
        //public Business Business { get; set; }
        //private Business _business;
        // public ICollection<Business> Business;

        [Key]
        public int SenderId { get; set; }
        //public Business BusinessDetails
        //{
        //    get { return this._business; }
        //    set
        //    {
        //        this._business = value;
        //    }
        //}
        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }
        public string HMRCUserId { get { return this._hmrcUserId; } set { this._hmrcUserId = value; } }
        public string HMRCPassword { get { return this._hmrcPassword; } set { this._hmrcPassword = value; } }
        public string ForName1
        {
            get { return this._forName1; }
            set { this._forName1 = value; }
        }

        public string ForName2
        {
            get { return this._forName2; }
            set { this._forName2 = value; }
        }

        public string SurName
        {
            get { return this._surName; }
            set { this._surName = value; }
        }

        public string Telephone
        {
            get { return this._telephone; }
            set { this._telephone = value; }

        }

        public string Mobile
        {
            get { return this._mobile; }
            set { this._mobile = value; }
        }

        public string Email
        {
            get { return this._email; }
            set { this._email = value; }
        }

        public string AddressLine1
        {
            get { return this._addressLine1; }
            set { this._addressLine1 = value; }
        }
        public string AddressLine2
        {
            get { return this._addressLine2; }
            set { this._addressLine2 = value; }
        }
        public string AddressLine3
        {
            get { return this._addressLine3; }
            set { this._addressLine3 = value; }
        }
        public string Postcode
        {
            get{return this._postCode;}
            set{this._postCode = value;}
        }
        public string Country
        {
            get { return this._country; }
            set { this._postCode = value; }
        }
        public SenderType Type
        {
            get { return this._type; }
            set { this._type = value; }

        }
    
        //public string SenderId
        //{
        //    get{return this._senderId;}
        //    set{this._senderId = value;}
        //}

        public string SenderPassword
        {
            get{return this._senderPassword;}
            set{this._senderPassword = value;}
        }
        //public bool IsTestCredentials
        //{
        //    get{return this._isTestCredentials;}
        //    set{this._isTestCredentials = value;}
        //}

        #endregion Properties 

        #region Contructor 

        public Sender() {
           // this.Business = new List<Business>();
        }

        public Sender(string title, string forname1, string forname2, string surname, string telephone, string mobile, string email, string addressline1, string addressline2, string addressline3, string postcode, string password)
        {
            this._type = SenderType.Contractor; // always idividual 
                                                // this._alTestCredentials = new ArrayList();
            this._title = title;
            this._forName1 = forname1;
            this._forName2 = forname2;
            this._surName = surname;
            this._telephone = telephone;
            this._mobile = mobile;
            this._email = email;
            this._addressLine1 = addressline1;
            this._addressLine2 = addressline2;
            this._addressLine3 = addressline3;
            this._postCode = postcode;
            //this._senderId = senderId;
            this._senderPassword = password;
        }
        #endregion Constructor 
   }

    public enum SenderType
    {
        Agent,
        BusinessPOC,
        Contractor
    }
}
