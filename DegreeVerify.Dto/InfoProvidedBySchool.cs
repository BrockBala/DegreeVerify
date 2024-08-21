using System;

namespace DegreeVerify.DTO
{
    public class InfoProvidedBySchool
    {
        public DateTime? ProjectedGradDate { get; set; }

        public string SchoolComment { get; set; }

        //[JsonConverter(typeof(CustomDateConverter))]
        public DateTime? UpdatedDate { get; set; }
    }
}
