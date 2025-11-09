namespace BackEnd.Domain.Entities
{
    public class QuestionElement
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Rotation { get; set; }
        public decimal Scale { get; set; }
        public string SvgName { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public SurveyQuestion Question { get; set; } = null!;
    }
}