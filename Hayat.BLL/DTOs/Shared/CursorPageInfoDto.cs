namespace Hayat.BLL.DTOs.Shared
{
    public class CursorPageInfoDto
    {
        public int Limit { get; set; }
        public bool HasMore { get; set; }
        public string? NextCursor { get; set; }
    }
}
