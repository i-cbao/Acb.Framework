namespace Acb.Core.Domain.Dtos
{
    public class PageAndTimeDto : TimeInputDto, IPageInput
    {
        /// <inheritdoc />
        /// <summary> 当前页码 </summary>
        public int Page { get; set; }

        /// <inheritdoc />
        /// <summary> 每页数量 </summary>
        public int Size { get; set; }
    }
}
