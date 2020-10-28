using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;

namespace DemoApp.Services
{
	[MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.WindowsUniversal)]
	public class DemoService : BaseExtensionService, IDemoService, IMixedRealityExtensionService
	{
		private DemoServiceProfile demoServiceProfile;

		public DemoService(string name,  uint priority,  BaseMixedRealityProfile profile) : base(name, priority, profile) 
		{
			demoServiceProfile = (DemoServiceProfile)profile;
		}

		public string SampleString { get; set; } = "This is a string property";
        public Color SampleColor { get; set; } = Color.green;

        public Vector3 SampleVector3 { get; set; } = new Vector3(1,2,3);

        public Vector2 SampleVector2{ get; set; } = new Vector3(1,2);

        public float SampleFloat { get; set; } = Mathf.PI;

        public ChildObject SimpleChildObject { get; } = new ChildObject();

		public ToStringOverriddenObject ToStringObject { get; } = new ToStringOverriddenObject();

        public List<ChildObject> ListOfObjects { get; } = new List<ChildObject>
        {
            new ChildObject{Property1 = "Object1"},
            new ChildObject{Property1 = "Object2"},
            new ChildObject{Property1 = "Object3"},
            new ChildObject{Property1 = "Object4"},
            new ChildObject{Property1 = "Object5"},
        };

    }
}
