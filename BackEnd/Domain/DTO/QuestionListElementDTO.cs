using BackEnd.Domain.Entities;

namespace BackEnd.Domain.DTO
{
    public class QuestionListElementDTO
    {
        public QuestionListElementDTO(QuestionElement questionElement) {
            this.Id = questionElement.Id;
            this.OrderIndex = questionElement.OrderIndex;
            this.SvgName = questionElement.SvgName;
        }
        public long Id { get; set; }
        public int OrderIndex { get; set; }
        public string SvgName { get; set; } = string.Empty;
    }
}