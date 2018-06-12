using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASA.Core
{
    public class DataRequestStatus
    {
        private string _correlationId;
        private string _status;
        public int HMRCResponseId { get; set; }
        [ForeignKey("HMRCResponseId")]
        public virtual HMRCResponse HMRCResponse { get; set; }

        [Key]
        public int Id { get; set; }
        public string CorrelationId
        {
            get { return this._correlationId; }
            set { _correlationId = value; }
        }

        public string Status
        {
            get { return this._status; }
            set { _status = value; }
        }

        public DataRequestStatus()
        {
        }

        public DataRequestStatus(string correlationId, string status)
        {
            this._correlationId = correlationId;
            this._status = status;
        }
    }
}
