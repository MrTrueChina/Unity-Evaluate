using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Evaluate
{
    const string slowest = "Slowest";
    /// <summary>
    /// 运算符优先级，按C语言运算符优先级的100倍写入，越小越优先
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



    public static float Eval(string notation)
    {
        return SuffixEval(InfixToSuffix(SeparateNotation(notation.ToLower())));
    }


    /// <summary>
    /// 传入一个算式，在数字和括号左右两边增加空格以分隔各元素
    /// </summary>
    /// <param name="notation"></param>
    /// <returns></returns>
    static string SeparateNotation(string notation)
    {
        string separatedNotation = notation;
        
        separatedNotation = Regex.Replace(separatedNotation, "[-]?[0-9]+(\\.[0-9]+)?", " $0 ");   //数字

        separatedNotation = Regex.Replace(separatedNotation, "[(]", " ( ");    //左括号

        separatedNotation = Regex.Replace(separatedNotation, "[)]", " ) ");    //右括号

        return separatedNotation;

        /*
        "[-]?[0-9]+(\\.[0-9]+)?", " $0 " 解释：

        [-]             ->      匹配"-"
        ?               ->      前一个元素匹配零或一次
        [-]?            ->      有一个或没有"-"，用来匹配正负

        [0-9]           ->      匹配 0-9 的数字
        +               ->      前一个元素匹配至少一次
        [0-9]+          ->      匹配 0-9 的数字至少一次，用来匹配整数位

        ()              ->      打组，将多个匹配达成一个组，可以整组加限定，同时可以在替换时读取组
        \\.             ->      匹配".",从我们能看到的文本编辑器到编译后的字符串是一次转义，字符串到正则是第二次转义，所以要加两个"\"
        (\\.[0-9]+)     ->      匹配小数点和至少一位数字，用来匹配小数部分
        (\\.[0-9]+)?    ->      有小数部分或没有小数部分

        [-]?[0-9]+(\\.[0-9]+)?  ->  可以有也可以没有负号、有整数位、可以有也可以没有小数位  ->  正负小数或整数

        $0  ->  第0组，即整个正则表达式表达式，$0在替换时代表了整个正则表达式匹配到的字符串，对这个正则表达式而言就是匹配到的数字
         */
    }


    /// <summary>
    /// 传入以不少于一个空格为分隔的算式，包括括号也要用空格分隔开
    /// </summary>
    /// <param name="notation"></param>
    /// <returns></returns>
    static string[] InfixToSuffix(string notation)
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

        List<string> suffixNotation = new List<string>();

        foreach (string str in afterSplitNotation)
        {
            if (IsNumber(str))
            {
                suffixNotation.Add(str);
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

                        suffixNotation.Add(current);
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
                            suffixNotation.Add(stack.Pop());

                        stack.Push(str);
                    }
                }
            }
        }

        while (stack.Peek() != slowest)
            suffixNotation.Add(stack.Pop());    //把缓存栈里剩余的除最慢符之外的运算符都拿出来存进后缀表达式里

        return suffixNotation.ToArray();
    }

    static string[] splitNotation(string notation)
    {
        notation = notation.Trim();             //string.Trim()：去除字符串前后的空格
        return Regex.Split(notation, "\\s+");   //以空格为分隔，把每个部分分割开形成新的字符串
        /*        
        Regex.Split(string input, string pattern)：将 input 字符串在 pattern 正则匹配到的位置裁剪开，产生多个字符串

        \s      ->      空格的转义字符
        \\s     ->      两次转义的空格
        \\s+    ->      匹配一个或多个连续的空格

        */
    }

    static bool IsNumber(string str)
    {
        return Regex.IsMatch(str, "^[-]?[0-9]+(\\.[0-9]+)?$");  //正则判断是否是数字
        /*
        Regex.IsMatch(string input, string pattern)：检测输入的字符串是否匹配正则表达式

        ^[-]?[0-9]+(\\.[0-9]+)?$    解释：

        中间部分前面说了

        ^      ->    匹配必须从字符串或一行的开头开始
        $      ->    匹配必须出现在字符串或一行的结尾的 \n 之前
        
        ^[-]?[0-9]+(\\.[0-9]+)?$  ->  从字符串开头开始，到字符串结尾位置为止，是一个正或负的整数或小数

        */
    }

    
    /// <summary>
    /// 传入一个后缀表达式字符串数组
    /// </summary>
    /// <param name="notation"></param>
    /// <returns></returns>
    public static float SuffixEval(string[] notation)
    {
        Stack<float> stack = new Stack<float>();

        foreach (string str in notation)
        {
            if (IsNumber(str))
            {
                stack.Push(float.Parse(str));                   //是数字的话压入栈
            }
            else
            {
                float b = stack.Pop();                          //取出栈顶元素，这是第二个运算值

                float a = stack.Pop();                          //再次取出栈顶元素，这次是第一个运算值

                stack.Push(Calculate(a, b, str));
            }
        }

        return stack.Pop();
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
