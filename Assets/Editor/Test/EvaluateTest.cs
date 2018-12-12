using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class EvaluateTest
{
    struct string_float
    {
        public string str;
        public float value;

        public string_float(string str, float value)
        {
            this.str = str;
            this.value = value;
        }
    }

    [Test]
    public void ReversePolishEval()
    {
        string_float[] testData = new string_float[]
        {
            new string_float("1 2 + 3 *", 9),       //(1 + 2) * 3 = 9
            new string_float("1 2 * 3 +", 5),       //1 * 2 + 3 = 5
            new string_float("1 2 / 3 -", -2.5f),   //1 / 2 - 3 = -2.5
            new string_float("1 3 - 2 /", -1),      //(1 - 3) / 2 = -1
        };

        foreach (string_float currentData in testData)
            Assert.AreEqual(currentData.value, Evaluate.SuffixEval(currentData.str));
    }

    [Test]
    public void Eval_Test()
    {
        string_float[] testData = new string_float[]
        {
            new string_float("1 + 2", 3),
            new string_float("2 * 3", 6),
            new string_float("( 1 + 2 ) * 3", 9),
            new string_float("1 + ( 3 / 2 ) * 7 + 3", 14.5f),
            new string_float("1 max 3", 3),
            new string_float("1 min 3", 1),
            new string_float("1 * 3 max 3 * 5 min 7 * 5", 15),
            new string_float("( 1 max 3 ) min ( 5 max 7 )", 3),
        };

        foreach (string_float currentData in testData)
            Assert.AreEqual(currentData.value, Evaluate.Eval(currentData.str));
    }
}
