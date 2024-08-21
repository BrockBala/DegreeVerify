using Newtonsoft.Json;

namespace DegreeVerify.DTO
{
    public class CourseOfStudy : Course
    {
        public string NcesCIPCode { get; set; }
    }

    public class Course
    {
        [JsonProperty("course")]
        public string CourseName { get; set; }
    }
}
