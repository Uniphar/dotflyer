using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotFlyer.Common.Payload
{
    /// <summary>
    /// Email message payload.
    /// </summary>
    public class EmailMessage
    {
        [Required]
        public string FromEmail { get; set; }

        [Required]
        public string FromName { get; set; }

        public List<EmailRecipient> To { get; set; } = new List<EmailRecipient>();

        public List<EmailRecipient> Cc { get; set; } = new List<EmailRecipient>();

        public List<EmailRecipient> Bcc { get; set; } = new List<EmailRecipient>();

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public List<string> Attachments { get; set; } = new List<string>();

        public Dictionary<string, string> Tags { get; set; }
    }

    /// <summary>
    /// Email recipient payload.
    /// </summary>
    public class EmailRecipient
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }
    }
}