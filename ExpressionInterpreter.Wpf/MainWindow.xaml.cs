using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using ExpressionInterpreter.Logic;

namespace ExpressionInterpreter.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Interpreter _interpreter;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _interpreter=new Interpreter();
        }

        private void ClearResults()
        {
            TextBoxException.Text = "";
            TextBoxResult.Text = "";
        }

        private void ButtonParse_Click(object sender, RoutedEventArgs e)
        {
            //! Ausgabe der Texte der Inner-Exceptions über eigene Methode
            try
            {
                TextBoxLeftOperand.Text = "";
                TextBoxRightOperand.Text = "";
                TextBoxOperator.Text = "";
                _interpreter.Parse(TextBoxExpression.Text);
                TextBoxLeftOperand.Text = _interpreter.OperandLeft.ToString();
                TextBoxRightOperand.Text = _interpreter.OperandRight.ToString();
                TextBoxOperator.Text = _interpreter.Op.ToString();
            }
            catch (Exception ex)
            {
                TextBoxException.Text = $"Fehler beim Parsen des Ausdrucks\n{Interpreter.GetExceptionTextWithInnerExceptions(ex)}";
            }
        }



        private void ButtonInterpret_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            try
            {
                double result = _interpreter.Calculate();
                TextBoxResult.Text = result.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                TextBoxException.Text = $"Fehler beim Berechnen\n{Interpreter.GetExceptionTextWithInnerExceptions(ex)}";
            }
        }

    }
}
