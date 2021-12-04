using System;
using System.Collections.Generic;

#nullable disable

namespace login_edit_591_12
{
    public partial class GenderInfo
    {
        public GenderInfo()
        {
            UserInfos = new HashSet<UserInfo>();
        }

        public int IdGender { get; set; }
        public string GenderValue { get; set; }

        public virtual ICollection<UserInfo> UserInfos { get; set; }
    }
}
