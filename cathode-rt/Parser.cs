using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public class Parser
    {
        Token[] Tokens;
        int Position;
        int ParseRecursionCount = 0;
        Token Peek() => Tokens[Position];
        bool EndOfStream => (Position + 1) >= Tokens.Length;

        public bool MatchDoNotConsume(params TokenType[] tokenTypes)
        {
            return tokenTypes.Contains(Tokens[Position].TokenType);
        }

        public Token Next()
        {
            Token tok = Tokens[Position];
            ++Position;
            return tok;
        }

        public void Consume(params TokenType[] tokenTypes)
        {
            if (tokenTypes.Length < 1)
                ++Position;
            else if (tokenTypes.Contains(Tokens[Position].TokenType))
                ++Position;
            else
                throw GenerateBadTokException();
        }

        public Parser(Token[] tokens)
        {
            Tokens = tokens;
            Position = 0;
        }

        private InterpreterRuntimeException GenerateBadTokException()
        {
            return new InterpreterRuntimeException($"Unexpected token of type {Enum.GetName(Peek().TokenType)}.");
        }

        public static readonly TokenType[] BinaryExprTokenTypes = { TokenType.ASTERISK, TokenType.BACKSLASH,
            TokenType.MINUS, TokenType.PLUS, TokenType.DOUBLEAMPERSAND, TokenType.DOUBLEPIPE, 
            TokenType.EQUALSEQUALS, TokenType.EXCLAMATIONEQUALS, TokenType.LESSTHAN, TokenType.GREATERTHAN,
            TokenType.PERCENTAGE, TokenType.CARET };
        public SyntaxTree.SyntaxTreeNode ParseBinaryExpr(SyntaxTree.SyntaxTreeNode lhs)
        {
            Token peek = Next();
            switch (peek.TokenType)
            {
                case TokenType.ASTERISK:
                    return new SyntaxTree.MulSyntaxTreeNode(lhs, ParseLine());
                case TokenType.BACKSLASH:
                    return new SyntaxTree.DivSyntaxTreeNode(lhs, ParseLine());
                case TokenType.MINUS:
                    return new SyntaxTree.SubSyntaxTreeNode(lhs, ParseLine());
                case TokenType.PLUS:
                    return new SyntaxTree.AddSyntaxTreeNode(lhs, ParseLine());
                case TokenType.CARET:
                    return new SyntaxTree.XorSyntaxTreeNode(lhs, ParseLine());
                case TokenType.PERCENTAGE:
                    return new SyntaxTree.RemainderDivSyntaxTreeNode(lhs, ParseLine());
                case TokenType.DOUBLEAMPERSAND:
                    return new SyntaxTree.LogicalAndSyntaxTreeNode(lhs, ParseLine());
                case TokenType.DOUBLEPIPE:
                    return new SyntaxTree.LogicalOrSyntaxTreeNode(lhs, ParseLine());
                case TokenType.EQUALSEQUALS:
                    return new SyntaxTree.LogicalEqualsSyntaxTreeNode(lhs, ParseLine());
                case TokenType.EXCLAMATIONEQUALS:
                    return new SyntaxTree.LogicalNotEqualsSyntaxTreeNode(lhs, ParseLine());
                case TokenType.LESSTHAN:
                    return new SyntaxTree.LessThanSyntaxTreeNode(lhs, ParseLine());
                case TokenType.GREATERTHAN:
                    return new SyntaxTree.GreaterThanSyntaxTreeNode(lhs, ParseLine());
                default:
                    throw GenerateBadTokException();
            }
        }

        public SyntaxTree.SyntaxTreeNode ParseAssignmentExpr(string name)
        {
            Consume(); // Eat the =

            SyntaxTree.SyntaxTreeNode rhs = ParseLine();
            return new SyntaxTree.AssignVariableSyntaxTreeNode(name, rhs);
        }

        public SyntaxTree.SyntaxTreeNode ParseFunctionCallExpr(string idName)
        {
            Consume(TokenType.LEFTPARENTHESIS);

            List<SyntaxTree.SyntaxTreeNode> args = new List<SyntaxTree.SyntaxTreeNode>();
            while (!MatchDoNotConsume(TokenType.RIGHTPARENTHESIS))
            {
                args.Add(ParseLine());

                if (!MatchDoNotConsume(TokenType.COMMA))
                    break;
                else
                    Consume(TokenType.COMMA);
            }

            Consume(TokenType.RIGHTPARENTHESIS);

            return ParseExpr(new SyntaxTree.FunctionCallSyntaxTreeNode(idName, args.ToArray()));
        }

        public SyntaxTree.SyntaxTreeNode ParseExpr(SyntaxTree.SyntaxTreeNode lhs)
        {
            if (MatchDoNotConsume(BinaryExprTokenTypes))
                return ParseBinaryExpr(lhs);
            else if (MatchDoNotConsume(TokenType.PERIOD))
            {
                Consume(TokenType.PERIOD);
                Token id = Peek();
                Consume(TokenType.IDENTIFIER);

                if (MatchDoNotConsume(TokenType.EQUALS))
                {
                    Consume(TokenType.EQUALS);
                    return new SyntaxTree.AssignStructFieldSyntaxTreeNode(lhs, (string)id.Value, ParseLine());
                }

                return ParseExpr(new SyntaxTree.StructFieldSyntaxTreeNode(lhs, (string)id.Value));
            }
            else if (MatchDoNotConsume(TokenType.LEFTBRACKET))
            {
                Consume(TokenType.LEFTBRACKET);
                SyntaxTree.SyntaxTreeNode accessor = ParseLine();
                Consume(TokenType.RIGHTBRACKET);

                if (MatchDoNotConsume(TokenType.EQUALS))
                {
                    Consume(TokenType.EQUALS);
                    return new SyntaxTree.AssignArrayIdxSyntaxTreeNode(lhs, accessor, ParseLine());
                }

                return ParseExpr(new SyntaxTree.ArrayIdxSyntaxTreeNode(lhs, accessor));
            }
            else
                return lhs;
        }

        public SyntaxTree.SyntaxTreeNode ParseArrayExpr()
        {
            List<SyntaxTree.SyntaxTreeNode> elems = new List<SyntaxTree.SyntaxTreeNode>();

            while (!MatchDoNotConsume(TokenType.RIGHTCURLYBRACKET))
            {
                SyntaxTree.SyntaxTreeNode elem = ParseLine();
                elems.Add(elem);

                if (!MatchDoNotConsume(TokenType.COMMA))
                    break;
                else
                    Consume(TokenType.COMMA);
            }

            Consume(TokenType.RIGHTCURLYBRACKET);

            return new SyntaxTree.ArraySyntaxTreeNode(elems.ToArray());
        }

        public SyntaxTree.SyntaxTreeNode ParseLine()
        {
            ++ParseRecursionCount;

            Token tok = Next();
            switch (tok.TokenType)
            {
                case TokenType.EXCLAMATION:
                    {
                        SyntaxTree.SyntaxTreeNode invert = ParseLine();
                        return new SyntaxTree.NotSyntaxTreeNode(invert);
                    }
                case TokenType.MINUS:
                    {
                        SyntaxTree.SyntaxTreeNode changeSign = ParseLine();
                        return new SyntaxTree.FlipSignSyntaxTreeNode(changeSign);
                    }
                case TokenType.LEFTCURLYBRACKET:
                    return ParseArrayExpr();
                case TokenType.INCREMENT:
                    {
                        Consume(TokenType.LEFTPARENTHESIS);
                        Token idToken = Tokens[Position];
                        Consume(TokenType.IDENTIFIER);
                        Consume(TokenType.RIGHTPARENTHESIS);
                        return new SyntaxTree.IncrementSyntaxTreeNode((string)idToken.Value);
                    }
                case TokenType.LEFTPARENTHESIS:
                    {
                        SyntaxTree.SyntaxTreeNode current = ParseLine();
                        Consume(TokenType.RIGHTPARENTHESIS);
                        return ParseExpr(current);
                    }
                case TokenType.IDENTIFIER:
                    {
                        if (MatchDoNotConsume(BinaryExprTokenTypes))
                            return ParseBinaryExpr(new SyntaxTree.VariableSyntaxTreeNode((string)tok.Value));
                        else if (MatchDoNotConsume(TokenType.LEFTPARENTHESIS))
                            return ParseFunctionCallExpr((string)tok.Value);
                        else if (MatchDoNotConsume(TokenType.EQUALS))
                            return ParseAssignmentExpr((string)tok.Value);
                        else
                            return ParseExpr(new SyntaxTree.VariableSyntaxTreeNode((string)tok.Value));
                    }
                case TokenType.INTEGER_CONSTANT:
                    {
                        if (MatchDoNotConsume(BinaryExprTokenTypes))
                            return ParseBinaryExpr(new SyntaxTree.IntegerSyntaxTreeNode((long)tok.Value));
                        else
                            return new SyntaxTree.IntegerSyntaxTreeNode((long)tok.Value);
                    }
                case TokenType.FLOAT_CONSTANT:
                    {
                        if (MatchDoNotConsume(BinaryExprTokenTypes))
                            return ParseBinaryExpr(new SyntaxTree.FloatSyntaxTreeNode((double)tok.Value));
                        else
                            return new SyntaxTree.FloatSyntaxTreeNode((double)tok.Value);
                    }
                case TokenType.STRING_CONSTANT:
                    {
                        if (MatchDoNotConsume(BinaryExprTokenTypes))
                            return ParseBinaryExpr(new SyntaxTree.StringSyntaxTreeNode((string)tok.Value));
                        else
                            return new SyntaxTree.StringSyntaxTreeNode((string)tok.Value);
                    }
                case TokenType.VARIABLE_DEFINITION:
                    {
                        Token identifier = Next();

                        if (identifier.TokenType != TokenType.IDENTIFIER)
                            throw GenerateBadTokException();

                        if (MatchDoNotConsume(TokenType.EQUALS))
                        {
                            Consume();
                            return new SyntaxTree.DimensionAndAssignSyntaxTreeNode((string)identifier.Value,
                                ParseLine());
                        }
                        else
                            return new SyntaxTree.DimensionSyntaxTreeNode((string)identifier.Value);
                    }
                case TokenType.VOID:
                    return new SyntaxTree.VoidSyntaxTreeNode();
                case TokenType.ESC:
                    if (ParseRecursionCount > 1)
                        throw new InterpreterRuntimeException("Tried to use esc as part of another expression.");

                    return new SyntaxTree.EscSyntaxTreeNode();
                case TokenType.RET:
                    if (ParseRecursionCount > 1)
                        throw new InterpreterRuntimeException("Tried to use ret as part of another expression.");

                    if (Peek().TokenType == TokenType.EOL)
                        return new SyntaxTree.RetSyntaxTreeNode(null);
                    
                    return new SyntaxTree.RetSyntaxTreeNode(ParseLine());
                case TokenType.POST:
                    if (ParseRecursionCount > 1)
                        throw new InterpreterRuntimeException("Tried to use ret as part of another expression.");

                    return new SyntaxTree.PostSyntaxTreeNode(ParseLine());
                case TokenType.THEN:
                    if (ParseRecursionCount > 1)
                        throw new InterpreterRuntimeException("Tried to use then as part of another expression.");

                    return new SyntaxTree.ThenSyntaxTreeNode();
                case TokenType.WHILE:
                    {
                        Consume(TokenType.LEFTPARENTHESIS);
                        SyntaxTree.SyntaxTreeNode expr = ParseLine();
                        Consume(TokenType.RIGHTPARENTHESIS);

                        // Backup parse recursion count
                        int iBackup = ParseRecursionCount;
                        ParseRecursionCount = 0;
                        SyntaxTree.FunctionBodySyntaxTreeNode body = Parse(TokenType.LOOP).GetMainNode();
                        ParseRecursionCount = iBackup;

                        // Eat the loop
                        Consume(TokenType.LOOP);

                        return new SyntaxTree.WhileSyntaxTreeNode(expr, body);
                    }
                case TokenType.FOR:
                    {
                        Consume(TokenType.LEFTPARENTHESIS);
                        SyntaxTree.SyntaxTreeNode initial = ParseLine();
                        Consume(TokenType.COMMA);
                        SyntaxTree.SyntaxTreeNode condition = ParseLine();
                        Consume(TokenType.COMMA);
                        SyntaxTree.SyntaxTreeNode increment = ParseLine();
                        Consume(TokenType.RIGHTPARENTHESIS);

                        // Backup parse recursion count
                        int iBackup = ParseRecursionCount;
                        ParseRecursionCount = 0;
                        SyntaxTree.FunctionBodySyntaxTreeNode body = Parse(TokenType.LOOP).GetMainNode();
                        ParseRecursionCount = iBackup;

                        // Consume final loop
                        Consume(TokenType.LOOP);

                        return new SyntaxTree.ForSyntaxTreeNode(initial, condition, increment, body);
                    }
                case TokenType.IF:
                    {
                        Consume(TokenType.LEFTPARENTHESIS);
                        SyntaxTree.SyntaxTreeNode expr = ParseLine();
                        Consume(TokenType.RIGHTPARENTHESIS);

                        // Backup parse recursion count
                        int iBackup = ParseRecursionCount;
                        ParseRecursionCount = 0;
                        SyntaxTree.FunctionBodySyntaxTreeNode body = Parse(TokenType.THEN, TokenType.ELSE).GetMainNode();

                        if (MatchDoNotConsume(TokenType.ELSE))
                        {
                            ParseRecursionCount = 0;
                            Consume(TokenType.ELSE);
                            SyntaxTree.FunctionBodySyntaxTreeNode _else = Parse(TokenType.THEN).GetMainNode();
                            Consume(TokenType.THEN);
                            ParseRecursionCount = iBackup;
                            return new SyntaxTree.IfSyntaxTreeNode(expr, body, _else);
                        }
                        else
                        {
                            Consume(TokenType.THEN);
                            ParseRecursionCount = iBackup;
                            return new SyntaxTree.IfSyntaxTreeNode(expr, body);
                        }
                    }
                case TokenType.EOL:
                    // Ignore
                    return null;
                default:
                    throw GenerateBadTokException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminalTokenTypes">Additional tokens that will terminate parsing.</param>
        /// <returns></returns>
        public SyntaxTree Parse(params TokenType[] terminalTokenTypes)
        {
            SyntaxTree workingTree = new SyntaxTree();

            while (Position < Tokens.Length && Tokens[Position].TokenType != TokenType.EOF &&
                !terminalTokenTypes.Contains(Tokens[Position].TokenType))
            {
                ParseRecursionCount = 0;
                SyntaxTree.SyntaxTreeNode lineParsed = ParseLine();
                if (lineParsed != null)
                    workingTree.AddNode(lineParsed);
            }

            return workingTree;
        }
    }
}
