using System;
using Acb.Demo.Contracts.Enums;

namespace Acb.WebApi.Test.ViewModels
{
    public class VDemo
    {
        public DemoEnums Demo { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}
