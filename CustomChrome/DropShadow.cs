﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace CustomChrome
{
    internal class DropShadow : Control
    {
        static DropShadow()
        {
            Application.AddMessageFilter(new NonClientRedirector());
        }

        private readonly Form _form;
        private ImageCache _imageCache;
        private int _cornerSize;
        private int _borderWidth;

        public bool IsMaximized
        {
            get { return (NativeMethods.GetWindowLong(_form.Handle, NativeMethods.GWL_STYLE) & NativeMethods.WS_MAXIMIZE) != 0; }
        }

        public DropShadowBorder Border { get; private set; }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                cp.Style = NativeMethods.WS_POPUP | NativeMethods.WS_CLIPSIBLINGS | NativeMethods.WS_CLIPCHILDREN;
                cp.ExStyle = NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_LAYERED;

                return cp;
            }
        }

        public ImageCache ImageCache
        {
            get { return _imageCache; }
            set
            {
                if (_imageCache != value)
                {
                    _imageCache = value;

                    if (_imageCache != null)
                    {
                        Debug.Assert(_imageCache.CornerNE.Height == _imageCache.CornerNE.Width);

                        _cornerSize = _imageCache.CornerNE.Height;
                        _borderWidth = _imageCache.BorderN.Height;

                        Invalidate();
                    }
                }
            }
        }

        public DropShadow(Form form, DropShadowBorder border)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            TabStop = false;

            _form = form;

            Border = border;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var outerBounds = new Rectangle(0, 0, Width, Height);

            using (var target = new Bitmap(outerBounds.Width, outerBounds.Height, PixelFormat.Format32bppArgb))
            {
                using (var targetGraphics = Graphics.FromImage(target))
                {
                    // The top and bottom borders extend over the sides of the window.
                    // The left and right borders do no. This means that we need to
                    // update the bounds to make it seem like the left and right
                    // borders do extend outside of the window.

                    if (Border == DropShadowBorder.Left || Border == DropShadowBorder.Right)
                    {
                        outerBounds = new Rectangle(
                            outerBounds.Left,
                            outerBounds.Top - _borderWidth,
                            outerBounds.Width,
                            outerBounds.Height + _borderWidth * 2
                        );
                    }

                    if (Border == DropShadowBorder.Left || Border == DropShadowBorder.Top)
                    {
                        DrawImage(
                            targetGraphics,
                            _imageCache.CornerNW,
                            new Point(
                                outerBounds.Left,
                                outerBounds.Top
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Right || Border == DropShadowBorder.Top)
                    {
                        DrawImage(
                            targetGraphics,
                            _imageCache.CornerNE,
                            new Point(
                                outerBounds.Right - _cornerSize,
                                outerBounds.Top
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Bottom || Border == DropShadowBorder.Left)
                    {
                        DrawImage(
                            targetGraphics,
                            _imageCache.CornerSW,
                            new Point(
                                outerBounds.Left,
                                outerBounds.Bottom - _cornerSize
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Bottom || Border == DropShadowBorder.Right)
                    {
                        DrawImage(
                            targetGraphics,
                            _imageCache.CornerSE,
                            new Point(
                                outerBounds.Right - _cornerSize,
                                outerBounds.Bottom - _cornerSize
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Top)
                    {
                        DrawBorder(
                            targetGraphics,
                            _imageCache.BorderN,
                            new Rectangle(
                                outerBounds.Left + _cornerSize,
                                outerBounds.Top,
                                outerBounds.Width - _cornerSize * 2,
                                _borderWidth
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Bottom)
                    {
                        DrawBorder(
                            targetGraphics,
                            _imageCache.BorderS,
                            new Rectangle(
                                outerBounds.Left + _cornerSize,
                                outerBounds.Bottom - _borderWidth,
                                outerBounds.Width - _cornerSize * 2,
                                _borderWidth
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Left)
                    {
                        DrawBorder(
                            targetGraphics,
                            _imageCache.BorderW,
                            new Rectangle(
                                outerBounds.Left,
                                outerBounds.Top + _cornerSize,
                                _borderWidth,
                                outerBounds.Height - _cornerSize * 2
                            )
                        );
                    }

                    if (Border == DropShadowBorder.Right)
                    {
                        DrawBorder(
                            targetGraphics,
                            _imageCache.BorderE,
                            new Rectangle(
                                outerBounds.Right - _borderWidth,
                                outerBounds.Top + _cornerSize,
                                _borderWidth,
                                outerBounds.Height - _cornerSize * 2
                            )
                        );
                    }
                }

                // Get device contexts
                var screenDc = NativeMethods.GetDC(IntPtr.Zero);
                var memDc = NativeMethods.CreateCompatibleDC(screenDc);
                var hBitmap = IntPtr.Zero;
                var hOldBitmap = IntPtr.Zero;

                try
                {
                    // Get handle to the new bitmap and select it into the current device context
                    hBitmap = target.GetHbitmap(Color.FromArgb(0));
                    hOldBitmap = NativeMethods.SelectObject(memDc, hBitmap); // Set parameters for layered window update

                    var newSize = new NativeMethods.SIZE(target.Size); // Size window to match bitmap
                    var sourceLocation = new NativeMethods.POINT(Point.Empty);
                    var newLocation = new NativeMethods.POINT(Location); // Same as this window

                    var blend = new NativeMethods.BLENDFUNCTION
                    {
                        BlendOp = NativeMethods.AC_SRC_OVER, // Only works with a 32bpp bitmap
                        BlendFlags = 0, // Always 0
                        SourceConstantAlpha = 255, // Set to 255 for per-pixel alpha values
                        AlphaFormat = NativeMethods.AC_SRC_ALPHA // Only works when the bitmap contains an alpha channel
                    };

                    // Update the window
                    NativeMethods.UpdateLayeredWindow(
                        Handle, screenDc, ref newLocation, ref newSize,
                        memDc, ref sourceLocation, 0, ref blend,
                        NativeMethods.ULW_ALPHA
                    );
                }
                finally
                {
                    // Release device context
                    NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);

                    if (hBitmap != IntPtr.Zero)
                    {
                        NativeMethods.SelectObject(memDc, hOldBitmap);
                        NativeMethods.DeleteObject(hBitmap); // Remove bitmap resources
                    }

                    NativeMethods.DeleteDC(memDc);
                }
            }
        }

        private void DrawImage(Graphics graphics, Bitmap image, Point location)
        {
            if (image == null)
                return;

            graphics.DrawImageUnscaled(image, location);
        }

        private void DrawBorder(Graphics graphics, Bitmap image, Rectangle rectangle)
        {
            if (image == null)
                return;

            using (var brush = new TextureBrush(image))
            {
                brush.TranslateTransform(rectangle.Left, rectangle.Top);
                graphics.FillRectangle(brush, rectangle);
            }
        }

        public void Synchronize(Rectangle formBounds)
        {
            Visible = _form.Visible && !IsMaximized;

            if (!Visible)
                return;

            var bounds = CalculateBounds(formBounds);

            NativeMethods.SetWindowPos(
                Handle,
                _form.Handle,
                bounds.Left,
                bounds.Top,
                bounds.Width,
                bounds.Height,
                NativeMethods.SWP_NOCOPYBITS | NativeMethods.SWP_NOACTIVATE
            );

            Invalidate();
        }

        private Rectangle CalculateBounds(Rectangle formBounds)
        {
            switch (Border)
            {
                case DropShadowBorder.Left:
                    return new Rectangle(
                        formBounds.Left - _borderWidth,
                        formBounds.Top,
                        _borderWidth,
                        formBounds.Height
                    );

                case DropShadowBorder.Top:
                    return new Rectangle(
                        formBounds.Left - _borderWidth,
                        formBounds.Top - _borderWidth,
                        formBounds.Width + _borderWidth * 2,
                        _borderWidth
                    );

                case DropShadowBorder.Right:
                    return new Rectangle(
                        formBounds.Right,
                        formBounds.Top,
                        _borderWidth,
                        formBounds.Height
                    );

                case DropShadowBorder.Bottom:
                    return new Rectangle(
                        formBounds.Left - _borderWidth,
                        formBounds.Bottom,
                        formBounds.Width + _borderWidth * 2,
                        _borderWidth
                    );

                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    WmNCHitTest(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        private void WmNCHitTest(ref Message m)
        {
            var screenPoint = new Point(m.LParam.ToInt32());

            // convert to local coordinates
            var location = PointToClient(screenPoint);

            switch (Border)
            {
                case DropShadowBorder.Left:
                    m.Result = (IntPtr)NonClientHitTest.Left;
                    break;

                case DropShadowBorder.Right:
                    m.Result = (IntPtr)NonClientHitTest.Right;
                    break;

                case DropShadowBorder.Top:
                    if (location.X < _borderWidth)
                        m.Result = (IntPtr)NonClientHitTest.TopLeft;
                    else if (location.X >= Width - _borderWidth)
                        m.Result = (IntPtr)NonClientHitTest.TopRight;
                    else
                        m.Result = (IntPtr)NonClientHitTest.Top;
                    break;

                case DropShadowBorder.Bottom:
                    if (location.X < _borderWidth)
                        m.Result = (IntPtr)NonClientHitTest.BottomLeft;
                    else if (location.X >= Width - _borderWidth)
                        m.Result = (IntPtr)NonClientHitTest.BottomRight;
                    else
                        m.Result = (IntPtr)NonClientHitTest.Bottom;
                    break;
            }
        }

        private class NonClientRedirector : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                var control = Control.FromHandle(m.HWnd) as DropShadow;

                if (control == null)
                    return false;

                switch (m.Msg)
                {
                    case NativeMethods.WM_NCLBUTTONDBLCLK:
                    case NativeMethods.WM_NCLBUTTONDOWN:
                    case NativeMethods.WM_NCMBUTTONDBLCLK:
                    case NativeMethods.WM_NCMBUTTONDOWN:
                    case NativeMethods.WM_NCRBUTTONDBLCLK:
                    case NativeMethods.WM_NCRBUTTONDOWN:
                        control._form.Focus();
                        break;
                }

                switch (m.Msg)
                {
                    case NativeMethods.WM_NCLBUTTONDBLCLK:
                    case NativeMethods.WM_NCLBUTTONDOWN:
                    case NativeMethods.WM_NCLBUTTONUP:
                    case NativeMethods.WM_NCMBUTTONDBLCLK:
                    case NativeMethods.WM_NCMBUTTONDOWN:
                    case NativeMethods.WM_NCMBUTTONUP:
                    case NativeMethods.WM_NCRBUTTONDBLCLK:
                    case NativeMethods.WM_NCRBUTTONDOWN:
                    case NativeMethods.WM_NCRBUTTONUP:
                    case NativeMethods.WM_NCMOUSEMOVE:
                        ForwardMessage(control._form, ref m);
                        return true;
                }

                return false;
            }

            private void ForwardMessage(IWin32Window control, ref Message m)
            {
                NativeMethods.SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
            }
        }
    }
}
