namespace Lab1.Pages
{
    public partial class MainPage : ContentPage
    {
        private string _currentInput = "0";          // Текущий ввод
        private string _operation = "";              // Последняя выбранная операция
        private double _firstOperand = 0;            // Накопленный левый операнд

        private bool _isErrorState = false;
        private readonly Color _normalTextColor = Colors.Black;   
        private readonly Color _errorTextColor = Colors.OrangeRed;

        public static string CalcHistory { get; private set; } = "";

        public MainPage()
        {
            InitializeComponent();

            CalcHistory = Preferences.Default.Get("CalcHistory", "");
        }


        public static void ClearHistory()
        {
            CalcHistory = "";
            Preferences.Default.Remove("CalcHistory");
        }


        private void ResetErrorIfNeeded()
        {
            if (_isErrorState)
            {
                OperationLabel.Text = "0";
                _currentInput = "";
                _operation = "";
                _firstOperand = 0;
                _isErrorState = false;
                ResultLabel.TextColor = _normalTextColor;
                
            }
        }

        private void ShowError(string message)
        {
            _currentInput = message;
            _isErrorState = true;
            ResultLabel.TextColor = _errorTextColor;
            OperationLabel.Text = "";
            _operation = "";
            _firstOperand = 0;
            UpdateResultLabel();
        }

        private string FormatResult(double value)
        {
            if (double.IsNaN(value)) return "Ошибка";
            if (double.IsPositiveInfinity(value)) return "∞";
            if (double.IsNegativeInfinity(value)) return "-∞";

            if (Math.Abs(value) > 1e12 || (Math.Abs(value) > 0 && Math.Abs(value) < 1e-6))
            {
                return value.ToString("0.###E+0");
            }

            string s = value.ToString("G14");

            if (s.Contains('.'))
            {
                s = s.TrimEnd('0').TrimEnd('.');
            }

            return s;
        }

        private void NumericButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            var button = sender as Button;
            var number = button?.Text;

            if (number is null) return;

            if (_currentInput == "0" && number != ".")
                _currentInput = number;
            else
                _currentInput += number;


            if (_operation != "")
            {
                OperationLabel.Text = $"{_firstOperand} {_operation} {_currentInput}";
            }

            UpdateResultLabel();
        }

        private void PointButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (!_currentInput.Contains("."))
            {
                _currentInput += ",";
            }

            UpdateResultLabel();
        }

        private void PlusButton_Clicked(object sender, EventArgs e) => HandleOperator("+");
        private void DifferenceButton_Clicked(object sender, EventArgs e) => HandleOperator("-");
        private void MultiplicationButton_Clicked(object sender, EventArgs e) => HandleOperator("×");
        private void DivideButton_Clicked(object sender, EventArgs e) => HandleOperator("÷");

        private void HandleOperator(string operation)
        {
            ResetErrorIfNeeded();

            if (!double.TryParse(_currentInput, out var number))
                return;

            if (_operation == "")
            {
                _firstOperand = number;
                _operation = operation;
                _currentInput = "0";
                OperationLabel.Text = $"{_firstOperand} {_operation}";
            }
            else
            {
                equal();
                if (_isErrorState) return;

                _firstOperand = double.Parse(_currentInput);
                _operation = operation;
                _currentInput = "0";
                OperationLabel.Text = $"{_firstOperand} {_operation}";
            }

            UpdateResultLabel();
        }

        private void EqualButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();
            equal();
        }

        private void equal()
        {
            if (!double.TryParse(_currentInput, out var secondOperand))
                return;

            double result = _firstOperand;

            switch (_operation)
            {
                case "+":
                    result += secondOperand;
                    break;

                case "-":
                    result -= secondOperand;
                    break;

                case "×":
                    result *= secondOperand;
                    break;

                case "÷":
                    if (Math.Abs(secondOperand) < 1e-10)
                    {
                        ShowError("Cannot divide by zero");
                        return;
                    }
                    result /= secondOperand;
                    break;

                default:
                    return;
            }

            if (double.IsNaN(result))
            {
                ShowError("Ошибка");
                return;
            }
            if (double.IsPositiveInfinity(result))
            {
                ShowError("∞");
                return;
            }
            if (double.IsNegativeInfinity(result))
            {
                ShowError("-∞");
                return;
            }
            string formattedResult = FormatResult(result);

        
            string entry = $"{_firstOperand} {_operation} {secondOperand} = {formattedResult}\n";

        
            CalcHistory = entry + CalcHistory;
            Preferences.Default.Set("CalcHistory", CalcHistory);

            _currentInput = formattedResult;
            ResultLabel.TextColor = _normalTextColor;
            _isErrorState = false;

            _operation = "";
            _firstOperand = 0;
            OperationLabel.Text = "";

            UpdateResultLabel();
        }


        private void SignButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (double.TryParse(_currentInput, out var number))
            {
                _currentInput = FormatResult(-number);
                UpdateResultLabel();
                UpdateOperationLabel();
            }
        }

        private void PercentButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (double.TryParse(_currentInput, out var number))
            {
                _currentInput = FormatResult(number / 100);
                UpdateResultLabel();
                UpdateOperationLabel();
            }
        }

        private void SqrtButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (double.TryParse(_currentInput, out var number))
            {
                if (number < 0)
                {
                    ShowError("Invalid input");
                    return;
                }
                _currentInput = FormatResult(Math.Sqrt(number));
                UpdateResultLabel();
                UpdateOperationLabel();
            }
        }

        private void SquereButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (double.TryParse(_currentInput, out var number))
            {
                _currentInput = FormatResult(Math.Pow(number, 2));
                UpdateResultLabel();
                UpdateOperationLabel();
            }
        }

        private void InverseButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (double.TryParse(_currentInput, out var number))
            {
                if (Math.Abs(number) < 1e-10)
                {
                    ShowError("Cannot divide by zero");
                    return;
                }
                _currentInput = FormatResult(1.0 / number);
                UpdateResultLabel();
                UpdateOperationLabel();
            }
        }

        private void ModulButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (double.TryParse(_currentInput, out var number))
            {
                _currentInput = FormatResult(Math.Abs(number));
                UpdateResultLabel();
                UpdateOperationLabel();
            }
        }


        private void ClearButton_Clicked(object sender, EventArgs e)
        {
            _currentInput = "0";
            _operation = "";
            _firstOperand = 0;
            _isErrorState = false;
            ResultLabel.TextColor = _normalTextColor;
            OperationLabel.Text = "";
            UpdateResultLabel();
        }

        private void BackspaceButton_Clicked(object sender, EventArgs e)
        {
            ResetErrorIfNeeded();

            if (_currentInput.Length > 1)
                _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
            else
                _currentInput = "0";

            UpdateResultLabel();
        }

        private void UpdateResultLabel()
        {
            ResultLabel.Text = _currentInput;
        }

        private void UpdateOperationLabel()
        {
            if (_operation != "" && !_isErrorState)
            {
                OperationLabel.Text = $"{_firstOperand} {_operation} {_currentInput}";
            }
        }
    }
}