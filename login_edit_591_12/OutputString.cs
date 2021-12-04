using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace login_edit_591_12
{
    public class OutputString
    {
        public string tempStr { get; set; }

        public string getQuestions()
        {
            string tempStr = "";
            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
            {
                var list = db.QuestionInfos.Select(e => e);
                foreach (var question in list)
                    tempStr += $"{question.IdQuestion} {question.QuestionValue}\n";
            }
            return tempStr;
        }
    }
}
