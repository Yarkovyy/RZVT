using Azure.Core;
using System.ComponentModel.DataAnnotations;

namespace GalleryMVC.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly string[] _contentTypes;

        public AllowedExtensionsAttribute(string[] extensions, string[] contentTypes)
        {
            _extensions = extensions;
            _contentTypes = contentTypes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {        
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Invalid file extension. Allowed: {string.Join(", ", _extensions)}");
                }

                if (!_contentTypes.Contains(file.ContentType.ToLowerInvariant()))
                {
                    return new ValidationResult($"Invalid content type. Allowed: {string.Join(", ", _contentTypes)}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
    

