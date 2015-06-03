using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Utility
{
    public static class RolesHelper
    {
        public static List<RoleRef> GetRoles()
        {
            var list = new List<RoleRef>();
            foreach (var role in Enum.GetValues(typeof(UserRole)))
            {
                list.Add(new RoleRef(){Id = (int)role, Role = role.ToString()});
            }
            foreach (var role in Enum.GetValues(typeof(AgriUserRole)))
            {
                list.Add(new RoleRef(){Id = (int)role, Role = role.ToString()});
            }
            return list;
        } 
    }

    public class RoleRef
    {
        public int Id { get; set; }
        public string Role { get; set; }
    }
}