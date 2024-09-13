using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        EQUALSEQUALS,
        IF,
        ELSE,
        THEN,
        POST,
        RET,
        VOID,
        WHILE,
        LOOP,

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
                                return new Token(TokenType.THEN, (ZZString)"then");
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
                        return new Token(TokenType.IF, (ZZString)"if");
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
                                return new Token(TokenType.ELSE, (ZZString)"else");
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
                            return new Token(TokenType.VARIABLE_DEFINITION, (ZZString)"dim");
                        }
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
                            return new Token(TokenType.RET, (ZZString)"ret");
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
                                return new Token(TokenType.POST, (ZZString)"post");
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
                                return new Token(TokenType.VOID, (ZZString)"void");
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
                                    return new Token(TokenType.WHILE, (ZZString)"while");
                                }
                            }
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
                                return new Token(TokenType.LOOP, (ZZString)"loop");
                            }
                        }
                    }
                }

                Position = posBackup;
            }

            if (char.IsDigit(CurrentChar))
            {
                int constant = 0;

                while (char.IsDigit(CurrentChar))
                {
                    constant = (constant * 10) + int.Parse(CurrentChar.ToString());
                    ++Position;

                    if (Position >= Text.Length)
                        break;

                    // Float support
                    if (CurrentChar == '.')
                    {
                        float floatConstant = (float)constant;
                        int depth = 1;

                        ++Position;
                        if (Position >= Text.Length)
                            throw new InterpreterRuntimeException("Decimal point at the end of the line is " +
                                "not supported for float constants.");

                        while (char.IsDigit(CurrentChar))
                        {
                            floatConstant = floatConstant + (int.Parse(CurrentChar.ToString()) / 
                                (float)Math.Pow(10, depth));

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

                Debug.Assert(identifier.ToString() != "void");

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
                        return new Token(TokenType.PLUS, (ZZString)"+");
                    case '-':
                        ++Position;
                        return new Token(TokenType.MINUS, (ZZString)"-");
                    case '*':
                        ++Position;
                        return new Token(TokenType.ASTERISK, (ZZString)"*");
                    case '/':
                        ++Position;
                        return new Token(TokenType.BACKSLASH, (ZZString)"/");
                    case '(':
                        ++Position;
                        return new Token(TokenType.LEFTPARENTHESIS, (ZZString)"(");
                    case ')':
                        ++Position;
                        return new Token(TokenType.RIGHTPARENTHESIS, (ZZString)")");
                    case '{':
                        ++Position;
                        return new Token(TokenType.LEFTCURLYBRACKET, (ZZString)"{");
                    case '}':
                        ++Position;
                        return new Token(TokenType.RIGHTCURLYBRACKET, (ZZString)"}");
                    case '[':
                        ++Position;
                        return new Token(TokenType.LEFTBRACKET, (ZZString)"[");
                    case ']':
                        ++Position;
                        return new Token(TokenType.RIGHTBRACKET, (ZZString)"]");
                    case ',':
                        ++Position;
                        return new Token(TokenType.COMMA, (ZZString)",");
                    case '.':
                        ++Position;
                        return new Token(TokenType.PERIOD, (ZZString)".");
                    case '=':
                        ++Position;
                        return new Token(TokenType.EQUALS, (ZZString)"=");
                    case '!':
                        ++Position;
                        return new Token(TokenType.EXCLAMATION, (ZZString)"!");
                    case '<':
                        ++Position;
                        return new Token(TokenType.LESSTHAN, (ZZString)"<");
                    case '>':
                        ++Position;
                        return new Token(TokenType.GREATERTHAN, (ZZString)">");
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

                using (ExecutionContext callContext = new ExecutionContext())
                {
                    for (int i = 0; i < parameters.Count; ++i)
                        callContext.Variables.Add(descriptor.Arguments[i],
                            parameters[i]); // Line up our values with the parameter names

                    return Executor.Execute(callContext, descriptor.Name, userFn);
                }
            }

            try
            {
                return (ZZObject)methodInf.Invoke(null, parameters.ToArray());
            }
            catch (ArgumentException)
            {
                throw new InterpreterRuntimeException("Tried to supply either an incorrect amount of parameters " +
                    "or parameters with incorrect types to a standard function. " +
                    "Are you calling the correct function? Did you forget " +
                    "a type conversion?");
            }
        }

        private ZZFloat EvaluateFloatBinaryExpr(ZZFloat lhsVal)
        {
            Token operation = CurrentToken;
            ConsumeAny(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH);

            ZZObject testRhs = Evaluate();

            if (!(testRhs is ZZFloat))
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
            ConsumeAny(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH);

            ZZObject testRhs = Evaluate();

            if (!(testRhs is ZZInteger))
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

            Context.Variables.Add(((ZZString)id.Value).ToString(), new ZZVoid());

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

            if (!(cmpValue is ZZInteger integerComparison))
                throw new InterpreterRuntimeException("Tried to use an if statement with a non-integer comparison " +
                    "value.");

            Consume(TokenType.RIGHTPARENTHESIS);

            Context.ComparisonStack.Push(integerComparison); // Push the comparison value onto the stack
            return new ZZVoid();
        }

        private ZZVoid EvaluateWhileExpr()
        {
            Consume(TokenType.WHILE);
            Consume(TokenType.LEFTPARENTHESIS);

            ZZObject cmpValue = Evaluate();

            if (!(cmpValue is ZZInteger integerComparison))
                throw new InterpreterRuntimeException("Tried to use a while loop with a non-integer comparison " +
                    "value.");

            Consume(TokenType.RIGHTPARENTHESIS);

            // Do this each time and have executor handle the dirty work so interpreter doesn't have to
            //      juggle lines of code and whatnot

            if (integerComparison.Value != 0)
                Context.EnterWhileLoop(Text);
            else
                Context.WhileSkipStack.Push(1);

            return new ZZVoid();
        }

        private ZZVoid EvaluatePostExpr()
        {
            Consume(TokenType.POST);
            ZZObject postVal = Evaluate();

            Context.LastReturnValue = postVal;
            return new ZZVoid();
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
            return new ZZVoid();
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

        private ZZObject EvaluateArrayAccessorExpr(ZZArray arr)
        {
            Consume(TokenType.LEFTBRACKET);

            ZZObject accessor = Evaluate();

            if (!(accessor is ZZInteger))
                throw new InterpreterRuntimeException("Tried to use a non-integer value to index an array.");

            Consume(TokenType.RIGHTBRACKET);

            if (CurrentToken.TokenType == TokenType.EQUALS)
            {
                Consume(TokenType.EQUALS);
                ZZObject rhs = Evaluate();

                arr.Objects[((ZZInteger)accessor).Value] = rhs;
                return rhs;
            }

            if (((ZZInteger)accessor).Value >= arr.Objects.Length || ((ZZInteger)accessor).Value < 0)
                throw new InterpreterRuntimeException("Array index was out of bounds.");

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
                if (fieldVal is ZZInteger zint)
                    return EvaluateIntegerBinaryExpr(zint);
                else if (fieldVal is ZZFloat zfloat)
                    return EvaluateFloatBinaryExpr(zfloat);

            return EvaluateExpr(fieldVal);
        }
        
        private ZZObject EvaluateBinaryExpr(ZZObject value)
        {
            if (value is ZZFloat zflt)
                return EvaluateFloatBinaryExpr(zflt);
            else if (value is ZZInteger zint)
                return EvaluateIntegerBinaryExpr(zint);

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

        private ZZObject EvaluateExpr(ZZObject value)
        {
            if ((value is ZZInteger || value is ZZFloat) &&
                MatchDoNotConsume(TokenType.PLUS, TokenType.MINUS, TokenType.ASTERISK, TokenType.BACKSLASH))
                return EvaluateBinaryExpr(value);

            switch (CurrentToken.TokenType)
            {
                case TokenType.LEFTBRACKET:
                    if (value is ZZArray zarr)
                        return EvaluateArrayAccessorExpr(zarr);
                    else
                        throw new InterpreterRuntimeException("Tried to access a non-array with an array accessor.");
                case TokenType.PERIOD:
                    if (value is ZZStruct strct)
                        return EvaluateStructFieldExpr(strct);
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
                        return new ZZVoid();
                }
                else if (CurrentToken.TokenType == TokenType.THEN)
                {
                    Consume(TokenType.THEN);

                    Context.ComparisonStack.Pop(); // Successful if block complete, pop comparison

                    return new ZZVoid();
                }
                
                if (CurrentToken.TokenType == TokenType.ELSE)
                {
                    if (Context.IfSkipStack.Count > 0)
                        return new ZZVoid();
                    else
                    {
                        Consume(TokenType.ELSE);

                        // Push the inverse comparison onto the comparison stack
                        Context.ComparisonStack.Push(ImplMethods.Negate(Context.ComparisonStack.Pop()));

                        return new ZZVoid();
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

                return new ZZVoid();
            }

            switch (CurrentToken.TokenType)
            {
                case TokenType.LOOP:
                    Debug.Assert(Context.WhileReturnLines.Any());
                    Consume(TokenType.LOOP);

                    if (EvaluationStackDepth != 1)
                        throw new InterpreterRuntimeException("Tried to use loop as part of another expression.");

                    Context.ReturnWhile = true;
                    return new ZZVoid();

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

                case TokenType.VOID:
                    Consume(TokenType.VOID);
                    return new ZZVoid();

                case TokenType.LEFTCURLYBRACKET:
                    return EvaluateArrayExpr();

                case TokenType.LEFTPARENTHESIS:
                    {
                        Consume(TokenType.LEFTPARENTHESIS);
                        ZZObject evalResult = Evaluate();
                        Consume(TokenType.RIGHTPARENTHESIS);

                        return evalResult;
                    }

                case TokenType.EXCLAMATION:
                    {
                        Consume(TokenType.EXCLAMATION);
                        ZZObject evalResult = Evaluate();
                        if (evalResult is ZZInteger)
                            return ImplMethods.Negate((ZZInteger)evalResult);
                        else
                            throw new InterpreterRuntimeException("Tried to negate a non-integer value.");
                    }

                case TokenType.MINUS:
                    {
                        // Require parentheses
                        Consume(TokenType.MINUS);
                        Consume(TokenType.LEFTPARENTHESIS);

                        ZZObject evalResult = Evaluate();

                        Consume(TokenType.RIGHTPARENTHESIS);

                        if (evalResult is ZZInteger zint)
                            return EvaluateExpr(new ZZInteger(-zint.Value));
                        else if (evalResult is ZZFloat zflt)
                            return EvaluateExpr(new ZZFloat(-zflt.Value));
                        else
                            throw new InterpreterRuntimeException("Tried to make negative a non-integer, " +
                                "non-floating point value.");
                    }

                case TokenType.IDENTIFIER:
                    return EvaluateIdentifierExpr();

                case TokenType.STRING_CONSTANT:
                case TokenType.INTEGER_CONSTANT:
                case TokenType.FLOAT_CONSTANT:
                    {
                        ZZObject val = CurrentToken.Value;
                        ConsumeAny();
                        return EvaluateExpr(val);
                    }
                case TokenType.VARIABLE_DEFINITION:
                    return EvaluateVariableDimensionExpr();

                case TokenType.EOL:
                    return new ZZVoid();
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
