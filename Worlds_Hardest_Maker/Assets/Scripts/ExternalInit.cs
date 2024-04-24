// This fixes "The predefined type 'System.Runtime.CompilerServices.IsExternalInit' must be defined or imported in order to declare init-only setter"
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}