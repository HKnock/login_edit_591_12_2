using System;
using System.Collections.Generic;

#nullable disable

namespace login_edit_591_12
{
    public partial class QuestionInfo
    {
        public QuestionInfo()
        {
            UserInfos = new HashSet<UserInfo>();
        }

        public int IdQuestion { get; set; }
        public string QuestionValue { get; set; }

        public virtual ICollection<UserInfo> UserInfos { get; set; }
    }
}
