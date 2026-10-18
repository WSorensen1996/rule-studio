using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace SharedContext.Dao;

public partial class DataverseAccessObject : IDataverseAccessObject
{
    /// <summary>
    /// Generic method that updates an entity of type <typeparamref name="T"/> in XRM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="callerMethodName"></param>
    public void Update<T>(T entity, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        DataverseWrapAccess.WrapAction(logger, $"Update({entity.Id})", entity.LogicalName, () => orgService.Update(DataverseWrapAccess.Plain(entity)), callerMethodName, $" with data '{DataverseWrapAccess.GetAttributesFormatted(entity)}'");
    }
}