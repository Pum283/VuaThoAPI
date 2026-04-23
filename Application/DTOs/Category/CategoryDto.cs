namespace Application.DTOs.Category
{
    /// <summary>
    /// Danh mục/chuyên môn hiển thị cho client.
    /// </summary>
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

