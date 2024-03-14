using GameScript;
using UnityEditor;
using UnityEngine;

public class Menus : MonoBehaviour
{
    [MenuItem("GameScript/Show Settings")]
    static void ShowSettings() => Selection.activeObject = Settings.Instance;

    // [MenuItem("GameScript/Test")]
    // static void Test()
    // {
    //     BinaryFormatter serializer = new();
    //     using (FileStream fs = new(@"C:\Users\emful\Desktop\DATABASE\TMP\test.dat", FileMode.Create))
    //     {
    //         Tester o1 = new();
    //         Tester o2 = new();
    //         o1.link = o2;
    //         o1.name = "o1";
    //         o2.link = o1;
    //         o2.name = "o2";
    //         serializer.Serialize(fs, o1);
    //     }
    //     using (FileStream fs = new(@"C:\Users\emful\Desktop\DATABASE\TMP\test.dat", FileMode.Open))
    //     {
    //         Tester o = (Tester)serializer.Deserialize(fs);
    //         Debug.Log(o.name);
    //         Debug.Log(o.link.name);
    //         Debug.Log(o.link.link.name);
    //         Debug.Log(o.link.link.link.name);
    //     }

    // }

    // [Serializable]
    // public class Tester
    // {
    //     public string name;
    //     public Tester link;
    //     public override string ToString()
    //     {
    //         return $"{name}";
    //     }
    // }
}
