using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotFlyer.Common.Payload
{
    /// <summary>
    /// SMS message payload.
    /// </summary>
    public class SMSMessage
    {
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Body { get; set; }

        public IDictionary<string, string> Tags { get; set; }
    }
}