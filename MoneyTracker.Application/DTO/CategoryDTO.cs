
namespace MoneyTracker.Application.DTO
{
    public sealed class CategoryDTO
    {
        public int Id { get; init; }
        public string CategoryName { get; init; }
        public bool IsDisplay { get; set; }

        public CategoryDTO(int id, string categoryName)
        {
            Id = id;
            CategoryName = categoryName;
            IsDisplay = id < 2;
        }
    }
}
