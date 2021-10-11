using Windows.UI.Xaml.Controls;

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
