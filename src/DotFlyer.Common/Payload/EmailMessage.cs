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

        // We keep Body for legacy usage.
        // It is used as fallback if TemplateModel is not specified or rendering fails,
        // but either Body or TemplateModel must be provided.
        public string Body { get; set; }

        public IEnumerable<string> Attachments { get; set; }

        public IDictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Template-specific model. When provided, the email templating
        /// will be used to render HTML. If this is null, the Body will be used.
        /// </summary>
        public object TemplateModel { get; set; }

        /// <summary>
        /// Template identifier. Should correspond to a registered template and its model in the system.
        /// </summary>
        public string TemplateId { get; set; }
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