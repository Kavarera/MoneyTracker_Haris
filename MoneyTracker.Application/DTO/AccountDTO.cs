

namespace MoneyTracker.Application.DTO
{
    public sealed class AccountDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public bool IsDisplay { get; set; }
        public AccountDTO(int id, string name)
        {
            Id = id;
            Name = name;
            IsDisplay = id < 3;
        }
    }
}
