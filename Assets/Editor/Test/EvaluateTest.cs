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
    public void Eval_Test()
    {
        string_float[] testData = new string_float[]
        {
            new string_float("1 + 2", 3),
            new string_float("2 * 3", 6),
            new string_float("( 1 + 2 ) * 3", 9),
            new string_float("1 + ( 3 / 2 ) * 7 + 3", 14.5f),
            new string_float("max(1 , 3)", 3),
            new string_float("min(1 , 3)", 1),
            new string_float("min(max(1 * 3 , 3 * 5) , 7 * 5)", 15),
            new string_float("min(max( 1 , 3 ) , max( 5 , 7 ))", 3),
            new string_float("  min(max( 1  , 3   ) , max( 5  ,    7 ))  ", 3),
        };

        foreach (string_float currentData in testData)
            Assert.AreEqual(currentData.value, Evaluate.Eval(currentData.str));
    }

    [Test]
    public void Eval_Normal_Test()
    {
        string_float[] testData = new string_float[]
        {
            new string_float("1 + 2", 3),
            new string_float("2 * 3", 6),
            new string_float("( 1 + 2 ) * 3", 9),
            new string_float("1 + ( 3 / 2 ) * 7 + 3", 14.5f),
            new string_float("max ( 1 , 3 )", 3),
            new string_float("min ( 1 , 3 )", 1),
            new string_float("min ( max (1 * 3,  3 * 5) , 7 * 5)", 15),
            new string_float("min(  MaX(1, 3 ) ,  MAX(5 , 7 ))", 3),
            new string_float("min ( max( 1 , 3) , max ( 5 , 7) )", 3),
        };

        foreach (string_float currentData in testData)
            Assert.AreEqual(currentData.value, Evaluate.Eval(currentData.str));
    }

    [Test]
    public void Eval_Space_Test()
    {
        string_float[] testData = new string_float[]
        {
            new string_float("1   +  2", 3),
            new string_float("2* 3", 6),
            new string_float("(1+2)*3", 9),
            new string_float("1 + ( 3 / 2 ) * 7 + 3", 14.5f),
            new string_float("max   (1   , 3)     ", 3),
            new string_float("min(1,3)", 1),
            new string_float("1   * 3 max 3 * 5 min 7 * 5", 15),
            new string_float("min  (max(1 * 3,  3 * 5) , 7 * 5)", 15),
            new string_float("min(max( 1 , 3)  ,max (5,7) )", 3),
            new string_float("1+2", 3),
            new string_float("1.568*2.598", 4.073664f),
            new string_float("1.568*-2.598", -4.073664f),
            new string_float("((-7.25+6.25)*(7.6/96.4)+1576.2)*3.5", 5516.42406639f),
        };

        foreach (string_float currentData in testData)
            Assert.AreEqual(currentData.value, Evaluate.Eval(currentData.str));
    }

    [Test]
    public void Eval_Upper_Test()
    {
        string_float[] testData = new string_float[]
        {
            new string_float("mAx(1,3)", 3),
            new string_float("miN(1, 3)", 1),
            new string_float("MIn(Max(1 * 3, 3 * 5) , 7 * 5)", 15),
            new string_float("min(  MaX(1, 3 ) ,  MAX(5 , 7 ))", 3),
        };

        foreach (string_float currentData in testData)
            Assert.AreEqual(currentData.value, Evaluate.Eval(currentData.str));
    }
}
