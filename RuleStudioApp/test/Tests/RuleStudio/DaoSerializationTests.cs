using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace Tests.RuleStudio;

/// <summary>
/// The sandbox serializes what plug-ins send to the organization service with
/// DataContractSerializer, and the generated ExtendedEntity base is not a data contract. The DAO
/// must therefore send plain Entity objects on Create and Update.
/// </summary>
public class DaoSerializationTests
{
    [Fact]
    public void CreateAndUpdateSendPlainSerializableEntities()
    {
        var sent = new List<Entity>();
        var service = Substitute.For<IOrganizationService>();
        service.Create(Arg.Do<Entity>(sent.Add)).Returns(Guid.NewGuid());
        service.Update(Arg.Do<Entity>(sent.Add));
        var dao = new DataverseAccessObject(service, Substitute.For<ILogger>());

        dao.Create(new rs_Release { rs_Name = "TEST – release", rs_Status = rs_release_rs_status.Live });
        dao.Update(new rs_Rule(Guid.NewGuid()) { rs_Name = "TEST – rule", rs_PublishedVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, Guid.NewGuid()) });

        sent.Should().HaveCount(2);
        sent.Should().OnlyContain(e => e.GetType() == typeof(Entity));
        sent[0]["rs_name"].Should().Be("TEST – release");
        sent[1].Id.Should().NotBeEmpty();
        foreach (var entity in sent)
        {
            using var stream = new MemoryStream();
            new DataContractSerializer(typeof(Entity)).WriteObject(stream, entity);
            stream.Length.Should().BePositive();
        }
    }

    [Fact]
    public void ProxyTypesThemselvesDoNotSerialize()
    {
        using var stream = new MemoryStream();
        var serialize = () => new DataContractSerializer(typeof(Entity)).WriteObject(stream, new rs_Release { rs_Name = "x" });
        serialize.Should().Throw<InvalidDataContractException>("this is why the DAO converts proxies to plain entities");
    }
}
