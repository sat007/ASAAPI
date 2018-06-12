namespace ASA.Core
{
    internal class TestCredentials
    {
        private string _senderId = "";
        private string _senderValue = "";
        private string _vatRegNbr = "";

        public string SenderId
        {
            get { return this._senderId; }
            set { this._senderId = value; }
        }

        public string SenderValue
        {
            get { return this._senderValue; }
            set { this._senderValue = value; }
        }

        public string VatRegNbr
        {
            get { return this._vatRegNbr; }
            set { this._vatRegNbr = value; }
        }

        public TestCredentials()
        {
        }

        public TestCredentials(string senderId, string senderValue, string vatRegNbr)
        {
            this._senderId = senderId;
            this._senderValue = senderValue;
            this._vatRegNbr = vatRegNbr;
        }
    }
}
