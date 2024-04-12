// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#define INDEI

using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.ComponentModel;

namespace WinUIGallery.ControlPages
{
    public sealed partial class InputValidationPage : Page
    {
        public InputValidationPage()
        {
            ViewModel = new PurchaseViewModel();
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PurchaseViewModel), typeof(InputValidationPage), new PropertyMetadata(null));

        public PurchaseViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as PurchaseViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //CardHolderNameField.Focus(FocusState.Programmatic);
            //PlayEntranceTransition();
            /*
            Microsoft.UI.Xaml.Controls.InputValidationError err = new Microsoft.UI.Xaml.Controls.InputValidationError("testing");
            Windows.Foundation.Collections.IObservableVector<InputValidationError> validationErrors = CardHolderNameField.ValidationErrors;
            Windows.Foundation.Collections.IObservableVector<Microsoft.UI.Xaml.Controls.InputValidationError> errorsCasted = Shim.ReinterpretCast<Windows.Foundation.Collections.IObservableVector<Microsoft.UI.Xaml.Controls.InputValidationError>>(validationErrors);
            errorsCasted.Clear();
            var errCasted = Shim.ReinterpretCast<Microsoft.UI.Xaml.Controls.InputValidationError>(err);
            errorsCasted.Add(errCasted);
            */
        }

        /*
        private void PlayEntranceTransition()
        {
            var compositor = XamlRoot.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.2f, 1f));
            var duration = TimeSpan.FromMilliseconds(300);

            var backdropVisual = ElementCompositionPreview.GetElementVisual(Backdrop);
            var purchaseFormVisual = ElementCompositionPreview.GetElementVisual(PurchaseForm);

            // Initial conditions
            backdropVisual.Opacity = 0f;
            purchaseFormVisual.Opacity = 0f;

            // Set up animations
            var backdropVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            backdropVisualOpacityAnim.Target = "Opacity";
            backdropVisualOpacityAnim.Duration = duration;
            backdropVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var purchaseFormVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            purchaseFormVisualOpacityAnim.Target = "Opacity";
            purchaseFormVisualOpacityAnim.Duration = duration;
            purchaseFormVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            // Start animations
            backdropVisual.StartAnimation(backdropVisualOpacityAnim.Target, backdropVisualOpacityAnim);
            purchaseFormVisual.StartAnimation(purchaseFormVisualOpacityAnim.Target, purchaseFormVisualOpacityAnim);
        }

        public override Task PlayExitTransition()
        {
            var compositor = XamlRoot.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.7f, 0.0f), new Vector2(1.0f, 0.5f));
            var duration = TimeSpan.FromMilliseconds(500);

            var backdropVisual = ElementCompositionPreview.GetElementVisual(Backdrop);
            var purchaseFormVisual = ElementCompositionPreview.GetElementVisual(PurchaseForm);

            // Set up animations
            var backdropVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            backdropVisualOpacityAnim.Target = "Opacity";
            backdropVisualOpacityAnim.Duration = duration;
            backdropVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var purchaseFormVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            purchaseFormVisualOpacityAnim.Target = "Opacity";
            purchaseFormVisualOpacityAnim.Duration = duration;
            purchaseFormVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            // Start animations in scoped batch

            var scopedBatch = compositor.CreateScopedBatch(Windows.UI.Composition.CompositionBatchTypes.Animation);

            backdropVisual.StartAnimation(backdropVisualOpacityAnim.Target, backdropVisualOpacityAnim);
            purchaseFormVisual.StartAnimation(purchaseFormVisualOpacityAnim.Target, purchaseFormVisualOpacityAnim);

            scopedBatch.End();

            // Set up task completion

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            TypedEventHandler<object, CompositionBatchCompletedEventArgs> completedHandler = null;
            completedHandler = new TypedEventHandler<object, CompositionBatchCompletedEventArgs>((sender, args) =>
            {
                scopedBatch.Completed -= completedHandler;
                tcs.SetResult(null);
            });
            scopedBatch.Completed += completedHandler;

            return tcs.Task;
        }
        */
    }

    public partial class PurchaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public PurchaseViewModel()
        {
        }

        private string _name;

        [System.ComponentModel.DefaultValue("")]
        [MinLength(5, ErrorMessage = "Name must be more than 4 characters")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value);
            }
        }

        private string _cardNumber;
        [CreditCard]
        public string CardNumber
        {
            get
            {
                return _cardNumber;
            }
            set
            {
                SetValue(ref _cardNumber, value);
            }
        }

        private string _address;
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                SetValue(ref _address, value);
            }
        }

        private string _city;
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                SetValue(ref _city, value);
            }
        }

        private string _zip;
        [CustomValidation(typeof(PurchaseViewModel), "ValidateZip")]
        public string Zip
        {
            get
            {
                return _zip;
            }
            set
            {
                SetValue(ref _zip, value);
            }
        }

        private string _cardExpirationMonth;
        public string CardExpirationMonth
        {
            get
            {
                return _cardExpirationMonth;
            }
            set
            {
                SetValue(ref _cardExpirationMonth, value);
            }
        }

        private string _cardExpirationYear;
        [CustomValidation(typeof(PurchaseViewModel), "ValidateYear")]
        public string CardExpirationYear
        {
            get
            {
                return _cardExpirationYear;
            }
            set
            {
                SetValue(ref _cardExpirationYear, value);
            }
        }

        private string _ccv;
        public string CCV
        {
            get
            {
                return _ccv;
            }
            set
            {
                SetValue(ref _ccv, value);
            }
        }

        private string _billingAddress;
        public string BillingAddress
        {
            get
            {
                return _billingAddress;
            }
            set
            {
                SetValue(ref _billingAddress, value);
            }
        }

        private string _billingCity;
        public string BillingCity
        {
            get
            {
                return _billingCity;
            }
            set
            {
                SetValue(ref _billingCity, value);
            }
        }

        private string _billingZip;
        [CustomValidation(typeof(PurchaseViewModel), "ValidateZip")]
        public string BillingZip
        {
            get
            {
                return _billingZip;
            }
            set
            {
                SetValue(ref _billingZip, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INPC/INDEI
        private void SetValue<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                currentValue = newValue;
                NotifyPropertyChanged(propertyName);
                OnPropertyChanged(newValue, propertyName);
            }
        }

        Dictionary<string, List<System.ComponentModel.DataAnnotations.ValidationResult>> _errors = new Dictionary<string, List<System.ComponentModel.DataAnnotations.ValidationResult>>();
        public bool HasErrors
        {
            get
            {
                return _errors.Any();
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnPropertyChanged(object value, string propertyName)
        {
            ClearErrors(propertyName);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var result = Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                },
                results
                );

            if (!result)
            {
                AddErrors(propertyName, results);
            }
        }

        public static ValidationResult ValidateZip(string zip)
        {
            if (!zip.Any(x => char.IsLetter(x)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(
                    "Zip code must contain numbers only");
            }
        }

        public static ValidationResult ValidateYear(string year)
        {
            // This code is valid until the year 10000.
            if (year.Length != 4)
            {
                return new ValidationResult("Year must be in format XXXX");
            }

            if (DateTime.Now.Year <= int.Parse(year))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(
                    "Year must be on or after the current year.");
            }
        }

        private void AddErrors(string propertyName, IEnumerable<ValidationResult> results)
        {
            List<ValidationResult> errors = null;
            if (!_errors.TryGetValue(propertyName, out errors))
            {
                errors = new List<ValidationResult>();
                _errors.Add(propertyName, errors);
            }

            errors.AddRange(results);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.TryGetValue(propertyName, out var errors))
            {
                errors.Clear();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors[propertyName];
        }

        #endregion
    }
}
