using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum Token_Class
{
    T_Int, T_Float, T_String, T_Read, T_Write, T_Repeat, T_Until, T_If, T_Elseif, T_Else, T_Then, T_Return, T_Endl, T_End, // Reserved Keyword , Assigned By Dictionary
    T_Plus, T_Minus, T_Multiply, T_Divide, T_Assignment, // Arithmatic Operators , Assigned By Dictionary
    T_LessThan, T_GreaterThan, T_IsEqual, T_NotEqual, // Conditional Operators , Assigned By Dictionary
    T_And, T_Or, // Boolean Operators , Assigned By Dictionary
    T_Number, T_Identifier, // Basic Terms , Assigned By functions
    T_Comma, T_SemiColon, T_LeftParenthesis, T_RightParenthesis, T_LeftBrackets, T_RightBrackets, // Syntax , Assigned By Dictionary

    //FunctionCall,
    //CommentStatement, FunctionName, Parameter, FunctionDeclaration, FunctionBody, FunctionStatement, MainFunction, //function Statements
    //Program,
    //Term,	Equation, Expression, Assignment_Statement, DataType, DeclarationStatement,
    //Write_Statement, ReadStatement, ReturnStatement, Condition, ConditionStatement, 
    //IfStatement, ElseIfStatement, ElseStatement,
    //RepeatStatement
}
namespace TINY_Compiler
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
        Dictionary<string, Token_Class> Syntax = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.T_Int);
            ReservedWords.Add("float", Token_Class.T_Float);
            ReservedWords.Add("string", Token_Class.T_String);
            ReservedWords.Add("read", Token_Class.T_Read);
            ReservedWords.Add("write", Token_Class.T_Write);
            ReservedWords.Add("repeat", Token_Class.T_Repeat);
            ReservedWords.Add("until", Token_Class.T_Until);
            ReservedWords.Add("if", Token_Class.T_If);
            ReservedWords.Add("elseif", Token_Class.T_Elseif);
            ReservedWords.Add("else", Token_Class.T_Else);
            ReservedWords.Add("then", Token_Class.T_Then);
            ReservedWords.Add("return", Token_Class.T_Return);
            ReservedWords.Add("endl", Token_Class.T_Endl);
            ReservedWords.Add("end", Token_Class.T_End);

            Operators.Add("+", Token_Class.T_Plus);
            Operators.Add("-", Token_Class.T_Minus);
            Operators.Add("*", Token_Class.T_Multiply);
            Operators.Add("/", Token_Class.T_Divide);
            Operators.Add(":=", Token_Class.T_Assignment);
            Operators.Add("<", Token_Class.T_LessThan);
            Operators.Add(">", Token_Class.T_GreaterThan);
            Operators.Add("=", Token_Class.T_IsEqual);
            Operators.Add("<>", Token_Class.T_NotEqual);
            Operators.Add("&&", Token_Class.T_And);
            Operators.Add("||", Token_Class.T_Or);

            Syntax.Add(",", Token_Class.T_Comma);
            Syntax.Add(";", Token_Class.T_SemiColon);
            Syntax.Add("{", Token_Class.T_LeftParenthesis);
            Syntax.Add("}", Token_Class.T_RightParenthesis);
            Syntax.Add("(", Token_Class.T_LeftBrackets);
            Syntax.Add(")", Token_Class.T_RightBrackets);

        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                // --------------------------------------------- CHARACTER -------------------------------------------------

                if ( (CurrentChar >= 'A' && CurrentChar <= 'Z') || (CurrentChar >= 'a' && CurrentChar <= 'z') ) 
                {
                   j++;
                   while(j < SourceCode.Length && ( char.IsLetterOrDigit(SourceCode[j]) ) )
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                   FindTokenClass(CurrentLexeme);
                   i = j - 1; //since the for loop increments one after the incremention we would be at i = j
                    continue;
                }

                // --------------------------------------------- NUMBER --------------------------------------------------

                else if( CurrentChar >= '0' && CurrentChar <= '9' ) 
                {
                    j++;
                    bool haveDot = false;
                    bool doubleDots = false;
                    while ( j < SourceCode.Length && (char.IsLetterOrDigit(SourceCode[j]) || SourceCode[j] == '.') )
                    {
                        if (SourceCode[j] == '.' && haveDot == false)
                        {
                            haveDot = true;
                        }
                        else if (SourceCode[j] == '.' && haveDot == true) //handle if two dots appear in the same number
                        {
                            doubleDots = true;
                        }
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (doubleDots)
                            Errors.Error_List.Add(CurrentLexeme);
                    else
                        FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }

                // --------------------------------------------- COMMENT --------------------------------------------------

                else if ( CurrentChar == '/' && SourceCode[i+1] == '*')
                {
                    j += 2;
                    while( j < SourceCode.Length && ! (SourceCode[j] == '*' && j+1 < SourceCode.Length && SourceCode[j+1] == '/') )
                    {
                        j++;
                    }
                    i = j + 1; //This will put us after the '/' since the j is at the '*'
                    continue;
                }

                // --------------------------------------------- Two Character Operator ------------------------------------

                else if( i+1 < SourceCode.Length && (Operators.ContainsKey(CurrentChar.ToString() + SourceCode[i + 1] ) ) )
                {
                    string s = CurrentChar.ToString() + SourceCode[i + 1];
                    FindTokenClass(s);
                    i++; // since we increment the i one more time after the loop this is like i+=2
                    continue;
                }

                // --------------------------------------------- One Character Operator ------------------------------------

                else if (Operators.ContainsKey(CurrentChar.ToString() ) )
                {
                    FindTokenClass(CurrentChar.ToString());
                    continue;
                }

                // --------------------------------------------- String ----------------------------------------------------

                else if (CurrentChar == '"')
                {
                    if (j+1 <= SourceCode.Length)
                        j++;
                    string s = "\"";
                    bool closed = false;

                    while(j < SourceCode.Length)
                    {
                        s += SourceCode[j];
                        if (SourceCode[j] == '"')
                        {
                            closed = true;
                            j++;
                            break;
                        }
                        j++;
                    }
                  
                    if (!closed)
                    {
                        Errors.Error_List.Add(s);
                        i = j - 1;
                        continue;
                    }

                    FindTokenClass(s);
                    i = j - 1;
                    continue;
                }
                // not any with other case
                else
                {
                    FindTokenClass(CurrentChar.ToString()); 
                }
            }
            
            Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC = new Token_Class();
            Token Tok = new Token();
            Tok.lex = Lex;


            //Is it a reserved word?
            if ( ReservedWords.ContainsKey(Lex) )
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if ( isIdentifier(Lex) )
            {
                Tok.token_type = Token_Class.T_Identifier;
                Tokens.Add(Tok);
            }
            //Is it a Constant?
            else if ( isConstant(Lex) )
            {
                Tok.token_type = Token_Class.T_Number;
                Tokens.Add(Tok);
            }
            //Does it start with a digit but is not a constant?
            else if (char.IsDigit(Lex[0]))
            {
                Errors.Error_List.Add(Lex);
            }
            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it a syntax xharacter?
            else if (Syntax.ContainsKey(Lex))
            {
                Tok.token_type = Syntax[Lex];
                Tokens.Add(Tok);
            }
            //is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.T_String;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }

            
        }

    

        bool isIdentifier(string lex)
        {
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(lex,@"^[a-zA-Z][a-zA-Z0-9]*$");
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(lex, @"^[0-9]+(\.[0-9]+)?$"); 
            return isValid;
        }
        bool isString(string lex)
        {
            bool isValid = System.Text.RegularExpressions.Regex.IsMatch(lex, @"^"".*""$");
            return isValid;
        }
    }
}
