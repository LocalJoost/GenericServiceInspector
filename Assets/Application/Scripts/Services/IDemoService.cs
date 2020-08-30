using Microsoft.MixedReality.Toolkit;
using UnityEngine;

namespace DemoApp.Services
{
	public interface IDemoService : IMixedRealityExtensionService
	{
        string SampleString { get; set; }
        Color SampleColor { get; set; }
        Vector3 SampleVector3 { get; set; }
        Vector2 SampleVector2 { get; set; }
        float SampleFloat { get; set; }

        ChildObject SimpleChildObject { get; }

        ToStringOverriddenObject ToStringObject { get; }
    }
}