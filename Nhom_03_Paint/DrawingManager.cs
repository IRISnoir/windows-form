using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Nhom_03_Paint
{
    internal class DrawingManager
    {
        // Danh sách các hình được vẽ
        private List<Shape> shapes = new List<Shape>();

        // Biến hỗ trợ Double Buffering
        private Bitmap backBuffer;
        private Graphics backGraphics;

        public DrawingManager()
        {
        }

        /// <summary>
        /// Thêm hình mới vào danh sách
        /// </summary>
        public void AddShape(Shape shape)
        {
            if (shape != null)
            {
                shapes.Add(shape);
            }
        }

        /// <summary>
        /// Xóa hình cuối cùng
        /// </summary>
        public void RemoveLastShape()
        {
            if (shapes.Count > 0)
            {
                shapes.RemoveAt(shapes.Count - 1);
            }
        }

        /// <summary>
        /// Xóa tất cả hình
        /// </summary>
        public void ClearAll()
        {
            shapes.Clear();
        }

        /// <summary>
        /// Lấy danh sách hình hiện tại
        /// </summary>
        public List<Shape> GetShapes()
        {
            return shapes;
        }

        /// <summary>
        /// Vẽ tất cả hình với Double Buffering để tránh giật lag
        /// </summary>
        public void DrawAll(Graphics g, int width, int height)
        {
            if (backBuffer == null || backBuffer.Width != width || backBuffer.Height != height)
            {
                backBuffer?.Dispose();
                backBuffer = new Bitmap(width, height);
                backGraphics?.Dispose();
                backGraphics = Graphics.FromImage(backBuffer);
            }

            // Clear backbuffer
            backGraphics.Clear(Color.White);

            // Vẽ tất cả hình lên backbuffer
            foreach (var shape in shapes)
            {
                if (shape != null)
                {
                    // Lưu trạng thái Transform hiện tại
                    var state = backGraphics.Save();

                    // Nếu có góc xoay, apply RotateTransform
                    if (shape.RotationAngle != 0)
                    {
                        // Tìm điểm trung tâm của hình
                        Rectangle bounds = shape.GetBoundingRectangle();
                        float centerX = bounds.X + bounds.Width / 2f;
                        float centerY = bounds.Y + bounds.Height / 2f;

                        backGraphics.TranslateTransform(centerX, centerY);
                        backGraphics.RotateTransform(shape.RotationAngle);
                        backGraphics.TranslateTransform(-centerX, -centerY);
                    }

                    // Vẽ hình
                    shape.Draw(backGraphics);

                    // Restore trạng thái Transform
                    backGraphics.Restore(state);
                }
            }

            // Sao chép từ backbuffer lên màn hình
            g.DrawImageUnscaled(backBuffer, 0, 0);
        }

        /// <summary>
        /// Lưu Panel thành file hình ảnh (JPG, PNG, BMP)
        /// </summary>
        public bool SaveImage(string filePath, Image imageToSave)
        {
            try
            {
                if (imageToSave == null)
                    return false;

                string ext = Path.GetExtension(filePath).ToLower();

                switch (ext)
                {
                    case ".jpg":
                    case ".jpeg":
                        imageToSave.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".png":
                        imageToSave.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".bmp":
                        imageToSave.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    default:
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu file: {ex.Message}", "Error");
                return false;
            }
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        public void Dispose()
        {
            backGraphics?.Dispose();
            backBuffer?.Dispose();
        }

        /// <summary>
        /// Lấy số lượng hình hiện tại
        /// </summary>
        public int GetShapeCount()
        {
            return shapes.Count;
        }
    }
}
