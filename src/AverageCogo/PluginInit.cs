using Autodesk.AutoCAD.Runtime;
using AverageCogo;

[assembly: ExtensionApplication(typeof(PluginInit))]
namespace AverageCogo;

public class PluginInit : IExtensionApplication
{
    public void Initialize()
    {
    }

    public void Terminate()
    {
    }
}
