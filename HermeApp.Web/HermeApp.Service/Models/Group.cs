using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermeApp
{
    public class Group 
    {
        [Key]
        public int GroupId { get; set; }

        public string GroupName { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = null!;

        public ICollection<UserGroup> UserGroups { get; set; }  = new List<UserGroup>();
    }
}
