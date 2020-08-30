using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using MRTKExtensions.ServiceExtensions;

namespace DemoApp.Services
{
	[MixedRealityServiceProfile(typeof(IDemoService))]
	[CreateAssetMenu(fileName = "DemoServiceProfile", menuName = "MixedRealityToolkit/DemoService Configuration Profile")]
	public class DemoServiceProfile : BaseMixedRealityProfile
	{
        [SerializeField] private string someProperty = "test";
        public string SomeProperty => someProperty;

        [SerializeField] private bool someOtherProperty = true;
        public bool SomeOtherProperty => someOtherProperty;
	}
}