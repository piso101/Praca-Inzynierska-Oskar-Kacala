using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class UpdateQuestionOrderDTO
    {
        public ICollection<Tuple<long,int>> Questions { get; set; } = new List<Tuple<long, int>>();
        public long SurveyId { get; set; }
    }
}
