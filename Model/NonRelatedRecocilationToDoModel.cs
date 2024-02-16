namespace OntrackDb.Model
{
    public class NonRelatedRecocilationToDoModel
    {

        public int Id { get; set; }
        public int PatientId { get; set; }
        public string ActionItem { get; set; }
        public string PatientToDo { get; set; }
    }
}
