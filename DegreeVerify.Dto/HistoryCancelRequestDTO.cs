using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegreeVerify.Dto
{
    public class HistoryCancelRequestDTO
    {
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionFee { get; set; }
        public string SalesTax { get; set; }
        public string TransactionTotal { get; set; }
        public string RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; } = null;
        public DateTime? NotifiedDate { get; set; } = null;
        public string NscHit { get; set; }
        public string SchoolContactHistory { get; set; }
        public string AppliedLikeSchool { get; set; }
        public string StudentComments { get; set; }
    }
}
