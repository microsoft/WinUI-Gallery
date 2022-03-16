using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics.SamplePages
{
    public sealed partial class SampleCompactSizingPage : Page
    {
        public TextBox FirstName => firstName;
        public TextBox LastName => lastName;
        public PasswordBox Password => password;
        public PasswordBox ConfirmPassword => confirmPassword;
        public DatePicker ChosenDate => chosenDate;

        public SampleCompactSizingPage()
        {
            this.InitializeComponent();
        }

        public void CopyState(SampleStandardSizingPage page)
        {
            FirstName.Text = page.FirstName.Text;
            LastName.Text = page.LastName.Text;
            Password.Password = page.Password.Password;
            ConfirmPassword.Password = page.ConfirmPassword.Password;
            ChosenDate.Date = page.ChosenDate.Date;
        }
    }
}
