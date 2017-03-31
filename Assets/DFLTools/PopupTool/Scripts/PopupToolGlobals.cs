using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PopupTool.Internals
{
	/// <summary>
	///     Environment 
	///     <para/>
	///     Holds environment variables for the popup tool 
	/// </summary>
	public static class Env
	{
		#region Private Fields

		static readonly string _outputDirectory = "Assets/Generated/PopupTool/Templates/";
		static readonly string _toolDirectory = "Assets/DFLTools/PopupTool/";
		static readonly string _workspaceSceneName = "TemplateWorkspace";

		#endregion

		#region Internal Properties

		public static string OutputPath
		{ get { return _outputDirectory; } }

		public static string WorkspaceName
		{ get { return _workspaceSceneName; } }

		public static string ToolDirectory
		{ get { return _toolDirectory;} }

		public static string WorkspacePath
		{ get { return ToolDirectory + _workspaceSceneName + ".unity"; } }

		public static bool TargetExists(string filename)
		{
			return File.Exists(_outputDirectory + "/" + filename);
		}

		#endregion

		public static void EnsureDirectory(string location)
		{
			Directory.CreateDirectory(location);
		}

	}

	/// <summary>
	///     Resources 
	///     <para/>
	///     Holds strings for the popup tool 
	/// </summary>
	public static class Res
	{
		#region Internal Fields

		public static readonly string
			WorkingMessage = "Use this window to customize the template's variables.",
			InformationMessage =
				"Get to making some popups, yo.",
			IncorrectSceneMessage =
				"In order to create and edit templates,"
				+ " the Procedural Popup Tool needs to use a unique workspace scene.",
			Lorem = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras iaculis, lectus in viverra maximus, ipsum lorem tempor erat, ut consectetur mauris purus a nulla. Fusce dignissim leo nibh. Morbi nec sagittis enim. Nam iaculis leo pharetra tellus fermentum, vitae viverra lacus tempor. Cras porttitor libero magna, ut scelerisque nunc convallis eu. Nunc ac urna fermentum, commodo massa quis, molestie risus. Ut in orci non nibh posuere commodo et eget risus. Donec at rhoncus dui. Etiam elementum consectetur neque vel suscipit. Ut ultricies, leo eget efficitur pretium, arcu orci consectetur lorem, in porta nunc ante et orci. Aenean vel dapibus neque, quis molestie lorem.Proin tincidunt lectus at tortor commodo, in mattis erat feugiat. In eu ante gravida, malesuada neque imperdiet, auctor mi.

Sed rhoncus dignissim eros, at bibendum odio dapibus vitae. Donec sit amet ante sed nibh placerat pharetra. Nullam at sollicitudin ex. Aenean ut efficitur lorem. Suspendisse in scelerisque neque. Nulla at diam quam. Donec interdum magna rutrum odio venenatis, non porttitor diam molestie. Lorem ipsum dolor sit amet, consectetur adipiscing elit.Maecenas tempor faucibus dui eget fermentum. Integer at viverra nunc. Quisque hendrerit commodo est, non suscipit nulla consectetur ut.Vivamus lectus tellus, aliquam et libero sit amet, mattis ornare neque.

Duis imperdiet lorem eu dolor gravida euismod. Etiam accumsan nec massa id mattis. Etiam egestas arcu eu magna auctor, at venenatis dolor tristique. Duis risus ipsum, rhoncus sed hendrerit mattis, feugiat eget lectus.Vivamus non est nec ante aliquam lacinia vel sit amet augue. Cras nulla ipsum, faucibus ac nulla et, ullamcorper posuere diam.Duis porttitor lobortis ornare. Vivamus dapibus nec tortor ac euismod.

Nulla ac diam scelerisque nisi porta pulvinar. In hac habitasse platea dictumst.In malesuada quam elementum, venenatis quam sit amet, pulvinar ligula. Proin consequat malesuada velit, nec mattis erat varius vitae.Aenean non eros non orci rutrum bibendum.Sed accumsan, libero ut consectetur sodales, odio diam congue libero, quis hendrerit leo purus ac sem. Vestibulum nec posuere magna. Praesent massa ante, euismod eu orci ac, tempor finibus nunc.Donec pretium vel nisl ultricies tincidunt. Aenean sed libero mattis, mollis ante sed, tempus augue.

Sed sollicitudin felis id turpis porttitor, eu auctor lacus ornare. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Ut mattis, quam a dignissim venenatis, felis ligula efficitur leo, nec molestie nisi tellus vel lorem. In a egestas lacus. Aenean eget nulla ut metus sodales molestie nec sed ante. Vestibulum ut rhoncus purus. Duis mi sapien, posuere in consequat quis, porta non turpis. Aenean maximus sed ex ac porta. Cras laoreet cursus bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit.Duis rhoncus diam neque, sed commodo metus sagittis in. Phasellus pulvinar dui sollicitudin, semper erat et, luctus libero. ";


		#endregion
	}
}
