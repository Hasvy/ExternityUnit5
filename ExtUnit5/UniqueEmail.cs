using Database;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExtUnit5
{
    public class UniqueEmail : ValidationAttribute
    {
        //private IDbContextFactory<AppDbContext> _dbContextFactory;

        //public UniqueEmail(IDbContextFactory<AppDbContext> dbContextFactory)
        //{
        //    _dbContextFactory = dbContextFactory;
        //}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = validationContext.GetRequiredService<AppDbContext>();
            if (value is string && !string.IsNullOrEmpty(value.ToString()))
            {
                bool isEmailUnique = context.Customers.FirstOrDefault(c => c.Email == (string)value) == null ? true : false;
                if (!isEmailUnique)
                {
                    return new ValidationResult("Email is used by other customer.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
