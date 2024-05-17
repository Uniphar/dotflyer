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
        public Contact From { get; set; }

        [Required]
        public IEnumerable<Contact> To { get; set; }

        public IEnumerable<Contact> Cc { get; set; }

        public IEnumerable<Contact> Bcc { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public IEnumerable<string> Attachments { get; set; }

        public IDictionary<string, string> Tags { get; set; }
    }

    /// <summary>
    /// Email contact payload.
    /// </summary>
    public class Contact
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }
    }
}