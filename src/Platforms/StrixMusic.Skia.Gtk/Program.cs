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
			host.Run();
		}
	}
}
