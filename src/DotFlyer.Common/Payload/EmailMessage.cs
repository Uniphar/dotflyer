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
        public required Contact From { get; init; }

        [Required]
        public required IEnumerable<Contact> To { get; init; }

        public IEnumerable<Contact>? Cc { get; init; }

        public IEnumerable<Contact>? Bcc { get; set; }

        [Required]
        public required string Subject { get; init; }

        // We keep Body for legacy usage.
        // It is used as fallback if TemplateModel is not specified or rendering fails,
        // but either Body or TemplateModel must be provided.
        public string? Body { get; init; }

        public IEnumerable<string>? Attachments { get; init; }

        public IDictionary<string, string>? Tags { get; init; }

        /// <summary>
        /// Template-specific model. When provided, the email templating
        /// will be used to render HTML. If this is null, the Body will be used.
        /// </summary>
        public object? TemplateModel { get; init; }

        /// <summary>
        /// Template identifier. Should correspond to a registered template and its model in the system.
        /// </summary>
        public string? TemplateId { get; init; }
    }

    /// <summary>
    /// Email contact payload.
    /// </summary>
    public class Contact
    {
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Name { get; init; }
    }
}