using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADCommands
{
    public class ComandosCAD
    {
        #region Funciones Básicas
        public double DistanceFrom(Curve c, Point3d pt)
        {
            return pt.DistanceTo(c.GetClosestPointTo(pt, false));
        }
        public Line DrawLine(Transaction tr,BlockTableRecord btr, Point3d pt1, Point3d pt2)
        {
            Line ln = new Line(pt1, pt2);
            btr.AppendEntity(ln);
            tr.AddNewlyCreatedDBObject(ln, true);
            return ln;
        }
        public Line ConnectPointToCurve(Transaction tr, BlockTableRecord btr, Point3d pt1, Curve c)
        {
            Line linea1;
            linea1 = DrawLine(tr, btr, pt1, c.GetClosestPointTo(pt1, false));
            return linea1;
        }

        #endregion
        #region Commandos
        [CommandMethod("B2C")]
        public void Block2Curve()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc==null)
            {
                return;
            }
            Database db = doc.Database;
            Editor ed = doc.Editor;
            //Solicitud al usuario de un nombre de bloque
            PromptEntityOptions Peo = new PromptEntityOptions("\nSeleccione el bloque: ");
            Peo.SetRejectMessage("Debe ser un bloque");
            Peo.AddAllowedClass(typeof(BlockReference), false);
            PromptEntityResult Per = ed.GetEntity(Peo);
            if (Per.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId brID = Per.ObjectId;
            PromptEntityOptions Peo2 = new PromptEntityOptions("\nSeleccione la curva: ");
            Peo2.SetRejectMessage("\n");
            Peo2.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult Per2 = ed.GetEntity(Peo2);
            if (Per.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId cId = Per2.ObjectId;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference br=tr.GetObject(brID, OpenMode.ForRead) as BlockReference;
                Curve c = tr.GetObject(cId, OpenMode.ForRead) as Curve;
                if (br!=null && c!=null)
                {
                    BlockTableRecord btr = tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;
                    ConnectPointToCurve(tr, btr, br.Position, c);
                }
                tr.Commit();
            }              
        }
        #endregion
    }
}
