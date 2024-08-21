using System.Collections.Generic;

namespace DegreeVerify.DTO
{
    public class DegreeDTO
    {
        public Status Status { get; set; }
        public TransactionDetails TransactionDetails { get; set; }
        public ClientData ClientData { get; set; }
        public StudentInfoProvided StudentInfoProvided { get; set; }
        public List<DegreeDetail> DegreeDetails { get; set; } = new List<DegreeDetail>();
        public List<EnrollmentDetails> EnrollmentDetails { get; set; } = new List<EnrollmentDetails>();
        public List<InfoProvidedBySchool> InfoProvidedBySchool { get; set; } = new List<InfoProvidedBySchool>();
    }
}
