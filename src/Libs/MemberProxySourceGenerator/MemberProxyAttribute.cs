using System;

namespace MemberProxySourceGenerator;

/// <summary>
/// Place on a parameter in a constructor to indicate that members which proxy to a field with that instance should be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class MemberProxyAttribute : Attribute
{
    public MemberProxyAttribute(MemberTypes memberFlags)
    {
        MemberFlags = memberFlags;
    }

    public MemberTypes MemberFlags { get; }
}
