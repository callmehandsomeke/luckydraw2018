using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;

namespace LuckyDraw2018Import
{
    class ADHelper
    {
        private static readonly string DomainName = "global.anz.com";

        public static IEnumerable<dynamic> GetAllUsersInGroup(string group)
        {
            var list = new List<dynamic>();
            using (var ctx = new PrincipalContext(ContextType.Domain, DomainName))
            {
                using (var grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, group))
                {
                    var members = grp.GetMembers();
                    foreach (var member in members)
                    {
                        list.Add(new { lanid = member.SamAccountName, name = member.DisplayName });
                    }
                }
            }
            return list;
        }
    }
}
