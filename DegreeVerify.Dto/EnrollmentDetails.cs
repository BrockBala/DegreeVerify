using System;
using System.Collections.Generic;

namespace DegreeVerify.DTO
{
    public class EnrollmentDetails
    {
        public string OfficialSchoolName { get; set; }
        public string SchoolCode { get; set; }
        public string BranchCode { get; set; }
        public string CurrentEnrollmentStatus { get; set; }
        public DateTime? EnrollmentSinceDate { get; set; }
        public List<EnrollmentData> EnrollmentData { get; set; } = new List<EnrollmentData>();
        public NameOnSchoolRecord NameOnSchoolRecord { get; set; }
    }

    public class EnrollmentData
    {
        public string EnrollmentId { get; set; }
        public string EnrollmentStatus { get; set; }
        public DateTime? TermBeginDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public DateTime? SchoolCertifiedOnDate { get; set; }
        public DateTime? AnticipatedGraduationDate { get; set; }
        public List<CourseOfStudy> MajorCoursesOfStudy { get; set; } = new List<CourseOfStudy>();
    }
}
