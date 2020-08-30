using MRTKExtensions.ServiceExtensions;

namespace DemoApp.Services
{
    [InspectorExpand]
    public class ToStringOverriddenObject 
    {
        public string InterfaceProp1 { get; set; } = "interface value1";
        public string InterfaceProp2 { get; set; } = "interface value2";

        public override string ToString()
        {
            return $"{InterfaceProp1}_{InterfaceProp2}";
        }
    }
}
