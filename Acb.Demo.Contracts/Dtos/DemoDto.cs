using Acb.Demo.Contracts.Enums;
using System;

namespace Acb.Demo.Contracts.Dtos
{
    public class DemoDto
    {
        public DemoEnums Demo { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}
