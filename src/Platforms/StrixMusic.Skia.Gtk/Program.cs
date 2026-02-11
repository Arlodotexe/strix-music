using System;
using GLib;
using LibVLCSharp.Shared;
using Uno.UI.Runtime.Skia.Gtk;

namespace StrixMusic.Skia.Gtk
{
	public sealed class Program
	{
		static void Main(string[] args)
		{
			// Initialize LibVLC early for media playback
			Core.Initialize();

			ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
			{
				Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
				expArgs.ExitApplication = true;
			};

			var host = new GtkHost(() => new App());

			// Wire up caption button actions — host.Window is accessed lazily when clicked (after window exists).
			AppFrame.MinimizeWindowAction = () => host.Window?.Iconify();
			AppFrame.MaximizeWindowAction = () =>
			{
				var w = host.Window;
				if (w != null)
				{
					if (w.IsMaximized)
						w.Unmaximize();
					else
						w.Maximize();
				}
			};

			// Uno's SetTitleBar doesn't implement window drag on the Gtk backend.
			// Use GTK's BeginMoveDrag to initiate a drag from the title bar area.
			AppFrame.BeginWindowDragAction = () =>
			{
				var w = host.Window;
				if (w != null)
				{
					var display = w.Display;
					var seat = display.DefaultSeat;
					var pointer = seat.Pointer;
					pointer.GetPosition(null, out int rootX, out int rootY);
					w.BeginMoveDrag(1, rootX, rootY, global::Gtk.Global.CurrentEventTime);
				}
			};

			// Undecorated windows lose WM resize borders. Implement edge resize via GTK.
			AppFrame.BeginWindowResizeAction = (edge) =>
			{
				var w = host.Window;
				if (w != null)
				{
					var display = w.Display;
					var seat = display.DefaultSeat;
					var pointer = seat.Pointer;
					pointer.GetPosition(null, out int rootX, out int rootY);
					w.BeginResizeDrag((Gdk.WindowEdge)edge, 1, rootX, rootY, global::Gtk.Global.CurrentEventTime);
				}
			};

			// Set cursor to indicate resize direction at window edges.
			AppFrame.SetEdgeCursorAction = (edge) =>
			{
				var w = host.Window;
				var gdkWindow = w?.Window;
				if (gdkWindow == null) return;

				if (edge < 0)
				{
					gdkWindow.Cursor = null;
					return;
				}

				var cursorType = edge switch
				{
					0 => Gdk.CursorType.TopLeftCorner,
					1 => Gdk.CursorType.TopSide,
					2 => Gdk.CursorType.TopRightCorner,
					3 => Gdk.CursorType.LeftSide,
					4 => Gdk.CursorType.RightSide,
					5 => Gdk.CursorType.BottomLeftCorner,
					6 => Gdk.CursorType.BottomSide,
					7 => Gdk.CursorType.BottomRightCorner,
					_ => Gdk.CursorType.Arrow
				};

				gdkWindow.Cursor = new Gdk.Cursor(cursorType);
			};

			host.Run();
		}
	}
}
