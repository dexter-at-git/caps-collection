using System;
using System.IO;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;
using FluentValidation;

namespace CapsCollection.Desktop.UI.Modules.Home.Validators
{
    public class HomeViewModelValidator : AbstractValidator<HomeViewModel>
    {
        public HomeViewModelValidator()
        {
            RuleFor(x => x.BottlesLookupPath).Must(CheckThatPathIsValid)
                .WithMessage("Path to bottles images is invalid");

            RuleFor(x => x.CapsLookupPath).Must(CheckThatPathIsValid)
                .WithMessage("Path to caps images is invalid");

            RuleFor(x => x.LabelsLookupPath).Must(CheckThatPathIsValid)
                .WithMessage("Path to labels images is invalid");
        }

        private bool CheckThatPathIsValid(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return true;
            }
            return Directory.Exists(path);
        }
    }
}