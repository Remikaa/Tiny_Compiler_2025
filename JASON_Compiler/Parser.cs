using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        //Node Program()
        //{
        //    Node program = new Node("Program");
        //    program.Children.Add(Header());
        //    program.Children.Add(DeclSec());
        //    program.Children.Add(Block());
        //    //program.Children.Add(match(Token_Class.Dot));
        //    MessageBox.Show("Success");
        //    return program;
        //}

        //Node Header()
        //{
        //    Node header = new Node("Header");
        //    // write your code here to check the header sructure
        //    return header;
        //}
        //Node DeclSec()
        //{
        //    Node declsec = new Node("DeclSec");
        //    // write your code here to check atleast the declare sturcure 
        //    // without adding procedures
        //    return declsec;
        //}
        //Node Block()
        //{
        //    Node block = new Node("block");
        //    // write your code here to match statements
        //    return block;
        //}

        //// Implement your logic here

        public Node Function_Call()
        {
            Node node = new Node("Function_Call");
            match(Token_Class.T_Identifier);
            match(Token_Class.T_LeftBrackets);
            var _arg = Argument();
            if (_arg != null) node.Children.Add(_arg);
            match(Token_Class.T_RightBrackets);

            return node;
        }
        public Node Argument()
        {
            Node node = new Node("Argument");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                match(Token_Class.T_Identifier);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                var _more = MoreArge();
                if (_more != null) node.Children.Add(_more);
            }
            return node;
        }
        public Node MoreArge()
        {
            Node node = new Node("MoreArge");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                match(Token_Class.T_Comma);
                var _arg = Argument();
                if (_arg != null) node.Children.Add(_arg);
                var _more = MoreArge();
                if (_more != null) node.Children.Add(_more);
            }
            return node;
        }
        public Node Term()
        {
            Node node = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Number)
                match(Token_Class.T_Number);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.T_LeftParenthesis)
            {
                var _fc = Function_Call();
                if (_fc != null) node.Children.Add(_fc);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
                match(Token_Class.T_Identifier);
            return node;
        }
        public Node Equation()
        {
            Node node = new Node("Equation");
            var _f = Factor();
            if (_f != null) node.Children.Add(_f);
            if (TokenStream[InputPointer].token_type == Token_Class.T_Plus || TokenStream[InputPointer].token_type == Token_Class.T_Minus)
            {
                var _add = AddOp();
                if (_add != null) node.Children.Add(_add);
                var _f2 = Factor();
                if (_f2 != null) node.Children.Add(_f2);
            }
            return node;
        }
        public Node AddOp()
        {
            Node node = new Node("AddOp");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Plus)
                match(Token_Class.T_Plus);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Minus)
                match(Token_Class.T_Minus);
            return node;
        }
        public Node MulEQ()
        {
            Node node = new Node("MulEQ");
            var _eq = Equation();
            if (_eq != null) node.Children.Add(_eq);
            node.Children.Add(MulOp());
            var _rec = MulEQ();
            if (_rec != null) node.Children.Add(_rec);
            return node;
        }
        public Node MulOp()
        {
            Node node = new Node("MulOp");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Multiply)
                match(Token_Class.T_Multiply);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Divide)
                match(Token_Class.T_Divide);

            return node;
        }
        public Node Factor()
        {
            Node node = new Node("Factor");
            if (TokenStream[InputPointer].token_type == Token_Class.T_LeftBrackets)
            {
                match(Token_Class.T_LeftBrackets);
                var _eq = Equation();
                if (_eq != null) node.Children.Add(_eq);
                match(Token_Class.T_RightBrackets);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                var _t = Term();
                if (_t != null) node.Children.Add(_t);
            }
            return node;
        }
        public Node Expression()
        {
            Node node = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Number || TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                var _t = Term();
                if (_t != null) node.Children.Add(_t);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_String)
                match(Token_Class.T_String);
            else
            {
                var _eq = Equation();
                if (_eq != null) node.Children.Add(_eq);
            }
            return node;
        }
        public Node Assignment_Statement()
        {
            Node node = new Node("Assignment_Statement");
            match(Token_Class.T_Identifier);
            match(Token_Class.T_Assignment);
            var _e = Expression();
            if (_e != null) node.Children.Add(_e);
            return node;
        }
        public Node Datatype()
        {
            Node node = new Node("Datatype");
            if (TokenStream[InputPointer].token_type == Token_Class.T_String)
                match(Token_Class.T_String);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Int)
                match(Token_Class.T_Int);
            else match(Token_Class.T_Float);

            return node;
        }
        public Node Declaration_Statement()
        {
            Node node = new Node("Declaration_Statement");
            var _dt = Datatype();
            if (_dt != null) node.Children.Add(_dt);
            var _dec = Declaration();
            if (_dec != null) node.Children.Add(_dec);
            match(Token_Class.T_SemiColon);
            return node;
        }
        public Node Declaration()
        {
            Node node = new Node("Declaration");
            var _terms = Terms();
            if (_terms != null) node.Children.Add(_terms);
            if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                match(Token_Class.T_Comma);
                var _dec = Declaration();
                if (_dec != null) node.Children.Add(_dec);
            }
            return node;
        }
        public Node Terms()
        {
            Node node = new Node("Terms");
            if (TokenStream[InputPointer + 1].token_type == Token_Class.T_Assignment)
            {
                var _as = Assignment_Statement();
                if (_as != null) node.Children.Add(_as);
            }
            match(Token_Class.T_Identifier);
            return node;
        }
        public Node Write_Statement()
        {
            Node node = new Node("Write_Statement");
            match(Token_Class.T_Write);
            var _e = Expression();
            if (_e != null) node.Children.Add(_e);
            var _end = End_Statement();
            if (_end != null) node.Children.Add(_end);
            match(Token_Class.T_SemiColon);
            return node;
        }
        public Node End_Statement()
        {
            Node node = new Node("End_Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Endl)
                match(Token_Class.T_Endl);

            return node;
        }
        public Node Read_Statement()
        {
            Node node = new Node("Read_Statement");
            match(Token_Class.T_Read);
            match(Token_Class.T_Identifier);
            match(Token_Class.T_SemiColon);
            return node;
        }
        public Node Return_Statement()
        {
            Node node = new Node("Return_Statement");
            match(Token_Class.T_Return);
            var _e = Expression();
            if (_e != null) node.Children.Add(_e);
            match(Token_Class.T_SemiColon);
            return node;
        }
        public Node Condition()
        {
            Node node = new Node("Condition");
            match(Token_Class.T_Identifier);
            if (TokenStream[InputPointer].token_type == Token_Class.T_LessThan)
                match(Token_Class.T_LessThan);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_GreaterThan)
                match(Token_Class.T_GreaterThan);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_IsEqual)
                match(Token_Class.T_IsEqual);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_NotEqual)
                match(Token_Class.T_NotEqual);
            var _t = Term();
            if (_t != null) node.Children.Add(_t);
            return node;
        }
        public Node Condition_Statement()
        {
            Node node = new Node("Condition_Statement");
            var _c = Condition();
            if (_c != null) node.Children.Add(_c);
            var _rep = Condition_Repeat();
            if (_rep != null) node.Children.Add(_rep);
            return node;
        }
        public Node Condition_Repeat()
        {
            Node node = new Node("Condition_Repeat");
            if (TokenStream[InputPointer].token_type == Token_Class.T_And || TokenStream[InputPointer].token_type == Token_Class.T_Or)
            {
                var _b = Boolean_Operator();
                if (_b != null) node.Children.Add(_b);
                var _c = Condition();
                if (_c != null) node.Children.Add(_c);
                var _r = Condition_Repeat();
                if (_r != null) node.Children.Add(_r);
            }
            return node;
        }
        public Node Boolean_Operator()
        {
            Node node = new Node("Boolean_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.T_And)
                match(Token_Class.T_And);
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Or)
                match(Token_Class.T_Or);
            return node;
        }
        public Node If_Statement()
        {
            Node node = new Node("If_Statement");
            match(Token_Class.T_If);
            var _cs = Condition_Statement();
            if (_cs != null) node.Children.Add(_cs);
            match(Token_Class.T_Then);
            var _sr = Statements_Repeat();
            if (_sr != null) node.Children.Add(_sr);
            var _end = If_Statement_Ending();
            if (_end != null) node.Children.Add(_end);
            return node;
        }
        public Node Statements_Repeat()
        {
            Node node = new Node("Statements_Repeat");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Int || TokenStream[InputPointer].token_type == Token_Class.T_Float || TokenStream[InputPointer].token_type == Token_Class.T_String
                || TokenStream[InputPointer].token_type == Token_Class.T_Identifier || TokenStream[InputPointer].token_type == Token_Class.T_Write || TokenStream[InputPointer].token_type == Token_Class.T_Read
                || TokenStream[InputPointer].token_type == Token_Class.T_If || TokenStream[InputPointer].token_type == Token_Class.T_Repeat
                )
            {
                var _s = Statements();
                if (_s != null) node.Children.Add(_s);
                var _sr = Statements_Repeat();
                if (_sr != null) node.Children.Add(_sr);
            }
            return node;
        }
        public Node If_Statement_Ending()
        {
            Node node = new Node("If_Statement_Ending");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Elseif)
            {
                var _elif = Else_If_Statement();
                if (_elif != null) node.Children.Add(_elif);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Else)
            {
                var _else = Else_Statement();
                if (_else != null) node.Children.Add(_else);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_End)
                match(Token_Class.T_End);

            return node;
        }
        public Node Statements()
        {
            Node node = new Node("Statements");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Int || TokenStream[InputPointer].token_type == Token_Class.T_Float || TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                var _d = Declaration_Statement();
                if (_d != null) node.Children.Add(_d);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier && TokenStream[InputPointer + 1].token_type == Token_Class.T_LeftParenthesis)
            {
                var _fc = Function_Call();
                if (_fc != null) node.Children.Add(_fc);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                var _as = Assignment_Statement();
                if (_as != null) node.Children.Add(_as);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Write)
            {
                var _w = Write_Statement();
                if (_w != null) node.Children.Add(_w);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Read)
            {
                var _r = Read_Statement();
                if (_r != null) node.Children.Add(_r);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_If)
            {
                var _if = If_Statement();
                if (_if != null) node.Children.Add(_if);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Repeat)
            {
                var _rep = Repeat_Statement();
                if (_rep != null) node.Children.Add(_rep);
            }
            return node;
        }
        public Node Else_If_Statement()
        {
            Node node = new Node("Else_If_Statement");
            match(Token_Class.T_Elseif);
            var _c = Condition_Statement();
            if (_c != null) node.Children.Add(_c);
            match(Token_Class.T_Then);
            var _sr = Statements_Repeat();
            if (_sr != null) node.Children.Add(_sr);
            var _end = If_Statement_Ending();
            if (_end != null) node.Children.Add(_end);
            return node;
        }
        public Node Else_Statement()
        {
            Node node = new Node("Else_Statement");
            match(Token_Class.T_Else);
            var _sr = Statements_Repeat();
            if (_sr != null) node.Children.Add(_sr);
            match(Token_Class.T_End);
            return node;
        }
        public Node Repeat_Statement()
        {
            Node node = new Node("Repeat_Statement");
            match(Token_Class.T_Repeat);
            var _sr = Statements_Repeat();
            if (_sr != null) node.Children.Add(_sr);
            match(Token_Class.T_Until);
            node.Children.Add(Condition_Statement());
            return node;
        }
        public Node Function_Name()
        {
            Node node = new Node("Function_Name");
            match(Token_Class.T_Identifier);
            return node;
        }
        public Node Parameter()
        {
            Node node = new Node("Parameter");
            var _d = Datatype();
            if (_d != null) node.Children.Add(_d);
            match(Token_Class.T_Identifier);
            return node;
        }
        public Node Function_Declaration()
        {
            Node node = new Node("Function_Declaration");
            var _dt = Datatype();
            if (_dt != null) node.Children.Add(_dt);
            var _fn = Function_Name();
            if (_fn != null) node.Children.Add(_fn);
            match(Token_Class.T_LeftParenthesis);
            var _pr = Parameter_Repeat();
            if (_pr != null) node.Children.Add(_pr);
            match(Token_Class.T_RightParenthesis);
            return node;
        }
        public Node Parameter_Repeat()
        {
            Node node = new Node("Parameter_Repeat");
            if (TokenStream[InputPointer + 1].token_type == Token_Class.T_Identifier)
            {
                var _p = Parameter();
                if (_p != null) node.Children.Add(_p);
                var _pc = Parameter_Cont();
                if (_pc != null) node.Children.Add(_pc);
            }
            return node;
        }
        public Node Parameter_Cont()
        {
            Node node = new Node("Parameter_Cont");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                match(Token_Class.T_Comma);
                var _p = Parameter();
                if (_p != null) node.Children.Add(_p);
                var _pc = Parameter_Cont();
                if (_pc != null) node.Children.Add(_pc);
            }
            return node;
        }
        public Node Function_Body()
        {
            Node node = new Node("Function_Body");
            match(Token_Class.T_LeftParenthesis);
            var _sr = Statements_Repeat();
            if (_sr != null) node.Children.Add(_sr);
            var _ret = Return_Statement();
            if (_ret != null) node.Children.Add(_ret);
            match(Token_Class.T_RightParenthesis);
            return node;
        }
        public Node Function_Statement()
        {
            Node node = new Node("Function_Statement");
            var _decl = Function_Declaration();
            if (_decl != null) node.Children.Add(_decl);
            var _body = Function_Body();
            if (_body != null) node.Children.Add(_body);
            return node;
        }
        public Node Main_Function()
        {
            Node node = new Node("Main_Function");
            var _d = Datatype();
            if (_d != null) node.Children.Add(_d);
            match(Token_Class.T_main);
            match(Token_Class.T_LeftBrackets);
            match(Token_Class.T_RightBrackets);
            var _body = Function_Body();
            if (_body != null) node.Children.Add(_body);
            return node;
        }
        public Node Program()
        {
            Node node = new Node("Program");
            var _fsr = Function_Statement_Repeat();
            if (_fsr != null) node.Children.Add(_fsr);
            var _main = Main_Function();
            if (_main != null) node.Children.Add(_main);
            return node;
        }
        public Node Function_Statement_Repeat()
        {
            Node node = new Node("Function_Statement_Repeat");
            if (TokenStream[InputPointer + 1].token_type == Token_Class.T_Identifier)
            {
                var _fs = Function_Statement();
                if (_fs != null) node.Children.Add(_fs);
                var _rec = Function_Statement_Repeat();
                if (_rec != null) node.Children.Add(_rec);
            }
            return node;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
