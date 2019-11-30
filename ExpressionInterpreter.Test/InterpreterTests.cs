using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionInterpreter.Logic;

namespace ExpressionInterpreter.Test
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod()]
        public void Constructor_EmptyExpression_ShouldThrowParseException()
        {
            string expressionString = "";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Ausdruck ist null oder empty!", ex.Message);
            }
        }

        [TestMethod()]
        public void Constructor_SetExpressionStringOk_ShouldBeAsSet()
        {
            string expressionString = "5,7/3,5";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            Assert.AreEqual(expressionString, interpreter.ExpressionText);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod()]
        public void Constructor_SetExpressionStringMissingRightOperand_ShouldThrowArgumentException()
        {
            string expressionString = "5,7/";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
        }

        [TestMethod()]
        public void Constructor_SetExpressionStringMissingRightOperand_ShouldReturnCorrectErrorMessage()
        {
            string expressionString = "5,7/";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {

                Assert.AreEqual("Rechter Operand ist fehlerhaft", ex.Message);
            }
        }

        [TestMethod()]
        public void Constructor_SetExpressionStringMissingRightOperand_ShouldReturnInnerException()
        {
            string expressionString = "5,7/";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual(typeof(ArgumentException), ex.InnerException.GetType());
            }
        }

        [TestMethod()]
        public void Constructor_SetExpressionStringMissingRightOperand_ShouldReturnInnerExceptionMessage()
        {
            string expressionString = "5,7/";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual("Zahl fehlt komplett", ex.InnerException.Message);
            }
        }

        [TestMethod()]
        public void Constructor_SetExpressionStringMissingRightOperandDecimals_ShouldReturnInnerExceptionMessage()
        {
            string expressionString = "5,7/3,";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Rechter Operand ist fehlerhaft", ex.Message);
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual("Nachkommaanteil ist fehlerhaft", ex.InnerException.Message);
            }
        }

        [TestMethod()]
        public void Constructor_RightNumberOnlyDecimals_ShouldReturnInnerExceptionMessage()
        {
            string expressionString = "5,7/,3";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Rechter Operand ist fehlerhaft", ex.Message);
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual("Ganzzahlanteil ist fehlerhaft", ex.InnerException.Message);
            }
        }

        [TestMethod()]
        public void Constructor_ErrorInOperator_ShouldReturnInnerExceptionMessage()
        {
            string expressionString = "5,7 x 2,3";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Operator x ist fehlerhaft!", ex.Message);
            }
        }

        [TestMethod()]
        public void GetExceptionTextWithInnerExceptions_IllegalRightOperand_ShouldReturnMultipleExceptionMessages()
        {
            string expressionString = "5,7/3,";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                string exceptionString = Interpreter.GetExceptionTextWithInnerExceptions(ex);
                Assert.AreEqual("Exceptionmessage: Rechter Operand ist fehlerhaft\r\nInner Exception 1: Nachkommaanteil ist fehlerhaft\r\nInner Exception 2: Integeranteil fehlt oder beginnt nicht mit Ziffer\r\n",
                    exceptionString);
            }
        }

        [TestMethod()]
        public void GetExceptionTextWithInnerExceptions_IllegalLeftOperand_ShouldReturnMultipleExceptionMessages()
        {
            string expressionString = "5,/3,5";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                string exceptionString = Interpreter.GetExceptionTextWithInnerExceptions(ex);
                Assert.AreEqual("Exceptionmessage: Linker Operand ist fehlerhaft\r\nInner Exception 1: Nachkommaanteil ist fehlerhaft\r\nInner Exception 2: Integeranteil fehlt oder beginnt nicht mit Ziffer\r\n",
                    exceptionString);
            }
        }

        [TestMethod()]
        public void Calculate_DivideByZero_ShouldThrowExceptionMessages()
        {
            string expressionString = "5,7/0,0";
            try
            {
                Interpreter interpreter = new Interpreter();
                interpreter.Parse(expressionString);
                var result = interpreter.Calculate();
                Assert.Fail("Exception nicht aufgetreten!");
            }
            catch (Exception ex)
            {
                string exceptionString = Interpreter.GetExceptionTextWithInnerExceptions(ex);
                Assert.AreEqual("Exceptionmessage: Division durch 0 ist nicht erlaubt\r\n",
                    exceptionString);
            }
        }

        [TestMethod()]
        public void Calculate_Divide_ShouldSetResultToExpectedValue()
        {
            string expressionString = "5,7/2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            var result = interpreter.Calculate();
            Assert.AreEqual(5.7 / 2.0, result, 0.001);
        }

        [TestMethod()]
        public void Calculate_Multiply_ShouldSetResultToExpectedValue()
        {
            string expressionString = "5,7 * 2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            var result = interpreter.Calculate();
            Assert.AreEqual(5.7 * 2.0, result, 0.001);
        }

        [TestMethod()]
        public void Calculate_Add_ShouldSetResultToExpectedValue()
        {
            string expressionString = "5,7 + 2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            var result = interpreter.Calculate();
            Assert.AreEqual(5.7 + 2.0, result, 0.001);
        }

        [TestMethod()]
        public void Calculate_Subtract_ShouldSetResultToExpectedValue()
        {
            string expressionString = "5,7 - 2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            var result = interpreter.Calculate();
            Assert.AreEqual(5.7 - 2.0, result, 0.001);
        }

        [TestMethod()]
        public void Calculate_NegativeValue_ShouldSetResultToExpectedValue()
        {
            string expressionString = "-5,7 - 2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            var result = interpreter.Calculate();
            Assert.AreEqual(-5.7 - 2.0, result, 0.001);
        }

        [TestMethod()]
        public void Parse_LeftOperand_ShouldReturnCorrectValue()
        {
            string expressionString = "5,7/2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            Assert.AreEqual(5.7, interpreter.OperandLeft, 0.001);
        }

        [TestMethod()]
        public void Parse_RightOperand_ShouldReturnCorrectValue()
        {
            string expressionString = "5,7/2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            Assert.AreEqual(2.0, interpreter.OperandRight, 0.001);
        }

        [TestMethod()]
        public void Parse_Operator_ShouldReturnCorrectValue()
        {
            string expressionString = "5,7/2,0";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            Assert.AreEqual('/', interpreter.Op);
        }

        [TestMethod()]
        public void Parse_ExpressionString_ShouldReturnCorrectValue()
        {
            string expressionString = " 5,7 / 2,0 ";
            Interpreter interpreter = new Interpreter();
            interpreter.Parse(expressionString);
            Assert.AreEqual(" 5,7 / 2,0 ", interpreter.ExpressionText);
        }

    }
}
