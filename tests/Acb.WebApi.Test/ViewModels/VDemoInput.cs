﻿using Acb.Demo.Contracts.Enums;
using Acb.WebApi.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Acb.WebApi.Test.ViewModels
{
    public class VDemoInput : VPageAndTimeInput
    {
        /// <summary> Demo </summary>
        [Required(ErrorMessage = "must demo")]
        public DemoEnums Demo { get; set; }

        /// <summary> Name </summary>
        [Required(ErrorMessage = "名字不能为空")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "名字应为2-5个字符")]
        public string Name { get; set; }

        /// <summary> Time </summary>
        [Required(ErrorMessage = "时间不能为空")]
        //[EnumDataType(typeof(long),ErrorMessage = "时间格式不正确")]
        public DateTime? Time { get; set; }
    }

    public class VTestInput
    {
        public DemoEnums Demo { get; set; }
        public string Name { get; set; }
        public DateTime? Time { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
