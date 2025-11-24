namespace BackEnd.Domain.DTO
{
    public class ChangeQuestionElementOrderDTO
    {
        public ICollection<Tuple<long, int>> Elements { get; set; } = new List<Tuple<long, int>>();
        public long QuestionID { get; set; }
    }
}