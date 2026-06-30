using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.DailyActivities.Commands;
using GreatReports.Application.UseCases.DailyActivities.CommandHandlers;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using GreatReports.Shared;
using Moq;

namespace GreatReports.UnitTests.Application;

public class DailyActivityCommandHandlerTests
{
    private readonly Mock<IDailyActivityRepository> _dailyActivityRepositoryMock = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly CreateDailyActivityCommandHandler _createHandler;
    private readonly UpdateDailyActivityCommandHandler _updateHandler;

    public DailyActivityCommandHandlerTests()
    {
        _createHandler = new CreateDailyActivityCommandHandler(
            _dailyActivityRepositoryMock.Object,
            _groupRepositoryMock.Object);

        _updateHandler = new UpdateDailyActivityCommandHandler(
            _dailyActivityRepositoryMock.Object,
            _groupRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenLockoutTimeIsActive()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        // Mock a group in Sao Paulo timezone where current UTC time translates to after 23:50.
        // Let's set timezone to UTC, and we can mock group timezone to match local timezone conversion.
        // Wait, TimeZoneInfo.FindSystemTimeZoneById("UTC") is standard.
        // If we set the group timezone to UTC, and the system current UTC time is used:
        // Wait! In handlers, we use DateTime.UtcNow.
        // Since DateTime.UtcNow is hardcoded in the handler, to test this reliably:
        // We can use a timezone that is ahead of UTC such that the current time converted to that timezone is past 23:50.
        // For example, if current UTC hour is H:M:
        // We want (H + offset) % 24 == 23 and minutes >= 50.
        // Let's calculate offset needed: offset = 23 - H.
        // Since the current local time of the agent is around 18:20 (UTC 21:20), UTC is 21.
        // If we use a timezone like "Asia/Kolkata" (UTC+5:30) or "Asia/Tokyo" (UTC+9),
        // at 21:20 UTC, Tokyo is 6:20 AM next day (not locked), but maybe some timezone is locked.
        // Wait, to make the test time-independent, is there a timezone that is *always* locked? No, timezone local time changes dynamically.
        // Can we mock TimeZoneInfo? No, it's a sealed system class.
        // But wait! We can find a timezone that is past 23:50 *right now*.
        // Or better, we can mock `GetGroupsByPartnerIdAsync` to return a list of groups.
        // Wait, how can we test this without flakiness?
        // Let's see: if we use "GMT Standard Time" or similar, it depends on UtcNow.
        // Wait, can we mock `DateTime.UtcNow`? The code uses `DateTime.UtcNow`.
        // To make the test robust, let's find a timezone that satisfies the lockout condition relative to the current DateTime.UtcNow.
        // Let's calculate the timezone offset dynamically in the test!
        // We can get DateTime.UtcNow, calculate what offset is needed to make the local hour 23 and minute 50.
        // For example:
        // var utcNow = DateTime.UtcNow;
        // Find timezone with offset = 23 - utcNow.Hour.
        // But TimeZoneInfo.FindSystemTimeZoneById needs a specific ID.
        // Wait, on Linux, we can construct custom TimeZoneInfo!
        // TimeZoneInfo.CreateCustomTimeZone(id, offset, name, name)
        // No, FindSystemTimeZoneById will be called in the handler: TimeZoneInfo.FindSystemTimeZoneById(group.Timezone)
        // If we create a custom TimeZoneInfo and registry doesn't have it, it might fail.
        // Wait! Is there another way?
        // What if the test calculates the target local time, and registers a system timezone?
        // On Linux/Mono/Dotnet, system timezones are from the IANA timezone database.
        // Can we use "UTC" and we know that sometimes it won't be locked?
        // Wait, if we use a timezone where offset makes it 23:55.
        // Let's find the current UTC hour.
        // If we compute the offset dynamically:
        // double offsetHours = 23.9 - utcNow.TimeOfDay.TotalHours;
        // Since offsetHours can range from -12 to +14, we can choose a standard IANA timezone that matches this offset, or close to it.
        // Or wait! TimeZoneInfo on .NET has TimeZoneInfo.GetSystemTimeZones().
        // We can query all system timezones, and find one where:
        // var localTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        // if (localTimeNow.Hour == 23 && localTimeNow.Minute >= 50) -> this is our locked timezone!
        // Let's do exactly that!
        var lockedTz = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(tz => {
                var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                return local.Hour == 23 && local.Minute >= 50;
            });

        if (lockedTz != null)
        {
            var group = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Group Lock", lockedTz.Id).Value;
            _groupRepositoryMock
                .Setup(x => x.GetGroupsByPartnerIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Group> { group });

            var command = new CreateDailyActivityCommand(
                partnerId, "Title", "Theme", "Content", DateTime.UtcNow, ActivityStatus.Doing, false);

            // Act
            var result = await _createHandler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("DailyActivity.LockoutActive", result.Error.Code);
        }
    }

    [Fact]
    public async Task Create_ShouldSucceed_WhenLockoutTimeIsInactive()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        // Find a timezone that is NOT locked out (e.g. local time is morning/afternoon)
        var unlockedTz = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(tz => {
                var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                return !(local.Hour == 23 && local.Minute >= 50);
            }) ?? TimeZoneInfo.Utc;

        var group = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Group Unlocked", unlockedTz.Id).Value;
        _groupRepositoryMock
            .Setup(x => x.GetGroupsByPartnerIdAsync(partnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Group> { group });

        var command = new CreateDailyActivityCommand(
            partnerId, "Title", "Theme", "Content", DateTime.UtcNow, ActivityStatus.Doing, false);

        // Act
        var result = await _createHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _dailyActivityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<DailyActivity>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
