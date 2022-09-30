using Autodesk.AutoCAD.Runtime;
using StringCogo;

[assembly: ExtensionApplication(typeof(PluginInit))]
namespace StringCogo;

public class PluginInit : IExtensionApplication
{
    public void Initialize()
    {
    }

    public void Terminate()
    {
    }
}
