using System.IO;
using UnityEngine;

namespace LevelEditTool.Internals
{
	/// <summary>
	///   Environment 
	///   <para/>
	///   Holds environment variables for the level editing tool 
	/// </summary>
	public static class Env
	{
		#region Public Fields

		public const string OutputPath = "Assets/Generated/HTLLevelEdit/Templates/";
		public const string ToolDirectory = "Assets/DFLTools/HTLLevelEditPrefabs";
		public const string WorkspaceName = "HTLLevelWorkspace";

		#endregion

		#region Public Properties

		public static string WorkspacePath
		{ get { return ToolDirectory + '/' + WorkspaceName + ".unity"; } }

		#endregion

		#region Public Methods

		public static void EnsureDirectory(string location)
		{
			Directory.CreateDirectory(location);
		}

		public static bool TargetExists(string filename)
		{
			return File.Exists(OutputPath + "/" + filename);
		}

		#endregion
	}
}
