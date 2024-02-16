using System;

namespace OntrackDb.Dto
{
    public class PatientDto
    {
        public int Id { get; set; }
        public Boolean IsDeleted { get; set; }
        public Double CholestrolPDC { get; set; }
        public Double DiabetesPDC { get; set; }
        public Double RASAPDC { get; set; }
    }
}
