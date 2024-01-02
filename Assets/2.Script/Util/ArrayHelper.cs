using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ArrayHelper
{
    public static T[] Add<T>(T p_value, T[] p_array)
    {
        ArrayList t_tempList = new ArrayList();

        foreach (T t_item in p_array) t_tempList.Add(t_item);
        t_tempList.Add(p_value);

        return t_tempList.ToArray(typeof(T)) as T[];
    }

    public static T[] Remove<T>(int p_index, T[] p_array)
    {
        ArrayList t_tempList = new ArrayList();

        foreach (T t_item in p_array) t_tempList.Add(t_item);
        t_tempList.RemoveAt(p_index);

        return t_tempList.ToArray(typeof(T)) as T[];
    }

    public static List<T> ArrayToList<T>(T[] p_array)
    {
        return p_array.ToList();
    }

    public static T[] ListToArray<T>(List<T> p_list)
    {
        return p_list.ToArray();
    }
}
