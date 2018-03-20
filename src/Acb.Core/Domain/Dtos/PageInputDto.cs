namespace Acb.Core.Domain.Dtos
{
    public interface IPageInput
    {
        /// <summary> 当前页码 </summary>
        int Page { get; set; }

        /// <summary> 每页数量 </summary>
        int Size { get; set; }
    }

    /// <summary> 分页实体 </summary>
    public class PageInputDto : DDto, IPageInput
    {
        /// <summary> 当前页码 </summary>
        public int Page { get; set; }

        /// <summary> 每页数量 </summary>
        public int Size { get; set; }

        public PageInputDto()
        {
            Page = 1;
            Size = 20;
        }
        public PageInputDto(int page, int size)
        {
            Page = page;
            Size = size;
        }
    }
}
