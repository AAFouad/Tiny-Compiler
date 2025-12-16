using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    // Reserved words
    T_Main, T_Int, T_Float, T_String, T_Read, T_Write,
    T_Repeat, T_Until, T_If, T_ElseIf, T_Else, T_Then,
    T_End, T_Endl, T_Return,

    // Operators
    T_Dot, T_Semicolon, T_Comma, T_LParanthesis, T_RParanthesis,
    T_EqualOp, T_CondtionEqual, T_LessThanOp, T_GreaterThanOp, T_NotEqualOp,
    T_PlusOp, T_MinusOp, T_MultiplyOp, T_DivideOp,
    T_Or, T_And,
    T_LCurly, T_RCurly, T_LSquare, T_RSquare,

    // Defined
    T_Comment, T_Identifier, T_Constant
}

namespace Tiny_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            // Reserved words
            ReservedWords.Add("MAIN", Token_Class.T_Main);
            ReservedWords.Add("INT", Token_Class.T_Int);
            ReservedWords.Add("FLOAT", Token_Class.T_Float);
            ReservedWords.Add("STRING", Token_Class.T_String);
            ReservedWords.Add("READ", Token_Class.T_Read);
            ReservedWords.Add("WRITE", Token_Class.T_Write);
            ReservedWords.Add("REPEAT", Token_Class.T_Repeat);
            ReservedWords.Add("UNTIL", Token_Class.T_Until);
            ReservedWords.Add("IF", Token_Class.T_If);
            ReservedWords.Add("ELSEIF", Token_Class.T_ElseIf);
            ReservedWords.Add("ELSE", Token_Class.T_Else);
            ReservedWords.Add("THEN", Token_Class.T_Then);
            ReservedWords.Add("END", Token_Class.T_End);
            ReservedWords.Add("ENDL", Token_Class.T_Endl);
            ReservedWords.Add("RETURN", Token_Class.T_Return);

            // Operators
            Operators.Add(".", Token_Class.T_Dot);
            Operators.Add(";", Token_Class.T_Semicolon);
            Operators.Add(",", Token_Class.T_Comma);
            Operators.Add("(", Token_Class.T_LParanthesis);
            Operators.Add(")", Token_Class.T_RParanthesis);
            Operators.Add(":=", Token_Class.T_EqualOp);
            Operators.Add("=", Token_Class.T_CondtionEqual);
            Operators.Add("<", Token_Class.T_LessThanOp);
            Operators.Add(">", Token_Class.T_GreaterThanOp);
            Operators.Add("<>", Token_Class.T_NotEqualOp);
            Operators.Add("+", Token_Class.T_PlusOp);
            Operators.Add("-", Token_Class.T_MinusOp);
            Operators.Add("*", Token_Class.T_MultiplyOp);
            Operators.Add("/", Token_Class.T_DivideOp);
            Operators.Add("||", Token_Class.T_Or);
            Operators.Add("&&", Token_Class.T_And);
            Operators.Add("{", Token_Class.T_LCurly);
            Operators.Add("}", Token_Class.T_RCurly);
            Operators.Add("[", Token_Class.T_LSquare);
            Operators.Add("]", Token_Class.T_RSquare);
        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i + 1;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (SourceCode[i] == '/' &&
                    (i + 1) < SourceCode.Length &&
                    SourceCode[i + 1] == '*' &&
                    (i + 2) < SourceCode.Length &&
                    SourceCode[i + 2] != '/')
                {
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '*' &&
                            (j + 1) < SourceCode.Length &&
                            SourceCode[j + 1] == '/')
                        {
                            CurrentLexeme += SourceCode[j];
                            CurrentLexeme += SourceCode[j + 1];
                            j += 2;
                            break;
                        }

                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }

                if (CurrentChar == '"')
                {
                    while (j < SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                        if (SourceCode[j] == '"')
                        {
                            j++;
                            break;
                        }
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z')
                {
                    while (j < SourceCode.Length &&
                          ((SourceCode[j] >= 'A' && SourceCode[j] <= 'z') ||
                           (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }
                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    while (j < SourceCode.Length &&
                          ((SourceCode[j] >= '0' && SourceCode[j] <= '9') ||
                           SourceCode[j] == '.'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }
                else
                {
                    if (SourceCode[i] == '<' && SourceCode[j] == '>')
                    {
                        CurrentLexeme += SourceCode[j];
                        FindTokenClass(CurrentLexeme);
                        i = j;
                        continue;
                    }

                    if (SourceCode[i] == '&' && SourceCode[j] == '&')
                    {
                        CurrentLexeme += SourceCode[j];
                        FindTokenClass(CurrentLexeme);
                        i = j;
                        continue;
                    }

                    if (SourceCode[i] == '|' && SourceCode[j] == '|')
                    {
                        CurrentLexeme += SourceCode[j];
                        FindTokenClass(CurrentLexeme);
                        i = j;
                        continue;
                    }

                    if (SourceCode[i] == ':' && SourceCode[j] == '=')
                    {
                        CurrentLexeme += SourceCode[j];
                        FindTokenClass(CurrentLexeme);
                        i = j;
                        continue;
                    }

                    CurrentLexeme = CurrentChar.ToString();
                    FindTokenClass(CurrentLexeme);
                    continue;
                }
            }

            Tiny_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;

            if (isReserved(Lex))
            {
                if (ReservedWords.ContainsKey(Lex.ToUpper()))
                {
                    Tok.token_type = ReservedWords[Lex.ToUpper()];
                    Tokens.Add(Tok);
                    return;
                }
            }

            if (isString(Lex))
            {
                Tok.token_type = Token_Class.T_String;
                Tokens.Add(Tok);
                return;
            }

            if (isComment(Lex))
            {
                Tok.token_type = Token_Class.T_Comment;
                Tokens.Add(Tok);
                return;
            }

            if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.T_Identifier;
                Tokens.Add(Tok);
                return;
            }

            if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.T_Constant;
                Tokens.Add(Tok);
                return;
            }

            if (Operators.ContainsKey(Lex.ToUpper()))
            {
                Tok.token_type = Operators[Lex.ToUpper()];
                Tokens.Add(Tok);
                return;
            }
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }

        bool isIdentifier(string lex)
        {
            var exp = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$");
            return exp.IsMatch(lex);
        }

        bool isConstant(string lex)
        {
            var exp = new Regex(@"^[0-9]+(\.[0-9]+)?$");
            return exp.IsMatch(lex);
        }

        bool isComment(string lex)
        {
            return new Regex(@"^/\*.*\*/$", RegexOptions.Singleline).IsMatch(lex);
        }

        bool isString(string lex)
        {
            var exp = new Regex("^\"[^\"]*\"$");
            return exp.IsMatch(lex);
        }

        bool isReserved(string lex)
        {
            var exp = new Regex(@"\b(?:main|int|float|string|read|write|repeat|until|if|elseif|else|then|end|return|endl)\b");
            return exp.IsMatch(lex);
        }

        bool isOperator(string lex)
        {
            var exp = new Regex(@"(?:\+|-|\*|/|&&|\|\||<|>|=|<>)");
            return exp.IsMatch(lex);
        }
    }
}
