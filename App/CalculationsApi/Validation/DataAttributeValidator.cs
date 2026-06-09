using System.ComponentModel.DataAnnotations;

namespace CalculationsApi.Validation
{
    public class DataAttributeValidator
    {
        public Dictionary<string, string[]> Validate(object data)
        {
            var context = new ValidationContext(data);
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(
                data,
                context,
                validationResults,
                validateAllProperties: true);

            // convert the validation result into a dictionary grouped by the field name so its easy for the FE to render error messages
            var errors = validationResults
                .SelectMany(result =>
                    result.MemberNames.Select(member => new
                    {
                        Member = member,
                        Error = result.ErrorMessage ?? "Validation error"
                    }))
                .GroupBy(x => x.Member)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Error).ToArray());

            return errors;

        }
    }
}
