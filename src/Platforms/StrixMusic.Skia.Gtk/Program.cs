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

			host.Run();
		}
	}
}
