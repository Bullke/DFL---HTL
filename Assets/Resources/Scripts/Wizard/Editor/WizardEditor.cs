using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace HTLWizards
{
	[CustomEditor(typeof(Wizard))]
	class WizardEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}
	}
}
