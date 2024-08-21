using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DegreeVerify.DTO
{
    public class DegreeDetail
    {
        public string DegreeStatus { get; set; }
        public string OfficialSchoolName { get; set; }
        public string SchoolCode { get; set; } 
        public string BranchCode { get; set; }
        public NameOnSchoolRecord NameOnSchoolRecord { get; set; }
        public string SchoolDivision { get; set; }
        public string JointInstitution { get; set; }
        public string DegreeTitle { get; set; }
        public DateTime AwardDate { get; set; }
        public List<CourseOfStudy> MajorCoursesOfStudy { get; set; } = new List<CourseOfStudy>();
        public List<CourseOfStudy> MinorCoursesOfStudy { get; set; } = new List<CourseOfStudy>();
        public List<Course> MajorOptions { get; set; } = new List<Course>();
        public List<Course> MajorConcentrations { get; set; } = new List<Course>();
        public string AcademicHonors { get; set; }
        public string HonorsProgram { get; set; }
        public string OtherHonors { get; set; }
        public DatesOfAttendance DatesOfAttendance { get; set; } = new DatesOfAttendance();
        public string DegreeId { get; set; }
    }

    public class DatesOfAttendance
    {
        public string DatesOfAttendanceId { get; set; }
        public DateTime? TermBeginDate { get; set; } = null;
        public DateTime? TermEndDate { get; set; } = null;
    }
}
