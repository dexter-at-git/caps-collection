1. Properties of Prism libraries and CapsCollection.Silverlight.UI.Modules.Services "Copy Local" attribute should be set as "False". 
   Otherwise our application throw an assembly conflict exception.  
		Microsoft.Practices.Prism 
		Microsoft.Practices.Prism.Interactivity
		Microsoft.Practices.Prism.MefExtensions
		Microsoft.Practices.ServiceLocation
		CapsCollection.Silverlight.UI.Modules.Services

   
2. Do not reference code contracts libraries for ImageTools for Silverlight. Otherwise it will cause ambiguous references.
		ImageTools.Contracts.dll
		ImageTools.Controls.Contracts.dll
		ImageTools.Filtering.Contracts.dll
		ImageTools.Utils.Contracts.dll