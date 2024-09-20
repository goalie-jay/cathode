using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace cathode_rt
{
    public enum TokenType
    {
        IDENTIFIER,
        INTEGER_CONSTANT,
        FLOAT_CONSTANT,
        STRING_CONSTANT,
        PLUS,
        MINUS,
        ASTERISK,
        PERCENTAGE,
        BACKSLASH,
        EXCLAMATION,
        LEFTPARENTHESIS,
        RIGHTPARENTHESIS,
        LEFTCURLYBRACKET,
        RIGHTCURLYBRACKET,
        LEFTBRACKET,
        RIGHTBRACKET,
        LESSTHAN,
        GREATERTHAN,
        COMMA,
        PERIOD,
        EQUALS,
        VARIABLE_DEFINITION,
        INCREMENT,
        EXCLAMATIONEQUALS,
        EQUALSEQUALS,
        DOUBLEAMPERSAND,
        DOUBLEPIPE,
        IF,
        ELSE,
        THEN,
        POST,
        RET,
        VOID,
        WHILE,
        LOOP,
        ESC,
        MKREF,
        DEREF,

        EOL
    }

    public class InterpreterRuntimeException : Exception
    {
        public InterpreterRuntimeException(string details) : base(details) { }
    }

    public class Token
    {
        public ZZObject Value { get; private set; }
        public TokenType TokenType { get; private set; }

        public Token(TokenType type, ZZObject value)
        {
            Value = value;
            TokenType = type;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(TokenType), TokenType) + ": " + Value.ToString();
        }
    }

    public class Interpreter
    {
        public string Text { get; private set; }
        int Position;
        Token CurrentToken;
        ExecutionContext Context;
        int EvaluationStackDepth = 0;

        private int Atoi(char val)
        {
            return (int)val - 48;
        }

        public Interpreter(ExecutionContext context, string text)
        {
            Context = context;
            Text = text;
            Position = 0;
            CurrentToken = null;
        }

        void Consume(TokenType expectedType)
        {
            if (CurrentToken.TokenType == expectedType)
                CurrentToken = GetNextToken();
            else
                throw new InterpreterRuntimeException($"Unexpected token of type " +
                    $"{Enum.GetName(CurrentToken.TokenType)} where type {Enum.GetName(expectedType)} was " +
                    $"expected.");
        }

        void ConsumeAny(params TokenType[] expectedTypes)
        {
            if (expectedTypes.Length < 1)
            {
                CurrentToken = GetNextToken();
                return;
            }

            if (expectedTypes.Contains(CurrentToken.TokenType))
                CurrentToken = GetNextToken();
            else
            {
                List<string> prettyTokenTypes = new List<string>();

                foreach (TokenType tt in expectedTypes)
                    prettyTokenTypes.Add(Enum.GetName(tt));

                throw new InterpreterRuntimeException($"Unexpected token of type " +
                    $"{Enum.GetName(CurrentToken.TokenType)} where types {"{"} {string.Join(", ", prettyTokenTypes)} {"}"} " +
                    $"was expected.");
            }
        }

        bool MatchDoNotConsume(params TokenType[] expectedTypes)
        {
            return expectedTypes.Contains(CurrentToken.TokenType);
        }

        bool PeekMatchDoNotConsume(params TokenType[] expectedTypes)
        {
            return expectedTypes.Contains(Peek().TokenType);
        }

        bool DoNotMatchDoNotConsume(params TokenType[] expectedTypes)
        {
            return !MatchDoNotConsume(expectedTypes);
        }

        char CurrentChar => Text[Position];
        Token GetNextToken()
        {
            if (Position >= Text.Length)
                return new Token(TokenType.EOL, null);

            while (char.IsWhiteSpace(CurrentChar))
            {
                ++Position;

                if (Position >= Text.Length)
                    return new Token(TokenType.EOL, null);
            }

            if (CurrentChar == ';')
                return new Token(TokenType.EOL, null);

            // then
            if (CurrentChar == 't')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'h')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'e')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'n')
                        {
                            ++Position;
                            if (Position >= Text.Length || char.IsWhiteSpace(CurrentChar))
                            {
                                ++Position;
                                return new Token(TokenType.THEN, null);
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // if 
            if (CurrentChar == 'i')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'f')
                {
                    ++Position;
                    if (Position < Text.Length && char.IsWhiteSpace(CurrentChar))
                    {
                        ++Position;
                        return new Token(TokenType.IF, null);
                    }
                }

                Position = posBackup;
            }

            // else
            if (CurrentChar == 'e')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'l')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 's')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'e')
                        {
                            ++Position;
                            if (Position >= Text.Length || char.IsWhiteSpace(CurrentChar))
                            {
                                ++Position;
                                return new Token(TokenType.ELSE, null);
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // dim
            if (CurrentChar == 'd')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'i')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'm')
                    {
                        ++Position;
                        if (Position < Text.Length && char.IsWhiteSpace(CurrentChar))
                        {
                            ++Position;
                            return new Token(TokenType.VARIABLE_DEFINITION, null);
                        }
                    }
                }

                Position = posBackup;
            }

            // inc
            if (CurrentChar == 'i')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'n')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'c')
                    {
                        ++Position;
                        if (Position < Text.Length && !char.IsLetterOrDigit(CurrentChar))
                            return new Token(TokenType.INCREMENT, null);
                    }
                }

                Position = posBackup;
            }

            // ret
            if (CurrentChar == 'r')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'e')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 't')
                    {
                        ++Position;
                        if (Position < Text.Length && char.IsWhiteSpace(CurrentChar))
                        {
                            ++Position;
                            return new Token(TokenType.RET, null);
                        }
                    }
                }

                Position = posBackup;
            }

            // post
            if (CurrentChar == 'p')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'o')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 's')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 't')
                        {
                            ++Position;
                            if (Position < Text.Length && char.IsWhiteSpace(CurrentChar))
                            {
                                ++Position;
                                return new Token(TokenType.POST, null);
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // void
            if (CurrentChar == 'v')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'o')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'i')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'd')
                        {
                            ++Position;
                            if (Position >= Text.Length || !char.IsLetterOrDigit(CurrentChar))
                                return new Token(TokenType.VOID, ZZVoid.Void);
                        }
                    }
                }

                Position = posBackup;
            }

            // while
            if (CurrentChar == 'w')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'h')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'i')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'l')
                        {
                            ++Position;
                            if (Position < Text.Length && CurrentChar == 'e')
                            {
                                ++Position;
                                if (Position >= Text.Length || char.IsWhiteSpace(CurrentChar))
                                {
                                    ++Position;
                                    return new Token(TokenType.WHILE, null);
                                }
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // mkref
            if (CurrentChar == 'm')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'k')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'r')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'e')
                        {
                            ++Position;
                            if (Position < Text.Length && CurrentChar == 'f')
                            {
                                ++Position;
                                if (Position >= Text.Length || !char.IsLetterOrDigit(CurrentChar))
                                    return new Token(TokenType.MKREF, null);
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // deref
            if (CurrentChar == 'd')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'e')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'r')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'e')
                        {
                            ++Position;
                            if (Position < Text.Length && CurrentChar == 'f')
                            {
                                ++Position;
                                if (Position >= Text.Length || !char.IsLetterOrDigit(CurrentChar))
                                    return new Token(TokenType.DEREF, null);
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // esc
            if (CurrentChar == 'e')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 's')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'c')
                    {
                        ++Position;
                        if (Position >= Text.Length || char.IsWhiteSpace(CurrentChar))
                        {
                            ++Position;
                            return new Token(TokenType.ESC, null);
                        }
                    }
                }

                Position = posBackup;
            }

            // loop
            if (CurrentChar == 'l')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'o')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'o')
                    {
                        ++Position;
                        if (Position < Text.Length && CurrentChar == 'p')
                        {
                            ++Position;
                            if (Position >= Text.Length || char.IsWhiteSpace(CurrentChar))
                            {
                                ++Position;
                                return new Token(TokenType.LOOP, null);
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            // ==
            if (CurrentChar == '=')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == '=')
                {
                    ++Position;
                    return new Token(TokenType.EQUALSEQUALS, null);
                }

                Position = posBackup;
            }

            // !=
            if (CurrentChar == '!')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == '=')
                {
                    ++Position;
                    return new Token(TokenType.EXCLAMATIONEQUALS, null);
                }

                Position = posBackup;
            }

            // ||
            if (CurrentChar == '|')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == '|')
                {
                    ++Position;
                    return new Token(TokenType.DOUBLEPIPE, null);
                }

                Position = posBackup;
            }

            // &&
            if (CurrentChar == '&')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == '&')
                {
                    ++Position;
                    return new Token(TokenType.DOUBLEAMPERSAND, null);
                }

                Position = posBackup;
            }

            if (char.IsDigit(CurrentChar))
            {
                int constant = 0;

                while (char.IsDigit(CurrentChar))
                {
                    constant = (constant * 10) + Atoi(CurrentChar);
                    ++Position;

                    if (Position >= Text.Length)
                        break;

                    // Float support
                    if (CurrentChar == '.')
                    {
                        double floatConstant = (double)constant;
                        int depth = 1;

                        ++Position;
                        if (Position >= Text.Length)
                            throw new InterpreterRuntimeException("Decimal point at the end of the line is " +
                                "not supported for float constants.");

                        while (char.IsDigit(CurrentChar))
                        {
                            double add = (Atoi(CurrentChar) /
                                (double)Math.Pow(10, depth));
                            floatConstant = floatConstant + add;
                            ++depth;

                            ++Position;
                            if (Position >= Text.Length)
                                break;
                        }

                        return new Token(TokenType.FLOAT_CONSTANT, (ZZFloat)floatConstant);
                    }
                }

                return new Token(TokenType.INTEGER_CONSTANT, (ZZInteger)constant);
            }
            else if (char.IsLetter(CurrentChar))
            {
                StringBuilder identifier = new StringBuilder();

                while (char.IsLetterOrDigit(CurrentChar))
                {
                    identifier.Append(CurrentChar);
                    ++Position;

                    if (Position >= Text.Length)
                        break;
                }

                return new Token(TokenType.IDENTIFIER, (ZZString)identifier.ToString());
            }
            else if (CurrentChar == '"')
            {
                ++Position;

                if (Position >= Text.Length)
                    throw new InterpreterRuntimeException("Line terminated before string literal ended.");

                StringBuilder stringLiteral = new StringBuilder();

                while (CurrentChar != '"')
                {
                    stringLiteral.Append(CurrentChar);
                    ++Position;

                    if (Position >= Text.Length)
                        break;

                    if (CurrentChar == '\\')
                    {
                        // Escape sequence

                        ++Position;

                        if (Position >= Text.Length)
                            throw new InterpreterRuntimeException("Line terminated before string literal ended.");

                        stringLiteral.Append(CurrentChar);
                        ++Position;

                        if (Position >= Text.Length)
                            throw new InterpreterRuntimeException("Line terminated before string literal ended.");

                        // I know this looks like crap but idk how else to do it rn
                    }
                }

                ++Position;
                return new Token(TokenType.STRING_CONSTANT, (ZZString)stringLiteral.ToString());
            }
            else
                switch (CurrentChar)
                {
                    case '+':
                        ++Position;
                        return new Token(TokenType.PLUS, null);
                    case '-':
                        ++Position;
                        return new Token(TokenType.MINUS, null);
                    case '*':
                        ++Position;
                        return new Token(TokenType.ASTERISK, null);
                    case '%':
                        ++Position;
                        return new Token(TokenType.PERCENTAGE, null);
                    case '/':
                        ++Position;
                        return new Token(TokenType.BACKSLASH, null);
                    case '(':
                        ++Position;
                        return new Token(TokenType.LEFTPARENTHESIS, null);
                    case ')':
                        ++Position;
                        return new Token(TokenType.RIGHTPARENTHESIS, null);
                    case '{':
                        ++Position;
                        return new Token(TokenType.LEFTCURLYBRACKET, null);
                    case '}':
                        ++Position;
                        return new Token(TokenType.RIGHTCURLYBRACKET, null);
                    case '[':
                        ++Position;
                        return new Token(TokenType.LEFTBRACKET, null);
                    case ']':
                        ++Position;
                        return new Token(TokenType.RIGHTBRACKET, null);
                    case ',':
                        ++Position;
                        return new Token(TokenType.COMMA, null);
                    case '.':
                        ++Position;
                        return new Token(TokenType.PERIOD, null);
                    case '=':
                        ++Position;
                        return new Token(TokenType.EQUALS, null);
                    case '!':
                        ++Position;
                        return new Token(TokenType.EXCLAMATION, null);
                    case '<':
                        ++Position;
                        return new Token(TokenType.LESSTHAN, null);
                    case '>':
                        ++Position;
                        return new Token(TokenType.GREATERTHAN, null);
                }

            throw new Exception();
        }

        Token Peek(int i = 1)
        {
            int pos = Position;
            Token ret = null;

            for (int j = 0; j < i; ++j)
                ret = GetNextToken();

            Position = pos;

            return ret;
        }

        private ZZObject EvaluateFunctionCallExpr(Token identifier)
        {
            Consume(TokenType.LEFTPARENTHESIS);

            List<ZZObject> parameters = new List<ZZObject>();

            while (!MatchDoNotConsume(TokenType.RIGHTPARENTHESIS))
            {
                parameters.Add(Evaluate());

                if (!MatchDoNotConsume(TokenType.COMMA))
                    break; // Leave if no comma
                else
                    Consume(TokenType.COMMA);
            }

            Consume(TokenType.RIGHTPARENTHESIS);

            var methodInf = Context.LookupBuiltInFunction(((ZZString)identifier.Value).ToString());

            if (methodInf == null)
            {
                // Check for user-defined function
                string[] userFn = Context.GetFunctionOrReturnNullIfNotPresent(
                    ((ZZString)identifier.Value).ToString(), 
                    out ZZFunctionDescriptor descriptor);

                if (userFn == null)
                    throw new InterpreterRuntimeException("Function could not be resolved. Is there a typo, or " +
                        "have you checked to make sure the appropriate namespace has been imported?");

                if (descriptor.Arguments.Length != parameters.Count)
                    throw new InterpreterRuntimeException("Tried to supply an incorrect amount of parameters in " +
                        "a function call.");

                // Backup execution context
                ExecutionContext context = Program.CurrentlyExecutingContext;
                using (ExecutionContext callContext = new ExecutionContext())
                {
                    for (int i = 0; i < parameters.Count; ++i)
                        callContext.Variables.Add(descriptor.Arguments[i],
                            parameters[i]); // Line up our values with the parameter names

                    ZZObject obj = EvaluateExpr(Executor.Execute(callContext, descriptor.Name, userFn));

                    // Restore execution context
                    Program.CurrentlyExecutingContext = context;
                    return obj;
                }
            }

            try
            {
                ZZObject retVal = EvaluateExpr((ZZObject)methodInf.Invoke(null, parameters.ToArray()));

                return retVal;
            }
            catch (InterpreterRuntimeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is System.Reflection.TargetParameterCountException)
                    throw new InterpreterRuntimeException("Tried to supply either an incorrect amount of parameters " +
                        "or parameters with incorrect types to a standard function. " +
                        "Are you calling the correct function? Did you forget " +
                        "a type conversion?");
                else
                    throw;
            }
        }

        private ZZFloat EvaluateFloatBinaryExpr(ZZFloat lhsVal)
        {
            Token operation = CurrentToken;
            ConsumeAny(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH);

            ZZObject testRhs = Evaluate();

            if (testRhs.ObjectType != ZZObjectType.FLOAT)
                throw new InterpreterRuntimeException("The right-hand side of a floating point binary" +
                    " expression was not a float.");

            ZZFloat rhsValue = (ZZFloat)testRhs;

            switch (operation.TokenType)
            {
                case TokenType.PLUS:
                    return lhsVal + rhsValue;
                case TokenType.MINUS:
                    return lhsVal - rhsValue;
                case TokenType.BACKSLASH:
                    return lhsVal / rhsValue;
                case TokenType.ASTERISK:
                    return lhsVal * rhsValue;
            }

            return 0f;
        }

        private ZZInteger EvaluateIntegerBinaryExpr(ZZInteger lhsVal)
        {
            Token operation = CurrentToken;
            ConsumeAny(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH, TokenType.PERCENTAGE);

            ZZObject testRhs = Evaluate();

            if (testRhs.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("The right-hand side of an integer binary" +
                    " expression was not an integer.");

            ZZInteger rhsValue = (ZZInteger)testRhs;

            switch (operation.TokenType)
            {
                case TokenType.PLUS:
                    return lhsVal + rhsValue;
                case TokenType.MINUS:
                    return lhsVal - rhsValue;
                case TokenType.BACKSLASH:
                    return lhsVal / rhsValue;
                case TokenType.ASTERISK:
                    return lhsVal * rhsValue;
                case TokenType.PERCENTAGE:
                    return lhsVal % rhsValue;
            }

            return 0;
        }

        ZZObject EvaluateVariableDimensionExpr()
        {
            Consume(TokenType.VARIABLE_DEFINITION);

            Token id = CurrentToken;
            Consume(TokenType.IDENTIFIER);

            if (Context.Variables.ContainsKey(((ZZString)id.Value).ToString()))
                throw new InterpreterRuntimeException("Tried to dimension variable that already existed.");

            Context.Variables.Add(((ZZString)id.Value).ToString(), ZZVoid.Void);

            if (CurrentToken.TokenType == TokenType.EQUALS)
            {
                Consume(TokenType.EQUALS);

                ZZObject value = Evaluate(); // Dimension and assignment at once
                Context.Variables[((ZZString)id.Value).ToString()] = value;
            }

            return Context.Variables[((ZZString)id.Value).ToString()];
        }

        ZZObject EvaluateVariableAssignmentExpr(Token idToken)
        {
            ZZString id = (ZZString)idToken.Value;

            Consume(TokenType.EQUALS);

            ZZObject rhsValue = Evaluate();

            if (!Context.Variables.ContainsKey(id.ToString()))
                throw new InterpreterRuntimeException("Tried to assign a value to an undimensioned variable.");

            Context.Variables[id.ToString()] = rhsValue;

            return rhsValue;
        }

        private ZZVoid EvaluateIfExpr()
        {
            Consume(TokenType.IF);
            Consume(TokenType.LEFTPARENTHESIS);

            ZZObject cmpValue = Evaluate();

            if (cmpValue.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use an if statement with a non-integer comparison " +
                    "value.");

            ZZInteger integerComparison = (ZZInteger)cmpValue;

            Consume(TokenType.RIGHTPARENTHESIS);

            Context.ComparisonStack.Push(integerComparison); // Push the comparison value onto the stack
            return ZZVoid.Void;
        }

        private ZZVoid EvaluateWhileExpr()
        {
            Consume(TokenType.WHILE);
            if (Context.ForceNextWhileToEvaluateFalse)
            {
                Context.WhileSkipStack.Push(1);
                Context.ForceNextWhileToEvaluateFalse = false;
                return ZZVoid.Void;
            }

            Consume(TokenType.LEFTPARENTHESIS);

            ZZObject cmpValue = Evaluate();

            if (cmpValue.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use a while loop with a non-integer comparison " +
                    "value.");

            ZZInteger integerComparison = (ZZInteger)cmpValue;

            Consume(TokenType.RIGHTPARENTHESIS);

            // Do this each time and have executor handle the dirty work so interpreter doesn't have to
            //      juggle lines of code and whatnot

            if (integerComparison.Value != 0)
                Context.EnterWhileLoop(Text);
            else
                Context.WhileSkipStack.Push(1);

            return ZZVoid.Void;
        }

        private ZZVoid EvaluatePostExpr()
        {
            Consume(TokenType.POST);
            ZZObject postVal = Evaluate();

            Context.LastReturnValue = postVal;
            return ZZVoid.Void;
        }

        private ZZVoid EvaluateRetExpr()
        {
            Consume(TokenType.RET);

            if (CurrentToken.TokenType != TokenType.EOL)
            {
                ZZObject retVal = Evaluate();

                Context.LastReturnValue = retVal;
            }

            Context.HasReturned = true;
            return ZZVoid.Void;
        }

        private ZZArray EvaluateArrayExpr()
        {
            Consume(TokenType.LEFTCURLYBRACKET);

            List<ZZObject> objectsInArr = new List<ZZObject>();

            while (!MatchDoNotConsume(TokenType.RIGHTCURLYBRACKET))
            {
                objectsInArr.Add(Evaluate());

                if (!MatchDoNotConsume(TokenType.COMMA))
                    break; // Leave if no comma
                else
                    Consume(TokenType.COMMA);
            }

            Consume(TokenType.RIGHTCURLYBRACKET);

            return new ZZArray(objectsInArr.ToArray());
        }

        private ZZObject EvaluateVariableRetrievalExpr(Token idToken)
        {
            ZZObject idVal = idToken.Value;

            if (!Context.Variables.ContainsKey(((ZZString)idVal).ToString()))
                throw new Exception("Tried to look up value of an undimensioned variable.");

            ZZObject val = Context.Variables[((ZZString)idVal).ToString()];

            return val;
        }

        private ZZObject EvaluateStringAccessorExpr(ZZString str)
        {
            Consume(TokenType.LEFTBRACKET);

            ZZObject accessor = Evaluate();

            if (accessor.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use a non-integer value to index a string.");

            Consume(TokenType.RIGHTBRACKET);

            ZZInteger intAccess = (ZZInteger)accessor;
            if (intAccess.Value >= str.Contents.Length)
                throw new InterpreterRuntimeException("String index was out of bounds.");

            if (CurrentToken.TokenType == TokenType.EQUALS)
            {
                Consume(TokenType.EQUALS);
                ZZObject rhs = Evaluate();

                if (rhs.ObjectType != ZZObjectType.STRING)
                    throw new InterpreterRuntimeException("Tried to assign non-string to string position.");

                ZZString strRhs = (ZZString)rhs;

                if (strRhs.Contents.Length != 1)
                    throw new InterpreterRuntimeException("Tried to assign string with a length different than one character to a string position.");

                char[] strContentsMutable = str.Contents.ToArray();
                strContentsMutable[intAccess.Value] = strRhs.Contents[0];
                str.Contents = new string(strContentsMutable);

                return strRhs;
            }

            return EvaluateExpr(new ZZString(str.Contents.ToArray()[intAccess.Value].ToString()));
        }

        private ZZObject EvaluateArrayAccessorExpr(ZZArray arr)
        {
            Consume(TokenType.LEFTBRACKET);

            ZZObject accessor = Evaluate();

            if (accessor.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use a non-integer value to index an array.");

            Consume(TokenType.RIGHTBRACKET);

            if (((ZZInteger)accessor).Value >= arr.Objects.Length || ((ZZInteger)accessor).Value < 0)
                throw new InterpreterRuntimeException("Array index was out of bounds.");

            if (CurrentToken.TokenType == TokenType.EQUALS)
            {
                Consume(TokenType.EQUALS);
                ZZObject rhs = Evaluate();

                arr.Objects[((ZZInteger)accessor).Value] = rhs;
                return rhs;
            }

            return EvaluateExpr(arr.Objects[((ZZInteger)accessor).Value]);
        }

        private ZZObject EvaluateStructFieldExpr(ZZStruct structVal)
        {
            Consume(TokenType.PERIOD);

            ZZString memberText = (ZZString)CurrentToken.Value;
            Consume(TokenType.IDENTIFIER);

            ZZObject fieldVal;
            if (CurrentToken.TokenType == TokenType.EQUALS)
            {
                Consume(TokenType.EQUALS);

                ZZObject lhs = Evaluate();
                if (!structVal.Fields.ContainsKey(memberText))
                    structVal.Fields.Add(memberText, lhs);
                else
                    structVal.Fields[memberText] = lhs;
            }
            else if (!structVal.Fields.ContainsKey(memberText))
                throw new InterpreterRuntimeException("Tried to look up a nonexistent field on a real struct.");

            fieldVal = structVal.Fields[memberText];

            if (MatchDoNotConsume(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH))
                if (fieldVal.ObjectType == ZZObjectType.INTEGER)
                    return EvaluateIntegerBinaryExpr((ZZInteger)fieldVal);
                else if (fieldVal.ObjectType == ZZObjectType.FLOAT)
                    return EvaluateFloatBinaryExpr((ZZFloat)fieldVal);

            return EvaluateExpr(fieldVal);
        }
        
        private ZZObject EvaluateBinaryExpr(ZZObject value)
        {
            if (value.ObjectType == ZZObjectType.FLOAT)
                return EvaluateFloatBinaryExpr((ZZFloat)value);
            else if (value.ObjectType == ZZObjectType.INTEGER)
                return EvaluateIntegerBinaryExpr((ZZInteger)value);

            throw new InterpreterRuntimeException("Tried to involve non-number type in binary expression.");
        }

        private ZZObject EvaluateIdentifierExpr()
        {
            Token identifier = CurrentToken;
            Consume(TokenType.IDENTIFIER);

            switch (CurrentToken.TokenType)
            {
                case TokenType.LEFTPARENTHESIS:
                    return EvaluateFunctionCallExpr(identifier);
                case TokenType.EQUALS:
                    return EvaluateVariableAssignmentExpr(identifier);
            }

            return EvaluateExpr(EvaluateVariableRetrievalExpr(identifier));
        }

        private ZZObject EvaluateEqualityCheckExpr(ZZObject value)
        {
            Consume(TokenType.EQUALSEQUALS);

            ZZObject evalResult = Evaluate();

            return ImplMethods.Compare(value, evalResult);
        }

        private ZZObject EvaluateInequalityCheckExpr(ZZObject value)
        {
            Consume(TokenType.EXCLAMATIONEQUALS);

            ZZObject evalResult = Evaluate();

            return ImplMethods.Negate(ImplMethods.Compare(value, evalResult));
        }

        private ZZObject EvaluateLogicalAndExpr(ZZObject value)
        {
            if (value.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use non-integer for left side of logical and expression.");

            Consume(TokenType.DOUBLEAMPERSAND);

            ZZObject evalResult = Evaluate();

            if (evalResult.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use non-integer for right side of logical and expression.");

            return ImplMethods.Both((ZZInteger)value, (ZZInteger)evalResult);
        }

        private ZZObject EvaluateLogicalOrExpr(ZZObject value)
        {
            if (value.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use non-integer for left side of logical or expression.");

            Consume(TokenType.DOUBLEPIPE);

            ZZObject evalResult = Evaluate();

            if (evalResult.ObjectType != ZZObjectType.INTEGER)
                throw new InterpreterRuntimeException("Tried to use non-integer for right side of logical or expression.");

            return ImplMethods.Either((ZZInteger)value, (ZZInteger)evalResult);
        }

        private ZZObject EvaluateLessThanExpr(ZZObject value)
        {
            Consume(TokenType.LESSTHAN);

            ZZObject evalResult = Evaluate();

            return ImplMethods.LessThan(value, evalResult);
        }

        private ZZObject EvaluateGreaterThanExpr(ZZObject value)
        {
            Consume(TokenType.GREATERTHAN);

            ZZObject evalResult = Evaluate();

            return ImplMethods.GreaterThan(value, evalResult);
        }

        private ZZObject EvaluateExpr(ZZObject value)
        {
            if (((value.ObjectType == ZZObjectType.INTEGER) || (value.ObjectType == ZZObjectType.FLOAT)) &&
                MatchDoNotConsume(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH, 
                TokenType.PERCENTAGE))
                return EvaluateBinaryExpr(value);

            if (MatchDoNotConsume(TokenType.EQUALSEQUALS))
                return EvaluateEqualityCheckExpr(value);

            if (MatchDoNotConsume(TokenType.EXCLAMATIONEQUALS))
                return EvaluateInequalityCheckExpr(value);

            if (MatchDoNotConsume(TokenType.LESSTHAN))
                return EvaluateLessThanExpr(value);

            if (MatchDoNotConsume(TokenType.GREATERTHAN))
                return EvaluateGreaterThanExpr(value);

            if (MatchDoNotConsume(TokenType.DOUBLEAMPERSAND))
                return EvaluateLogicalAndExpr(value);

            if (MatchDoNotConsume(TokenType.DOUBLEPIPE))
                return EvaluateLogicalOrExpr(value);

            switch (CurrentToken.TokenType)
            {
                case TokenType.LEFTBRACKET:
                    if (value.ObjectType == ZZObjectType.ARRAY)
                        return EvaluateArrayAccessorExpr((ZZArray)value);
                    else if (value.ObjectType == ZZObjectType.STRING)
                        return EvaluateStringAccessorExpr((ZZString)value);
                    else
                        throw new InterpreterRuntimeException("Tried to access a non-array with an array accessor.");
                case TokenType.PERIOD:
                    if (value.ObjectType == ZZObjectType.STRUCT)
                        return EvaluateStructFieldExpr((ZZStruct)value);
                    else
                        throw new InterpreterRuntimeException("Tried to access a non-struct with dot notation.");
            }

            return value;
        }

        private ZZObject Evaluate()
        {
            ++EvaluationStackDepth;

            if (Context.ComparisonStack.Count > 0)
            {
                ZZInteger value = Context.ComparisonStack.Peek();

                if (value.Value == 0) // If comparison failed, do not execute unless statement is a THEN
                {
                    if (CurrentToken.TokenType == TokenType.THEN)
                    {
                        if (Context.IfSkipStack.Count > 0)
                            Context.IfSkipStack.Pop();
                        else
                        {
                            Consume(TokenType.THEN);
                            Context.ComparisonStack.Pop(); // If block is over, pop comparison off the stack
                                                           //   and resume normal execution
                        }
                    }

                    if (CurrentToken.TokenType == TokenType.IF)
                        Context.IfSkipStack.Push(1);

                    if (CurrentToken.TokenType != TokenType.ELSE)
                        return ZZVoid.Void;
                }
                else if (CurrentToken.TokenType == TokenType.THEN)
                {
                    Consume(TokenType.THEN);

                    Context.ComparisonStack.Pop(); // Successful if block complete, pop comparison

                    return ZZVoid.Void;
                }
                
                if (CurrentToken.TokenType == TokenType.ELSE)
                {
                    if (Context.IfSkipStack.Count > 0)
                        return ZZVoid.Void;
                    else
                    {
                        Consume(TokenType.ELSE);

                        // Push the inverse comparison onto the comparison stack
                        Context.ComparisonStack.Push(ImplMethods.Negate(Context.ComparisonStack.Pop()));

                        return ZZVoid.Void;
                    }
                }
            }

            if (Context.WhileSkipStack.Any())
            {
                if (CurrentToken.TokenType == TokenType.WHILE)
                    Context.WhileSkipStack.Push(1); // Band-aid fix to fix nested loops

                if (CurrentToken.TokenType == TokenType.LOOP)
                {
                    if (EvaluationStackDepth != 1)
                        throw new InterpreterRuntimeException("Tried to use loop as part of another expression.");

                    Context.WhileSkipStack.Pop();
                }

                return ZZVoid.Void;
            }

            switch (CurrentToken.TokenType)
            {
                case TokenType.LOOP:
                    Debug.Assert(Context.WhileReturnLines.Any());
                    Consume(TokenType.LOOP);

                    if (EvaluationStackDepth != 1)
                        throw new InterpreterRuntimeException("Tried to use loop as part of another expression.");

                    Context.ReturnWhile = true;
                    return ZZVoid.Void;
                case TokenType.ESC:
                    Debug.Assert(Context.WhileReturnLines.Any());
                    Consume(TokenType.ESC);

                    if (EvaluationStackDepth != 1)
                        throw new InterpreterRuntimeException("Tried to use esc as part of another expression.");

                    Context.ReturnWhile = true;
                    Context.ForceNextWhileToEvaluateFalse = true;
                    return ZZVoid.Void;

                case TokenType.WHILE:
                    if (EvaluationStackDepth != 1)
                        throw new InterpreterRuntimeException("Tried to use if as part of another expression.");

                    return EvaluateWhileExpr();

                case TokenType.IF:
                    if (EvaluationStackDepth != 1)
                        throw new InterpreterRuntimeException("Tried to use if as part of another expression.");

                    return EvaluateIfExpr();

                case TokenType.POST:
                    return EvaluatePostExpr();

                case TokenType.THEN:
                    throw new InterpreterRuntimeException("Tried to use then outside of an if block.");

                case TokenType.ELSE:
                    throw new InterpreterRuntimeException("Tried to use else outside of an if block.");

                case TokenType.RET:
                    if (EvaluationStackDepth != 1)
                        // Do not allow other expressions to use ret, as this would be insane
                        throw new InterpreterRuntimeException("Tried to use ret as part of another expression.");
                    else
                        return EvaluateRetExpr();

                case TokenType.LEFTCURLYBRACKET:
                    return EvaluateArrayExpr();

                case TokenType.LEFTPARENTHESIS:
                    {
                        Consume(TokenType.LEFTPARENTHESIS);
                        ZZObject evalResult = Evaluate();
                        Consume(TokenType.RIGHTPARENTHESIS);

                        return EvaluateExpr(evalResult);
                    }

                case TokenType.EXCLAMATION:
                    {
                        Consume(TokenType.EXCLAMATION);
                        ZZObject evalResult = Evaluate();
                        if (evalResult.ObjectType == ZZObjectType.INTEGER)
                            return ImplMethods.Negate((ZZInteger)evalResult);
                        else if (evalResult.ObjectType == ZZObjectType.FILEHANDLE)
                            return ImplMethods.Negate((ZZInteger)ImplMethods.ConvertToInteger(evalResult));
                        else
                            throw new InterpreterRuntimeException("Tried to negate a non-integer value.");
                    }

                case TokenType.MINUS:
                    {
                        // No more parentheses required for this
                        Consume(TokenType.MINUS);

                        ZZObject evalResult = ZZVoid.Void;
                        switch (CurrentToken.TokenType)
                        {
                            case TokenType.IDENTIFIER:
                                evalResult = EvaluateVariableRetrievalExpr(CurrentToken);
                                break;
                            case TokenType.INTEGER_CONSTANT:
                                {
                                    Token constant = CurrentToken;
                                    Consume(TokenType.INTEGER_CONSTANT);
                                    return new ZZInteger(-((ZZInteger)constant.Value).Value);
                                }
                            case TokenType.FLOAT_CONSTANT:
                                {
                                    Token constant = CurrentToken;
                                    Consume(TokenType.FLOAT_CONSTANT);
                                    return new ZZFloat(-((ZZFloat)constant.Value).Value);
                                }
                        }

                        if (evalResult.ObjectType == ZZObjectType.INTEGER)
                            return EvaluateExpr(new ZZInteger(-((ZZInteger)evalResult).Value));
                        else if (evalResult.ObjectType == ZZObjectType.FLOAT)
                            return EvaluateExpr(new ZZFloat(-((ZZFloat)evalResult).Value));
                        else
                            throw new InterpreterRuntimeException("Tried to make negative a non-integer, " +
                                "non-floating point value.");
                    }

                case TokenType.IDENTIFIER:
                    return EvaluateIdentifierExpr();

                case TokenType.MKREF:
                    {
                        Consume(TokenType.MKREF);
                        Consume(TokenType.LEFTPARENTHESIS);
                        Token id = CurrentToken;
                        Consume(TokenType.IDENTIFIER);
                        Consume(TokenType.RIGHTPARENTHESIS);

                        ZZString fnName = (ZZString)id.Value;

                        ZZFunctionDescriptor[] allDescriptors = Program.GlobalContext.FunctionsAndBodies.Keys.ToArray();
                        long idx = -1;

                        List<string> functionNames = new List<string>();
                        foreach (var v in allDescriptors)
                            functionNames.Add(v.Name);

                        for (long n = 0; n < allDescriptors.Length; ++n)
                            if (allDescriptors[n].Name == fnName.Contents
                                && Program.CurrentlyExecutingContext
                                    .ImportedNamespaces.Contains(allDescriptors[n].Namespace)) /*Make sure it's imported*/
                            {
                                idx = n;
                                break;
                            }

                        if (idx == -1)
                        {
                            throw new InterpreterRuntimeException("Tried to make reference to nonexistent function." +
                                $" Identifier was {fnName.Contents} but available functions were " +
                                $"{{ {string.Join(", ", functionNames)} }}");
                        }

                        return EvaluateExpr(new ZZInteger(idx));
                    }

                case TokenType.DEREF:
                    {
                        Consume(TokenType.DEREF);
                        Consume(TokenType.LEFTPARENTHESIS);

                        ZZObject value = Evaluate();

                        Consume(TokenType.RIGHTPARENTHESIS);

                        switch (value.ObjectType)
                        {
                            case ZZObjectType.INTEGER:
                                long idx = ((ZZInteger)value).Value;

                                if (idx > -1 && idx < Program.GlobalContext.FunctionsAndBodies.Count)
                                    // Faster by nanoseconds than checking the other way
                                {
                                    ZZFunctionDescriptor desc = 
                                        Program.GlobalContext.FunctionsAndBodies.Keys.ToArray()[idx];

                                    // TODO: Optimize this by skipping identifier step since we already
                                    //  know which function it is
                                    return EvaluateFunctionCallExpr(new Token(TokenType.IDENTIFIER, (ZZString)desc.Name));
                                }

                                throw new InterpreterRuntimeException("Tried to dereference function with an invalid index.");

                            default:
                                throw new InterpreterRuntimeException("Tried to dereference function with a non-zero index.");
                        }
                    }

                case TokenType.VOID:
                case TokenType.STRING_CONSTANT:
                case TokenType.INTEGER_CONSTANT:
                case TokenType.FLOAT_CONSTANT:
                    {
                        ZZObject val = CurrentToken.Value;
                        ConsumeAny();
                        return EvaluateExpr(val);
                    }
                case TokenType.INCREMENT:
                    {
                        Consume(TokenType.INCREMENT);
                        Consume(TokenType.LEFTPARENTHESIS);

                        Token id = CurrentToken;
                        Consume(TokenType.IDENTIFIER);

                        Consume(TokenType.RIGHTPARENTHESIS);

                        string name = ((ZZString)id.Value).Contents;
                        if (!Context.Variables.ContainsKey(name))
                            throw new InterpreterRuntimeException("Tried to fast-increment a nonexistent variable.");

                        ZZObject variable = Context.Variables[name];

                        // To my future self:
                        //  This keyword caused a massive headache because it was modifying the Zero
                        //      special integer value.
                        //  It took forever to hunt down, so don't make that mistake again.
                        
                        switch (variable.ObjectType)
                        {
                            case ZZObjectType.INTEGER:
                                // Do this in place, extremely fast

                                ZZInteger intVar = (ZZInteger)variable;

                                // Be careful not to overwrite the special values
                                switch (intVar.Value)
                                {
                                    case -1:
                                    case 0:
                                    case 1:
                                        Context.Variables[name] = new ZZInteger(intVar.Value + 1);
                                        break;
                                    default:
                                        // intVar.Value = Context.GetIncrementResultUsingTable(intVar);
                                        // ++intVar.Value;
                                        FastOps.FastInc(intVar);
                                        break;
                                }

                                break;
                            default:
                                throw new InterpreterRuntimeException("Tried to fast-increment a non-integer.");
                        }

                        return variable;
                    }
                case TokenType.VARIABLE_DEFINITION:
                    return EvaluateVariableDimensionExpr();

                case TokenType.EOL:
                    return ZZVoid.Void;
                default:
                    throw new Exception();
            }

            throw new Exception();
        }

        public ZZObject Execute()
        {
            CurrentToken = GetNextToken();
            return Evaluate();
        }
    }
}
