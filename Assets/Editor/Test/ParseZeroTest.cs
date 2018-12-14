using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ParseZeroTest
{
    [Test]
    public void ParseZero()
    {
        Debug.Log("float.Parse(\"010\") = " + float.Parse("010"));

        Debug.Log(Regex.IsMatch("1", "^[-]?[0-9]+(\\.[0-9]+)?$"));

        //Debug.Log(new Stack<int>().Pop());    Steak不可取空，没有元素的时候取不出来
        //Debug.Log(new Stack<int>().Peek());   也读不出来，没有什么返回 null 之类的设计
    }
}
