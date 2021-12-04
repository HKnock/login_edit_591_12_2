using System;
using System.Collections.Generic;

#nullable disable

namespace login_edit_591_12
{
    public partial class UserInfo
    {
        public int IdUser { get; set; }
        public string Nickname { get; set; }
        public string UserPassword { get; set; }
        public string Phone { get; set; }
        public int IdQuestion { get; set; }
        public string Answer { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int? IdGender { get; set; }

        public virtual GenderInfo IdGenderNavigation { get; set; }
        public virtual QuestionInfo IdQuestionNavigation { get; set; }
    }
}
