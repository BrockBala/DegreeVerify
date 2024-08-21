using System;
using System.Collections.Generic;

namespace DegreeVerify.DTO
{
    public class StudentInfoProvided
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public List<PreviousName> PreviousNames { get; set; } = new List<PreviousName>();
        public DateTime? DateOfBirth { get; set; }
        public string SchoolCode { get; set; }
        public string BranchCode { get; set; }
        public string DegreeLevel { get; set; }
        public string ApplyLikeSchoolMatching { get; set; }
        public string DegreeTitle { get; set; }
        public int? YearAwarded { get; set; }
        public string Major { get; set; }
    }
}
