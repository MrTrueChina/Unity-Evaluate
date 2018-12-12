using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Evaluate
{
    const string slowest = "Slowest";
    /// <summary>
    /// 运算符优先级，按C语言运算符优先级的100倍写入，越小越快
    /// </summary>
    static readonly Dictionary<string, int> _operatorPrecedence = new Dictionary<string, int>()
    {
        { "(", int.MaxValue - 1 },      //算法上左括号特殊处理，处理方式表现为比其他运算符都要慢
                                        //右括号同样特殊处理，作为闭合括号的标志，本身不会存入栈，不需要设置优先级
        { "*", 300 },
        { "/", 300 },

        { "max", 350},
        { "min", 350},

        { "+", 400 },
        { "-", 400 },

        { slowest, int.MaxValue }     //最慢符，用于填充栈底
    };

    static Stack<float> _stack = new Stack<float>();


    public static float Eval(string notation)
    {
        Debug.Log(InfixToSuffix(notation));
        return SuffixEval(InfixToSuffix(notation));
    }

    /// <summary>
    /// 传入以空格为分隔的算式，包括括号也要用空格分隔开
    /// </summary>
    /// <param name="notation"></param>
    /// <returns></returns>
    static string InfixToSuffix(string notation)
    {
        /*
        百度给出的算法：https://baike.baidu.com/item/%E5%90%8E%E7%BC%80%E8%A1%A8%E8%BE%BE%E5%BC%8F

        计算机实现转换：
        将中缀表达式转换为后缀表达式的算法思想：

        ·开始扫描；
        ·数字时，加入后缀表达式；
        ·运算符：
            a. 若为 '('，入栈；
            b. 若为 ')'，则依次把栈中的的运算符加入后缀表达式中，直到出现'('，从栈中删除'(' ；
            c. 若为 除括号外的其他运算符， 当其优先级高于除'('以外的栈顶运算符时，直接入栈。否则从栈顶开始，依次弹出比当前处理的运算符优先级高和优先级相等的运算符，直到一个比它优先级低的或者遇到了一个左括号为止，然后将其自身压入栈中（先出后入）。
        ·当扫描的中缀表达式结束时，栈中的的所有运算符出栈；
         */

        string[] afterSplitNotation = splitNotation(notation);

        Stack<string> stack = new Stack<string>();
        stack.Push(slowest);                        //先把最慢符压入栈底，有最慢符兜底后续运算不需要考虑空栈情况
        string suffixNotation = "";

        foreach (string str in afterSplitNotation)
        {
            if (IsNumber(str))
            {
                suffixNotation += " " + str;
            }
            else
            {
                if (str == "(")
                {
                    stack.Push(str);
                }
                else if (str == ")")
                {
                    while (true)
                    {
                        string current = stack.Pop();

                        if (current == "(")
                            break;

                        suffixNotation += " " + current;
                    }
                }
                else
                {
                    if (_operatorPrecedence[str] < _operatorPrecedence[stack.Peek()])    //当前运算符的优先级高于栈顶运算符，左括号的优先级设置为仅次于最慢符
                    {
                        stack.Push(str);
                    }
                    else
                    {
                        while (_operatorPrecedence[stack.Peek()] <= _operatorPrecedence[str])
                            suffixNotation += " " + stack.Pop();

                        stack.Push(str);
                    }
                }
            }
        }

        while (stack.Peek() != slowest)
            suffixNotation += " " + stack.Pop();    //把缓存栈里剩余的除最慢符之外的运算符都拿出来存进后缀表达式里

        return suffixNotation.Substring(1, suffixNotation.Length - 1);  //开头有个空格要删掉
    }

    static string[] splitNotation(string notation)
    {
        return Regex.Split(notation, "\\s+", RegexOptions.IgnoreCase);  //以空格为分隔，把每个部分分割开形成新的字符串
    }

    static bool IsNumber(string str)
    {
        return Regex.IsMatch(str, "^[-]?[0-9]+(\\.[0-9]+)?$");          //正则判断是否是数字
    }


    /// <summary>
    /// 传入以空格为分隔的后缀表达式，如 1 2 + 3 *
    /// </summary>
    /// <param name="notation"></param>
    /// <returns></returns>
    public static float SuffixEval(string notation)
    {
        _stack.Clear();

        string[] afterSplitNotation = splitNotation(notation);
        
        foreach (string str in afterSplitNotation)
        {
            if (IsNumber(str))
            {
                _stack.Push(float.Parse(str));                  //是数字的话压入栈
            }
            else
            {
                float b = _stack.Pop();                         //取出栈顶元素，这是第二个运算值

                float a = _stack.Pop();                         //再次取出栈顶元素，这次是第一个运算值

                _stack.Push(Calculate(a, b, str));
            }
        }

        return _stack.Pop();
    }

    static float Calculate(float a, float b, string operatorString)
    {
        switch (operatorString)
        {
            case "+":
                return a + b;

            case "-":
                return a - b;

            case "*":
                return a * b;

            case "/":
                return a / b;

            case "max":
                return Mathf.Max(a, b);

            case "min":
                return Mathf.Min(a, b);

            default:
                return 0;
        }
    }
}
