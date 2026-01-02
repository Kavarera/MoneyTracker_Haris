using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Application.DTO
{
    public sealed record CategoryDTO
    {
        public int Id { get; init; }
        public string CategoryName { get; init; }

        public CategoryDTO(int id, string categoryName)
        {
            Id = id;
            CategoryName = categoryName;
        }
    }
}
