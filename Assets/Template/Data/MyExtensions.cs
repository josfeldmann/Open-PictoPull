using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public static partial class MyExtensions {
    public static T PickRandomNotNull<T>(this T[] arr) {
        List<T> res = new List<T>();

        foreach (T t in arr) {
            if (t != null) res.Add(t);
        }

        return res.PickRandom();
    }

    private static System.Random rnd = new System.Random();

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
        return source.OrderBy<T, int>((item) => rnd.Next());
    }


    public static T PickRandom<T>(this List<T> list) {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static int GetCount<T>(this Dictionary<T, int> dict, T t) {
        if (dict.ContainsKey(t)) {
            return dict[t];
        } else {
            return 0;
        }
    }

    public static void AddI<T>(this Dictionary<T, int> dict, T t, int amt) {
        if (dict.ContainsKey(t)) {
            dict[t] += amt;
        } else {
            dict[t] = amt;
        }
    }

 

    public static bool ContainsAny<T>(this List<T> list, List<T> other) {

        foreach (T t in list) {
            if (other.Contains(t)) return true;
        }

        return false;
    }


    public static void AddValue<T>(this Dictionary<T, int> dictionary, T key, int amount) {
        int count;
        dictionary.TryGetValue(key, out count);
        dictionary[key] = count + amount;
    }


    public static string FirstCharToUpper(this string input) =>
      input switch
      {
          null => throw new ArgumentNullException(nameof(input)),
          "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
          _ => input.First().ToString().ToUpper() + input.Substring(1)
      };
    public static void SortNavs<T>(this List<T> navs) where T : INavSortable {

        for (int i = 0; i < navs.Count; i++) {

            NavigationObject nav = navs[i].GetNav();
            if (i == 0) {
                nav.up = navs[navs.Count - 1].GetNav();
                nav.down = navs[i + 1].GetNav();
            } else if (i == navs.Count - 1) {
                nav.up = navs[i - 1].GetNav();
                nav.down = navs[0].GetNav();
            } else {
                nav.up = navs[i - 1].GetNav();
                nav.down = navs[i + 1].GetNav();
            }

        }

    }

    public static void EnsureImageAspectRatio(this Image i) {
        i.EnsureImageAspectRatio(i.sprite, Mathf.Max(i.rectTransform.rect.height, i.rectTransform.rect.width));
    }


    public static void EnsureImageAspectRatio(this Image i, Sprite sprite, float maxSize) {
        float height = 0;
        float width = 0;

        if (sprite.rect.width > sprite.rect.height) {
            width = maxSize;
            height = (sprite.rect.height / sprite.rect.width) * maxSize;
        } else {
            width = (sprite.rect.width / sprite.rect.height) * maxSize;
            height = maxSize;
        }
        i.rectTransform.sizeDelta = new Vector2(width, height);

    }

}


