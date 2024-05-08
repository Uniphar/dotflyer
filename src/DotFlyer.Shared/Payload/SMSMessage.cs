using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotFlyer.Common.Payload
{
    /// <summary>
    /// SMS message payload.
    /// </summary>
    public class SMSMessage
    {
        [Required]
        public string To { get; set; }

        [Required]
        public string Body { get; set; }

        public Dictionary<string, string> Tags { get; set; }
    }
}