using System;
using System.Text;

namespace ExpressionInterpreter.Logic
{
	public class Interpreter
	{
		private double _operandLeft;
		private double _operandRight;
		private char _op;  // Operator                  

		/// <summary>
		/// Eingelesener Text
		/// </summary>
		public string ExpressionText { get; private set; }

		public double OperandLeft
		{
			get { return _operandLeft; }
		}

		public double OperandRight
		{
			get { return _operandRight; }

		}

		public char Op
		{
			get { return _op; }
		}


		public void Parse(string expressionText)
		{
			if(String.IsNullOrEmpty(expressionText))
			{
				throw new Exception("Ausdruck ist null oder empty!");
			}
			ExpressionText = expressionText;
			ParseExpressionStringToFields();
		}

		/// <summary>
		/// Wertet den Ausdruck aus und gibt das Ergebnis zurück.
		/// Fehlerhafte Operatoren und Division durch 0 werden über Exceptions zurückgemeldet
		/// </summary>
		public double Calculate()
		{
			double result = 0;

			switch (Op)
			{
				case '+':
					result = OperandLeft + OperandRight;
					break;
				case '-':
					result = OperandLeft - OperandRight;
					break;
				case '*':
					result = OperandLeft * OperandRight;
					break;
				case '/':
					if(OperandRight == 0)
					{
						throw new DivideByZeroException("Division durch 0 ist nicht erlaubt");
					}
					else
					{
						result = OperandLeft / OperandRight;
					}
					break;
			}
			return result;
		}

		/// <summary>
		/// Expressionstring in seine Bestandteile zerlegen und in die Felder speichern.
		/// 
		///     { }[-]{ }D{D}[,D{D}]{ }(+|-|*|/){ }[-]{ }D{D}[,D{D}]{ }
		///     
		/// Syntax  OP = +-*/
		///         Vorzeichen -
		///         Zahlen double/int
		///         Trennzeichen Leerzeichen zwischen OP, Vorzeichen und Zahlen
		/// </summary>
		public void ParseExpressionStringToFields()
		{
			int pos = 0;
			SkipBlanks(ref pos);
			try
			{
				_operandLeft = ScanNumber(ref pos);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Linker Operand ist fehlerhaft", ex);
			}
			SkipBlanks(ref pos);
			_op = ExpressionText[pos++];
			SkipBlanks(ref pos);
			try
			{
				_operandRight = ScanNumber(ref pos);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Rechter Operand ist fehlerhaft", ex);
			}
			SkipBlanks(ref pos);
			switch (_op)
			{
				case '+':
				case '-':
				case '*':
				case '/':
					break;
				default:
					throw new Exception($"Operator x ist fehlerhaft!");
			}
		}

		/// <summary>
		/// Ein Double muss mit einer Ziffer beginnen. Gibt es Nachkommastellen,
		/// müssen auch diese mit einer Ziffer beginnen.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		private double ScanNumber(ref int pos)
		{
			double result;
			double sign = 1;
			if(ExpressionText.Length <= pos)
			{
				throw new ArgumentException("Zahl fehlt komplett");
			}
			SkipBlanks(ref pos);
			if (ExpressionText[pos].Equals('-'))
			{
				sign = -sign;
				pos++;
				SkipBlanks(ref pos);
			}
			try
			{
				result = ScanInteger(ref pos);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Ganzzahlanteil ist fehlerhaft", ex);
			}
			if (ExpressionText[pos].Equals(','))
			{
				pos++;
				int startPos = pos;
				double decimalPlace;
				try
				{
					decimalPlace = ScanInteger(ref pos);
				}
				catch (Exception ex)
				{
					throw new ArgumentException("Nachkommaanteil ist fehlerhaft", ex);
				}
				result += decimalPlace / (Math.Pow(10, pos - startPos));
			}
			return sign * result;
		}

		/// <summary>
		/// Eine Ganzzahl muss mit einer Ziffer beginnen.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		private int ScanInteger(ref int pos)
		{
			int number = 0;
			if(pos >= ExpressionText.Length || ExpressionText[pos] < '0' &&
					ExpressionText[pos] < '9')
			{
				throw new ArgumentException("Integeranteil fehlt oder beginnt nicht mit Ziffer");
			}
			if(ExpressionText[pos] >= '0' && ExpressionText[pos] <= '9')
			{
				while (ExpressionText[pos] != ' ' && ExpressionText[pos] >= '0' &&
					ExpressionText[pos] <= '9' && ExpressionText.Length >= pos)
				{
					number = number * 10 + ExpressionText[pos] - '0';
					pos++;
					if (pos == ExpressionText.Length)
					{
						break;
					}
				}
			}
			return number;
		}

		/// <summary>
		/// Setzt die Position weiter, wenn Leerzeichen vorhanden sind
		/// </summary>
		/// <param name="pos"></param>
		private void SkipBlanks(ref int pos)
		{
			if (ExpressionText.Length > pos)
			{
				while (ExpressionText[pos] == ' ' || ExpressionText.Length < pos)
				{
					if(pos < ExpressionText.Length - 1)
					{
						pos++;
					}
					else
					{
						break;
					}
				}
			}
		}

		/// <summary>
		/// Exceptionmessage samt Innerexception-Texten ausgeben
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string GetExceptionTextWithInnerExceptions(Exception ex)
		{
			//"Exceptionmessage: Rechter Operand ist fehlerhaft\r\nInner Exception 1: Nachkommaanteil ist fehlerhaft\r\nInner Exception 2: Integeranteil fehlt oder beginnt nicht mit Ziffer\r\n"
			string exBegin = "Exceptionmessage: ";
			string innerException1 = "Inner Exception 1: ";
			string innerException2 = "Inner Exception 2: ";
			string exceptionString;
			if(ex.InnerException == null)
				exceptionString = $"{exBegin}{ex.Message}\r\n";
			else
				exceptionString = $"{exBegin}{ex.Message}\r\n{innerException1}{ex.InnerException.Message}\r\n" +
					$"{innerException2}{ex.InnerException.InnerException.Message}\r\n";
			return exceptionString;
		}
	}
}
