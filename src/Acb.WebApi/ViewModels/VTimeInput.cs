using Acb.Core.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acb.WebApi.ViewModels
{
    public class VTimeInput : ITimeInput, IValidatableObject
    {
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var list = new List<ValidationResult>();
            if (Begin.HasValue && End.HasValue && Begin.Value >= End.Value)
                list.Add(new ValidationResult("开始时间需要大于结束时间"));
            return list;
        }
    }
}
