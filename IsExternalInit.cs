// ReSharper disable once CheckNamespace

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// The type System.Runtime.CompilerServices.IsExternalInit is required for full record support as it uses init only setters, but is only available in .NET 5 and later (which Unity doesn’t support).
    /// Working around this issue by declaring the System.Runtime.CompilerServices.IsExternalInit type in their own projects.
    ///
    /// https://docs.unity3d.com/2021.2/Documentation/Manual/CSharpCompiler.html
    /// </summary>
    public class IsExternalInit { }
}