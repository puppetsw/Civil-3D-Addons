using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using AverageCogo;

// ReSharper disable UnusedMember.Global

[assembly: CommandClass(typeof(Commands))]
namespace AverageCogo;

public static class Commands
{
    [CommandMethod("WMS", "_AverageCogo", CommandFlags.Modal)]
    public static void StringCogoPoints()
    {
        var entityType = RXObject.GetClass(typeof(CogoPoint));
        var typedValues = new TypedValue[] { new((int)DxfCode.Start, entityType.DxfName) };
        var selectionFilter = new SelectionFilter(typedValues);
        var pso = new PromptSelectionOptions { MessageForAdding = "\nSelect CogoPoints to average: " };
        var selection = CivilApplication.Editor.GetSelection(pso, selectionFilter);

        if (selection.Status != PromptStatus.OK)
            return;

        if (selection.Value.Count < 1)
            return;

        using var tr = CivilApplication.StartTransaction();

        double eastingSum = 0;
        double northingSum = 0;
        double elevationSum = 0;

        foreach (SelectedObject selectedObject in selection.Value)
        {
            var cogoPoint = (CogoPoint)tr.GetObject(selectedObject.ObjectId, OpenMode.ForRead);
            eastingSum += cogoPoint.Easting;
            northingSum += cogoPoint.Northing;
            elevationSum += cogoPoint.Elevation;
        }

        double eastingAvg = eastingSum / selection.Value.Count;
        double northingAvg = northingSum / selection.Value.Count;
        double elevationAvg = elevationSum / selection.Value.Count;

        CivilApplication.ActiveCivilDocument.CogoPoints.Add(new Point3d(eastingAvg, northingAvg, elevationAvg), true);

        tr.Commit();
    }
}
