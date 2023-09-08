using System.Collections;

namespace NovelGame{
    public class MyCollection
    {
        public static object RandomChoice(IList col)
        {
            if(col.Count == 0){
                return null;
            }
            System.Random r1 = new System.Random();
            int temp = r1.Next(0,col.Count);//0~col.Count-1までの数を生成する。
            return col[temp];
        }
    }
}