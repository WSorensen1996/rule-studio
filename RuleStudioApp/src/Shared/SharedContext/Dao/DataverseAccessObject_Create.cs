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
    /// Generic method that creates an entity of type <typeparamref name="T"/> in XRM
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="callerMethodName"></param>
    /// <returns></returns>
    public Guid Create<T>(T entity, [CallerMemberName] string callerMethodName = "") where T : Entity
    {
        return DataverseWrapAccess.WrapFunction(logger, "Create", entity.LogicalName, () => orgService.Create(DataverseWrapAccess.Plain(entity)), callerMethodName, $" with data '{DataverseWrapAccess.GetAttributesFormatted(entity)}'");
    }
}