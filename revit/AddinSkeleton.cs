using Autodesk.Revit.UI;
using System;

namespace MotorBloques.Revit
{
    public class AddinSkeleton : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            TaskDialog.Show("MotorBloques", "Add-in conectado correctamente.");
            return Result.Succeeded;
        }
    }
}
