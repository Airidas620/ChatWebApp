using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HermeApp
{
    public class Message 
     {
        [Key]
        public int MessageId { get; set; }

        public string MessageText { get; set; } = null!;

        public HermeAppWebUser Sender { get; set; } = new HermeAppWebUser();
        public string SenderId { get; set; }  = "";

        public HermeAppWebUser? Receiver { get; set; }
        public string? ReceiverId { get; set; }

        public Group? Group { get; set; }

        public int? GroupId { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
