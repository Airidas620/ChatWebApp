using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HermeApp;

// Add profile data for application users by adding properties to the HermeAppWebUser class
public class HermeAppWebUser : IdentityUser
{
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();

    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}

