using AegisLink.Server.Services;

namespace AegisLink.Tests.Server;

public class HubSessionTrackerTests
{
    [Fact]
    public void Join_AddsMember()
    {
        var tracker = new HubSessionTracker();

        tracker.Join("conn-1", "session-1");

        Assert.True(tracker.IsMember("conn-1", "session-1"));
    }

    [Fact]
    public void IsMember_ReturnsFalseWhenMissing()
    {
        var tracker = new HubSessionTracker();

        Assert.False(tracker.IsMember("conn-1", "session-1"));
    }

    [Fact]
    public void Remove_ClearsMembership()
    {
        var tracker = new HubSessionTracker();
        tracker.Join("conn-1", "session-1");

        tracker.Remove("conn-1");

        Assert.False(tracker.IsMember("conn-1", "session-1"));
    }
}
