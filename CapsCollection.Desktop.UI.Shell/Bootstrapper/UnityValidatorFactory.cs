using System;
using FluentValidation;
using Microsoft.Practices.Unity;

namespace CapsCollection.Desktop.UI.Shell.Bootstrapper
{
    public class UnityValidatorFactory : ValidatorFactoryBase
    {
        private readonly IUnityContainer _container;

        public UnityValidatorFactory(IUnityContainer container)
        {
            _container = container;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _container.Resolve(validatorType) as IValidator;
        }
    }
}