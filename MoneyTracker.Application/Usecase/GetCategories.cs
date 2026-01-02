using MoneyTracker.Application.DTO;
using MoneyTracker.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Application.Usecase
{
    public class GetCategories
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategories(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryDTO>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);

            // di sini tempat aturan aplikasi
            // contoh filtering / sorting dll
            return categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new CategoryDTO(c.Id, c.CategoryName))
                .ToList();
        }
    }
}
