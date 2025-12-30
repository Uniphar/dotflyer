using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotFlyer.Common.Payload;

/// <summary>
/// SMS message payload.
/// </summary>
public class SMSMessage
{
    public string? From { get; set; }

    [Required]
    public required string To { get; init; }

    [Required]
    public required string Body { get; init; }

    public IDictionary<string, string>? Tags { get; set; }
}