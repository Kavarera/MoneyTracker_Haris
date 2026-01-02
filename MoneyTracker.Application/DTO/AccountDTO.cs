

namespace MoneyTracker.Application.DTO
{
    public sealed record AccountDTO
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public AccountDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
