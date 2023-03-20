using System.Reflection;

namespace Lexer.Helpers;

internal static class PathHelper
{
    internal static readonly string AppDomainBase = AppDomain.CurrentDomain.BaseDirectory;

    internal static string ToAbsoluteDomain(string relative) => Path.Combine(AppDomainBase, relative);
}