using System.IO;
using FluentValidation;

namespace CapsCollection.Desktop.UI.Modules.Settings.ViewModels
{
    public class ImageLookupFolders
    {
        public string BottlesLookupFolder { get; set; }
        public string CapsLookupFolder { get; set; }
        public string LabelsLookupFolder { get; set; }
    }

    public class ImageLookupFoldersValidator : AbstractValidator<ImageLookupFolders>
    {
        public ImageLookupFoldersValidator()
        {
            RuleFor(x => x.BottlesLookupFolder)
                .Must(CheckThatPathIsValid)
                .WithMessage("Path to bottles images is invalid");

            RuleFor(x => x.CapsLookupFolder)
                .Must(CheckThatPathIsValid)
                .WithMessage("Path to caps images is invalid");

            RuleFor(x => x.LabelsLookupFolder)
                .Must(CheckThatPathIsValid)
                .WithMessage("Path to labels images is invalid");
        }
        
        private bool CheckThatPathIsValid(string path)
        {
            return Directory.Exists(path);
        }
    }
}