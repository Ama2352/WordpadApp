using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Wordpad.View
{
    internal class GlobalDashedLineAdorner : Adorner
    {
        private double _lineX = 0; // Vị trí X của đường gạch đứt
        private bool _isVisible = false; // Trạng thái hiển thị của đường gạch đứt
        private double _startY = 0; // Điểm bắt đầu Y của đường
        public GlobalDashedLineAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        public void UpdateLine(double x, double startY, bool isVisible)
        {
            _lineX = x;
            _startY = startY;
            _isVisible = isVisible;
            InvalidateVisual(); // Yêu cầu redraw
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!_isVisible) return;

            // Tạo bút gạch đứt
            Pen pen = new Pen(Brushes.Gray, 1)
            {
                DashStyle = new DashStyle(new double[] { 4, 2 }, 0)
            };

            // Vẽ đường gạch đứt
            double height = AdornedElement.RenderSize.Height;
            drawingContext.DrawLine(pen, new Point(_lineX, _startY), new Point(_lineX, height));
        }
    }
}
