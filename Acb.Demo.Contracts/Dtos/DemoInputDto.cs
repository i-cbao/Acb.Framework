using System;
using Acb.Demo.Contracts.Enums;

namespace Acb.Demo.Contracts.Dtos
{
    public class DemoInputDto
    {
        public DemoEnums Demo { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}
