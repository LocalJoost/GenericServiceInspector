using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;

namespace DemoApp.Services
{
	[MixedRealityServiceProfile(typeof(IDemoService))]
	[CreateAssetMenu(fileName = "DemoServiceProfile", menuName = "MixedRealityToolkit/DemoService Configuration Profile")]
	public class DemoServiceProfile : BaseMixedRealityProfile
	{
		// Store config data in serialized fields
	}
}