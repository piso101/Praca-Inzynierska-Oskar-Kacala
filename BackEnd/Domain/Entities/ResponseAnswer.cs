namespace BackEnd.Domain.Entities
{
    public class ResponseAnswer
    {
        public long Id { get; set; }
        public long ResponseId { get; set; }
        public long QuestionId { get; set; }
        public long SelectedElementId { get; set; }
    }
}