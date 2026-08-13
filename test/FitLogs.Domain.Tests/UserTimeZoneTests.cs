using System;
using Shouldly;
using Xunit;

namespace FitLogs.UserProfiles;

public class UserTimeZoneTests
{
    [Fact]
    public void Spring_forward_day_uses_a_23_hour_utc_window()
    {
        var range = UserTimeZone.GetUtcDateRange(new DateOnly(2024, 3, 10), "America/New_York");

        range.StartUtc.ShouldBe(new DateTime(2024, 3, 10, 5, 0, 0, DateTimeKind.Utc));
        range.EndUtc.ShouldBe(new DateTime(2024, 3, 11, 4, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Fall_back_day_uses_a_25_hour_utc_window()
    {
        var range = UserTimeZone.GetUtcDateRange(new DateOnly(2024, 11, 3), "America/New_York");

        range.StartUtc.ShouldBe(new DateTime(2024, 11, 3, 4, 0, 0, DateTimeKind.Utc));
        range.EndUtc.ShouldBe(new DateTime(2024, 11, 4, 5, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Local_today_is_not_derived_from_the_server_day()
    {
        var utc = new DateTime(2024, 1, 1, 17, 30, 0, DateTimeKind.Utc);

        UserTimeZone.GetLocalDate(utc, "Asia/Ho_Chi_Minh")
            .ShouldBe(new DateOnly(2024, 1, 2));
    }

    [Fact]
    public void Local_clock_value_is_not_reinterpreted_as_utc()
    {
        var localClockValue = new DateTime(2024, 1, 1, 23, 30, 0, DateTimeKind.Local);

        UserTimeZone.GetLocalDate(localClockValue, "Asia/Ho_Chi_Minh")
            .ShouldBe(new DateOnly(2024, 1, 1));
    }

    [Fact]
    public void Stored_local_date_range_keeps_the_selected_wall_clock_day()
    {
        var range = UserTimeZone.GetStoredLocalDateRange(new DateOnly(2024, 1, 13));

        range.Start.ShouldBe(new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Unspecified));
        range.End.ShouldBe(new DateTime(2024, 1, 14, 0, 0, 0, DateTimeKind.Unspecified));
    }
}
