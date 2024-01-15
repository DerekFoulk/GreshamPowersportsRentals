using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BlazorApp.Client.Shared
{
    [SupportedOSPlatform("browser")]
    public partial class ModelSelector
    {
        [JSImport("scrollToModelSelector", nameof(ModelSelector))]
        internal static partial void ScrollToModelSelector();
    }
}
