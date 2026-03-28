using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nhom_03_Paint
{
    internal class AppEnums
    {
    }

    public enum ShapeType
    {
        Line,
        Rectangle,
        Square,
        Ellipse,
        Circle,
        Triangle,
        Parallelogram,
        Text
    }

    public enum BrushStyle
    {
        Solid,
        Hatch,
        LinearGradient,
        PathGradient,
        Texture
    }
}
