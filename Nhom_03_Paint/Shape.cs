using System;
using System.Drawing;

namespace Nhom_03_Paint
{
    internal abstract class Shape
    {
        // Thuộc tính cơ bản
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Color BorderColor { get; set; } = Color.Black;
        public Color FillColor { get; set; } = Color.White;
        public int BorderWidth { get; set; } = 1;
        public Brush Brush { get; set; }
        public int RotationAngle { get; set; } = 0; // Góc xoay

        // Constructor
        protected Shape()
        {
            Brush = new SolidBrush(FillColor);
        }

        // Phương thức trừu tượng - các hình sẽ phải triển khai
        public abstract void Draw(Graphics g);

        // Phương thức hỗ trợ xoay
        public virtual void Rotate(int angle)
        {
            RotationAngle = (RotationAngle + angle) % 360;
        }

        // Phương thức hỗ trợ reset góc xoay
        public virtual void ResetRotation()
        {
            RotationAngle = 0;
        }

        // Tính giá trị tuyệt đối của chiều rộng
        public int GetWidth()
        {
            return Math.Abs(EndPoint.X - StartPoint.X);
        }

        // Tính giá trị tuyệt đối của chiều cao
        public int GetHeight()
        {
            return Math.Abs(EndPoint.Y - StartPoint.Y);
        }

        // Lấy Rectangle từ 2 điểm (xử lý vẽ ngược)
        public Rectangle GetBoundingRectangle()
        {
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = GetWidth();
            int height = GetHeight();

            return new Rectangle(x, y, width, height);
        }
    }
}
