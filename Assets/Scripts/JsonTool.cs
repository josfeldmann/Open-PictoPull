using Newtonsoft;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public static class JsonTool {


    public static string ObjectToString<T>(T t) {
        // return JsonUtility.ToJson(t);
       
        return Newtonsoft.Json.JsonConvert.SerializeObject(t, Formatting.None, new JsonSerializerSettings() {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }

    public static T StringToObject<T>(string json) {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }

}


public class CustomConverter : JsonConverter {
    private readonly Type[] _types;

    public CustomConverter(params Type[] types) {
        _types = types;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        Debug.LogError("Here");
        
        serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
        JToken t = JToken.FromObject(value);
        serializer.Serialize(writer, value);
        if (t.Type != JTokenType.Object) {
            //t.WriteTo(writer);
        } else {
            JObject o = (JObject)t;
            //IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

            //o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));
            
            //o.WriteTo(writer);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanRead {
        get { return false; }
    }

    public override bool CanConvert(Type objectType) {
        return _types.Any(t => t == objectType);
    }
}

