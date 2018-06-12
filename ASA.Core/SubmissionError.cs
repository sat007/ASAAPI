using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASA.Core
{
    public class SubmissionError
    {
        private string _errorRaisedBy = "";
        private string _errorNumber = "";
        private string _errorType = "";
        private string _errorText = "";
        private string _errorLocation = "";
        [Key]
        public int Id { get; set; }
        public int HMRCResponseId { get; set; }
        [ForeignKey("HMRCResponseId")]
        public virtual HMRCResponse HMRCResponse { get; set; }
        //[ForeignKey("PeriodData")]
        //public int PeriodId { get; set; }
        public string ErrorRaisedBy
        {
            get
            {
                return this._errorRaisedBy;
            }
            set
            {
                this._errorRaisedBy = value;
            }
        }

        public string ErrorNumber
        {
            get
            {
                return this._errorNumber;
            }
            set
            {
                this._errorNumber = value;
            }
        }

        public string ErrorType
        {
            get
            {
                return this._errorType;
            }
            set
            {
                this._errorType = value;
            }
        }

        public string ErrorText
        {
            get
            {
                return this._errorText;
            }
            set
            {
                this._errorText = value;
            }
        }

        public string ErrorLocation
        {
            get
            {
                return this._errorLocation;
            }
            set
            {
                this._errorLocation = value;
            }
        }
        public SubmissionError()
        {
        }

        public SubmissionError(string raisedBy, string number, string type, string text, string location)
        {
            this._errorRaisedBy = raisedBy;
            this._errorNumber = number;
            this._errorType = type;
            this._errorText = text;
            this._errorLocation = location;
        }
    }
}
