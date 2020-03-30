using System.Collections.Generic;

namespace KnowledgeShareR.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
        public List<Answer> Answers { get; set; }
    }
}