using AegisLink.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace AegisLink.Tests.Server;

public class AegisLinkDbContextTests
{
    [Fact]
    public void ModelConfiguration_SetsPrimaryKeyAndUniqueIndex()
    {
        var options = new DbContextOptionsBuilder<AegisLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AegisLinkDbContext(options);

        var entityType = context.Model.FindEntityType(typeof(UserKey));
        Assert.NotNull(entityType);

        var primaryKey = entityType!.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey!.Properties);
        Assert.Equal(nameof(UserKey.AegisId), primaryKey.Properties[0].Name);

        var publicKeyIndex = entityType.GetIndexes()
            .Single(index => index.Properties.Single().Name == nameof(UserKey.PublicKey));
        Assert.True(publicKeyIndex.IsUnique);
    }
}
