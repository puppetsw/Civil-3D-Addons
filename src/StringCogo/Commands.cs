using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(typeof(StringCogo.Commands))]
namespace StringCogo;

public static class Commands
{
    [CommandMethod("WMS", "_StringCogo", CommandFlags.Modal)]
    public static void StringCogoPoints()
    {
        // Select one CogoPoint
        var peo = new PromptEntityOptions("\nSelect CogoPoint to join: ");
        peo.SetRejectMessage("\nPlease select a CogoPoint. ");
        peo.AddAllowedClass(typeof(CogoPoint), true);

        var entity = CivilApplication.Editor.GetEntity(peo);

        if (entity.Status != PromptStatus.OK)
            return;

        // Find all CogoPoints which matching raw description
        using var tr = CivilApplication.StartTransaction();

        var pickedCogoPoint = (CogoPoint)tr.GetObject(entity.ObjectId, OpenMode.ForRead);

        var cogoPoints = new List<CogoPoint>();
        foreach (ObjectId objectId in CivilApplication.ActiveCivilDocument.CogoPoints)
        {
            var cogoPoint = (CogoPoint)tr.GetObject(objectId, OpenMode.ForRead);

            if (cogoPoint.RawDescription.Contains(pickedCogoPoint.RawDescription))
                cogoPoints.Add(cogoPoint);
        }

        // Sort points by number
        cogoPoints = new List<CogoPoint>(cogoPoints.OrderBy(x => x.PointNumber));

        // Create Poly/3Dpoly that joins the points
        var pointList = cogoPoints.Select(cogoPoint => new Point3d(cogoPoint.Easting, cogoPoint.Northing, cogoPoint.Elevation)).ToList();
        var point3dCollection = new Point3dCollection(pointList.ToArray());

        var poly3d = new Polyline3d(Poly3dType.SimplePoly, point3dCollection, false);

        var bt = (BlockTable) tr.GetObject(CivilApplication.ActiveDatabase.BlockTableId, OpenMode.ForRead);
        var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

        btr.AppendEntity(poly3d);
        tr.AddNewlyCreatedDBObject(poly3d, true);

        tr.Commit();
    }
}

