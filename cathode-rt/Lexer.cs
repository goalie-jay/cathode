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
        CARET,
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
        FOR,
        LOOP,
        ESC,
        MKREF,
        DEREF,

        EOL,
        EOF
    }

    public class InterpreterRuntimeException : Exception
    {
        public InterpreterRuntimeException(string details) : base(details) { }
    }

    public class Token
    {
        public object Value { get; private set; }
        public TokenType TokenType { get; private set; }

        public Token(TokenType type, object value)
        {
            Value = value;
            TokenType = type;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(TokenType), TokenType) + ": " + Value.ToString();
        }
    }

    public class Lexer
    {
        public string[] Lines;
        public string Text { get; private set; }
        int Position;
        Token CurrentToken;

        private int Atoi(char val)
        {
            return (int)val - 48;
        }

        public Lexer(string[] lines)
        {
            Lines = lines;
            Position = 0;
            CurrentToken = null;
        }

        char CurrentChar => Text[Position];
        Token GetToken()
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

            if (CurrentChar == 'f')
            {
                int posBackup = Position;
                ++Position;
                if (Position < Text.Length && CurrentChar == 'o')
                {
                    ++Position;
                    if (Position < Text.Length && CurrentChar == 'r')
                    {
                        ++Position;
                        if (Position >= Text.Length || char.IsWhiteSpace(CurrentChar))
                        {
                            ++Position;
                            return new Token(TokenType.FOR, null);
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

                        return new Token(TokenType.FLOAT_CONSTANT, (double)floatConstant);
                    }
                }

                return new Token(TokenType.INTEGER_CONSTANT, (long)constant);
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

                return new Token(TokenType.IDENTIFIER, (string)identifier.ToString());
            }
            else if (CurrentChar == '"')
            {
                ++Position;

                if (Position >= Text.Length)
                    throw new InterpreterRuntimeException("Line terminated before string literal ended.");

                StringBuilder stringLiteral = new StringBuilder();

                while (CurrentChar != '"')
                {
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
                    else
                    {
                        stringLiteral.Append(CurrentChar);
                        ++Position;

                        if (Position >= Text.Length)
                            break;
                    }
                }

                ++Position;
                return new Token(TokenType.STRING_CONSTANT, (string)stringLiteral.ToString());
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
                    case '^':
                        ++Position;
                        return new Token(TokenType.CARET, null);
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

            throw new InterpreterRuntimeException($"Unexpected character {CurrentChar}.");
        }

        public Token[] Lex()
        {
            List<Token> tokens = new List<Token>();

            foreach (string line in Lines)
            {
                Position = 0;
                Text = line;
                Token tok = GetToken();
                while (tok.TokenType != TokenType.EOL)
                {
                    tokens.Add(tok);
                    tok = GetToken();
                }
                tokens.Add(new Token(TokenType.EOL, null));
            }

            tokens.Add(new Token(TokenType.EOF, null));
            return tokens.ToArray();
        }
    }
}
