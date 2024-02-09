using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ArrayHelper
{
    /// <summary>
    /// Add a element to the array
    /// </summary>
    public static T[] Add<T>(T p_value, T[] p_array)
    {
        ArrayList t_tempList = new ArrayList();

        foreach (T t_item in p_array) t_tempList.Add(t_item);
        t_tempList.Add(p_value);

        return t_tempList.ToArray(typeof(T)) as T[];
    }

    /// <summary>
    /// Remove a element according to the given index from the array
    /// </summary>
    public static T[] Remove<T>(int p_index, T[] p_array)
    {
        ArrayList t_tempList = new ArrayList();

        foreach (T t_item in p_array) t_tempList.Add(t_item);
        t_tempList.RemoveAt(p_index);

        return t_tempList.ToArray(typeof(T)) as T[];
    }

    /// <summary>
    /// Convert an array to a list.
    /// </summary>
    public static List<T> ArrayToList<T>(T[] p_array)
    {
        return p_array.ToList();
    }

    /// <summary>
    /// Convert a list to an array.
    /// </summary>
    public static T[] ListToArray<T>(List<T> p_list)
    {
        return p_list.ToArray();
    }
}
