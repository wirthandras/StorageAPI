using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Configuration;
using StorageAPI.Model;

namespace StorageAPI.Requests.Validators
{
    public class StorageRequestValidator : AbstractValidator<StorageRequest>
    {
        private readonly AppDBContext _dbContext;
        public StorageRequestValidator(AppDBContext dbContext)
        {
            _dbContext = dbContext;

                RuleFor(request => request.Brand).MustAsync(async (brand, cancellation) =>
                {
                    return await _dbContext.Brand.AnyAsync(b => b.Name == brand);
                }).WithMessage(ErrorCode.BrandNotExist).WithErrorCode(ErrorCode.BrandNotExistCode);
        }
    }
}
