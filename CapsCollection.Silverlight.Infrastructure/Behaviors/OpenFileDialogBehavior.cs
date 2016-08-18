using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CapsCollection.Silverlight.Infrastructure.Behaviors
{
    [Description("Open File Dialog Box")]
    public class OpenFileDialogBehavior : TargetedTriggerAction<Button>
    {
        #region Dependency Properties

        public static readonly DependencyProperty FileDialogDialogResultCommandProperty =
            DependencyProperty.Register("FileDialogDialogResultCommandProperty",
            typeof(object), typeof(OpenFileDialogBehavior), null);

        public static readonly DependencyProperty FileDialogDialogResultFileNameCommandProperty =
            DependencyProperty.Register("FileDialogDialogResultFileNameCommandProperty",
            typeof(object), typeof(OpenFileDialogBehavior), null);

        /// <summary>
        /// Gets or sets the file dialog dialog result command.
        /// </summary>
        /// <value>
        /// The file dialog dialog result command.
        /// </value>
        public object FileDialogDialogResultCommand
        {
            get
            {
                return (object)base.GetValue(FileDialogDialogResultCommandProperty);
            }
            set
            {
                base.SetValue(FileDialogDialogResultCommandProperty, value);
            }
        }


        public object FileDialogDialogResultFileNameCommand
        {
            get
            {
                return (object)base.GetValue(FileDialogDialogResultFileNameCommandProperty);
            }
            set
            {
                base.SetValue(FileDialogDialogResultFileNameCommandProperty, value);
            }
        }


        public static readonly DependencyProperty DialogFilterProperty =
           DependencyProperty.Register("DialogFilter", typeof(string),
           typeof(OpenFileDialogBehavior), null);

        /// <summary>
        /// Gets or sets the dialog filter.
        /// </summary>
        /// <value>
        /// The dialog filter.
        /// </value>
        public string DialogFilter
        {
            get
            {
                if (GetValue(DialogFilterProperty) == null)
                {
                    return "PNG Files (*.png)|*.png|JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|All Files (*.*)|*.*";
                }
                return (string)base.GetValue(DialogFilterProperty);
            }
            set
            {
                base.SetValue(DialogFilterProperty, value);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            try
            {
                //Create a file dialog
                var selectPictureDialog = new OpenFileDialog();

                //set properties for this dialog ( only once file, title and check that file exist )
                selectPictureDialog.Filter = DialogFilter;
                selectPictureDialog.Multiselect = false;

                if (selectPictureDialog.ShowDialog() == true)
                {
                    byte[] buffer;
                    using (var stream = selectPictureDialog.File.OpenRead())
                    {
                        buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                    }

                    //assign selected picture
                    FileDialogDialogResultCommand = buffer;

                    FileDialogDialogResultFileNameCommand = selectPictureDialog.File.Name;
                }
            }
            catch (SecurityException excep)
            {
                Debug.WriteLine("OpenFileDialogBehavior: Security Error:" + excep.ToString());
            }

        }

        #endregion
    }
}
