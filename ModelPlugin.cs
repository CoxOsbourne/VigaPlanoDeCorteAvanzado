using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;

namespace VigaPlanoDeCorteAvanzado

{
    public class PluginData
    {
        #region Fields
        //
        // Define the fields specified on the Form.
        //
        /* Some examples:
        [StructuresField("RebarName")]
        public string RebarName;

        [StructuresField("RebarSize")]
        public string RebarSize;

        [StructuresField("RebarGrade")]
        public string RebarGrade;

        [StructuresField("RebarBendingRadius")]
        public string RebarBendingRadius;

        [StructuresField("RebarClass")]
        public int RebarClass;

        [StructuresField("RebarSpacing")]
        public double RebarSpacing;
        */
        #endregion
    }

    [Plugin("VigaPlanoDeCorteAvanzado")]
    [PluginUserInterface("VigaPlanoDeCorteAvanzado.MainForm")]
    public class VigaPlanoDeCorteAvanzado : PluginBase
    {
        #region Fields
        private Model _Model;
        private PluginData _Data;
        //
        // Define variables for the field values.
        //
        /* Some examples:
        private string _RebarName = string.Empty;
        private string _RebarSize = string.Empty;
        private string _RebarGrade = string.Empty;
        private ArrayList _RebarBendingRadius = new ArrayList();
        private int _RebarClass = new int();
        private double _RebarSpacing;
        */
        #endregion

        #region Properties
        private Model Model
        {
            get { return this._Model; }
            set { this._Model = value; }
        }

        private PluginData Data
        {
            get { return this._Data; }
            set { this._Data = value; }
        }
        #endregion

        #region Constructor
        public VigaPlanoDeCorteAvanzado(PluginData data)
        {
            Model = new Model();
            Data = data;
        }
        #endregion

        #region Overrides
        public override List<InputDefinition> DefineInput()
        {
            //
            // This is an example for selecting two points; change this to suit your needs.
            //
            //List<InputDefinition> PointList = new List<InputDefinition>();
            //Picker Picker = new Picker();
            //ArrayList PickedPoints = Picker.PickPoints(Picker.PickPointEnum.PICK_TWO_POINTS);

            //PointList.Add(new InputDefinition(PickedPoints));

            //return PointList;
            return new List<InputDefinition>(); // No se seleccionan puntos. No points selected
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                GetValuesFromDialog();

                //
                // This is an example for selecting two points; change this to suit your needs.
                //
                //ArrayList Points = (ArrayList)Input[0].GetInput();
                //Point StartPoint = Points[0] as Point;
                //Point EndPoint = Points[1] as Point;

                //
                // Write your code here; better yet, create private methods and call them from here.
                //
                if (Model.GetConnectionStatus())
                {
                    CrearVigaX(-150, 0, 3000+(2*150));
                    CrearVigaX(-150, 8000, 3000+(2*150));
                    CrearVigaY(0, -150, 8000+(2*150));
                    CrearVigaY(3000, -150, 8000+(2*150));
                    Model.CommitChanges();
                }


            }
            catch (Exception Exc)
            {
                MessageBox.Show(Exc.ToString());
            }

            return true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the values from the dialog and sets the default values if needed
        /// </summary>
        private void GetValuesFromDialog()
        {
            /* Some examples:
            _RebarName = Data.RebarName;
            _RebarSize = Data.RebarSize;
            _RebarGrade = Data.RebarGrade;

            char[] Parameters = { ' ' };
            string[] Radiuses = Data.RebarBendingRadius.Split(Parameters, StringSplitOptions.RemoveEmptyEntries);

            foreach (string Item in Radiuses)
                _RebarBendingRadius.Add(Convert.ToDouble(Item));

            _RebarClass = Data.RebarClass;
            _RebarSpacing = Data.RebarSpacing;

            if (IsDefaultValue(_RebarName))
                _RebarName = "REBAR";
            if (IsDefaultValue(_RebarSize))
                _RebarSize = "12";
            if (IsDefaultValue(_RebarGrade))
                _RebarGrade = "Undefined";
            if (_RebarBendingRadius.Count < 1)
                _RebarBendingRadius.Add(30.00);
            if (IsDefaultValue(_RebarClass) || _RebarClass <= 0)
                _RebarClass = 99;
            if (IsDefaultValue(_RebarSpacing) || _RebarSpacing <= 0)
                _RebarSpacing = 200.0;
            */
        }

