using System;

namespace FitLogs.UserProfiles;

/// <summary>Converts local calendar dates to UTC ranges without depending on the server's local zone.</summary>
public static class UserTimeZone
{
    /// <summary>Checks that a time-zone identifier is recognized by the host's IANA time-zone database.</summary>
    public static bool IsValid(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return false;
        }

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }

    /// <summary>Returns the user's local calendar date for a UTC instant.</summary>
    public static DateOnly GetLocalDate(DateTime utcNow, string timeZoneId)
    {
        var utc = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc, zone));
    }

    /// <summary>Returns a half-open UTC range covering one complete local calendar day.</summary>
    public static (DateTime StartUtc, DateTime EndUtc) GetUtcDateRange(DateOnly localDate, string timeZoneId)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var localStart = DateTime.SpecifyKind(localDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
        var localEnd = DateTime.SpecifyKind(localDate.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);

        return (ConvertBoundary(localStart, zone), ConvertBoundary(localEnd, zone));
    }

    private static DateTime ConvertBoundary(DateTime localBoundary, TimeZoneInfo zone)
    {
        // Rare zones can skip midnight; move forward to the first representable instant.
        while (zone.IsInvalidTime(localBoundary))
        {
            localBoundary = localBoundary.AddMinutes(1);
        }

        return TimeZoneInfo.ConvertTimeToUtc(localBoundary, zone);
    }
}
