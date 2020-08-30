#if UNITY_EDITOR
using System;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit;
using MRTKExtensions.ServiceExtensions.Editor;
using UnityEngine;
using UnityEditor;

namespace DemoApp.Services.Editor
{	
	[MixedRealityServiceInspector(typeof(IDemoService))]
	public class DemoServiceInspector : BaseGenericServiceInspector
	{
		//public override void DrawInspectorGUI(object target)
		//{
		//	DemoService service = (DemoService)target;
			
		//	// Draw inspector here
		//}
	}
}

#endif