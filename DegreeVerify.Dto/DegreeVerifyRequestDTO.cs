using System;
using System.Collections.Generic;

namespace DegreeVerify.DTO
{
    public class DegreeVerifyRequestDTO
    {
        public byte? RequestTypeId { get; set; }
        public string AccountId { get; set; } = null;
        public string OrganizationName { get; set; }
        public string CaseReferenceId { get; set; }
        public string CorrelationId { get; set; }
        public string ContactEmail { get; set; }
        public string SSN { get; set; }
        public string DateOfBirth { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public List<PreviousName> PreviousNames { get; set; } = new List<PreviousName>();
        public string StudentId { get; set; }
        public string Terms { get; set; }
        public string SchoolCode { get; set; }
        public string BranchCode { get; set; }
        public string DegreeTitle { get; set; }
        public short? YearAwarded { get; set; }
        public string Major { get; set; }
        public string DegreeLevelCode { get; set; }
        public string ApplyLikeSchoolMatching { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string EndClient { get; set; }
        public string SecondaryClient { get; set; }
        public DateTime? StartDate { get; set; }
        public string JobTitle { get; set; }
        public string NaicsCode { get; set; }
    }
}