        // Write your private methods here.
        private static ModelObject CrearVigaX(double PositionX, double PositionY, double LengthX) /*viga a lo largo del eje x ; beam throughout x axis*/
        {
            Beam VigaX = new Beam();
            VigaX.Name = "VIGA";
            VigaX.Profile.ProfileString = "HEB300";
            VigaX.Material.MaterialString = "S275JR";
            VigaX.Class = "2";
            VigaX.StartPoint.X = PositionX;
            VigaX.StartPoint.Y = PositionY;
            VigaX.StartPoint.Z = 0.0;
            VigaX.EndPoint.X = PositionX + LengthX; /*punto final de la viga= punto inicial + largo de la viga; end point = starting point + beam length*/
            VigaX.EndPoint.Y = PositionY;
            VigaX.EndPoint.Z = 0.0;
            VigaX.Position.Rotation = Position.RotationEnum.TOP;
            VigaX.Position.Plane = Position.PlaneEnum.MIDDLE;
            VigaX.Position.Depth = Position.DepthEnum.FRONT;
            if (!VigaX.Insert())
            {
                Console.WriteLine("Insertion of beam failed.");
            }
            if(PositionY==0) /*creación de planos de corte para la viga que empieza en (0,0,0); cutting planes generation for the beam that starts in (0,0,0)*/
            {
                CutPlane plane1 = new CutPlane(); /*plano de corte extremo inicial; starting point cutting plane*/
                plane1.Father = VigaX;
                plane1.Plane = new Plane();
                plane1.Plane.Origin = new Point(-150, -150); /*HEB 300, divides 300/2 y de ahí sale el 150; HEB300, divide 300/2 and there you have 150*/
                plane1.Plane.AxisX = new Vector(1, 1, 0); /*visto en el entorno de Tekla esto es muy gráfico; if you open Tekla Structures, this is self-explanatory*/
                plane1.Plane.AxisY = new Vector(0, 0, -1); /*ojo con el signo, porque determina dónde se elimina material; careful with the sign, because it determines where the material will be removed*/
                plane1.Insert();

                CutPlane plane2 = new CutPlane(); /*plano de corte en el extremo final; end point cutting plane*/
                plane2.Father = VigaX;
                plane2.Plane = new Plane();
                plane2.Plane.Origin = new Point(LengthX-150, -150); /*Hay que tener en cuenta el ancho del perfil. Accounting for profile width */
                plane2.Plane.AxisX = new Vector(-1, 1, 0);
                plane2.Plane.AxisY = new Vector(0, 0, 1);
                plane2.Insert();

            }
            else /*creación de planos de corte para la viga que empieza en el extremo opuesto del bastidor (y!=0);  cutting planes generation for the beam that starts in the opposite end of the frame (y!=0)*/
            {

                    CutPlane plane1 = new CutPlane();
                    plane1.Father = VigaX;
                    plane1.Plane = new Plane();
                    plane1.Plane.Origin = new Point(-150, PositionY+150);
                    plane1.Plane.AxisX = new Vector(1, -1, 0);
                    plane1.Plane.AxisY = new Vector(0, 0, 1);
                    plane1.Insert();

                    CutPlane plane2 = new CutPlane();
                    plane2.Father = VigaX;
                    plane2.Plane = new Plane();
                    plane2.Plane.Origin = new Point(LengthX - 150, PositionY+150);
                    plane2.Plane.AxisX = new Vector(-1, -1, 0);
                    plane2.Plane.AxisY = new Vector(0, 0, -1);
                    plane2.Insert();

                
            }
            
             
            return VigaX;
        }

        private static ModelObject CrearVigaY(double PositionX, double PositionY, double LengthY) /*creación de vigas que van a lo largo del eje y; beam generation throughout y-axis*/
        {
            Beam VigaY = new Beam();
            VigaY.Name = "VIGA";
            VigaY.Profile.ProfileString = "HEB300";
            VigaY.Material.MaterialString = "S275JR";
            VigaY.Class = "2";
            VigaY.StartPoint.X = PositionX;
            VigaY.StartPoint.Y = PositionY;
            VigaY.StartPoint.Z = 0.0;
            VigaY.EndPoint.X = PositionX;
            VigaY.EndPoint.Y = PositionY+LengthY;
            VigaY.EndPoint.Z = 0.0;
            VigaY.Position.Rotation = Position.RotationEnum.TOP;
            VigaY.Position.Plane = Position.PlaneEnum.MIDDLE;
            VigaY.Position.Depth = Position.DepthEnum.FRONT;
            if (!VigaY.Insert())
            {
                Console.WriteLine("Insertion of beam failed.");
            }
            if (PositionX == 0)
            {
                CutPlane plane1 = new CutPlane();
                plane1.Father = VigaY;
                plane1.Plane = new Plane();
                plane1.Plane.Origin = new Point(-150, -150);
                plane1.Plane.AxisX = new Vector(1, 1, 0);
                plane1.Plane.AxisY = new Vector(0, 0, 1);
                plane1.Insert();

                CutPlane plane2 = new CutPlane();
                plane2.Father = VigaY;
                plane2.Plane = new Plane();
                plane2.Plane.Origin = new Point(-150,LengthY-150);
                plane2.Plane.AxisX = new Vector(1, -1, 0);
                plane2.Plane.AxisY = new Vector(0, 0, -1);
                plane2.Insert();

            }
            else
            {
                CutPlane plane1 = new CutPlane();
                plane1.Father = VigaY;
                plane1.Plane = new Plane();
                plane1.Plane.Origin = new Point(PositionX+150, -150);
                plane1.Plane.AxisX = new Vector(-1, 1, 0);
                plane1.Plane.AxisY = new Vector(0, 0, -1);
                plane1.Insert();

                CutPlane plane2 = new CutPlane();
                plane2.Father = VigaY;
                plane2.Plane = new Plane();
                plane2.Plane.Origin = new Point(PositionX+150, LengthY - 150);
                plane2.Plane.AxisX = new Vector(-1, -1, 0);
                plane2.Plane.AxisY = new Vector(0, 0, 1);
                plane2.Insert();
            }

             
            return VigaY;
        }

        #endregion
    }
}
