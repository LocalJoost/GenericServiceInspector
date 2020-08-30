using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp.Services
{
    public class ChildObject
    {
        public string Property1 { get; set; } = "demo value1";
        public string Property2 { get; set; } = "demo value2";

        public SecondLevelChildObject SecondLevel { get; } = new SecondLevelChildObject();
    }
}
