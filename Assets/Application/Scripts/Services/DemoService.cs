using System;
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

		public override void Initialize()
		{
			base.Initialize();

			// Do service initialization here.
		}

		public override void Update()
		{
			base.Update();

			// Do service updates here.
		}
	}
}
