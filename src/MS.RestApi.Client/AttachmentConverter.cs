using System;
using System.Collections.Generic;
using MS.RestApi.Abstractions;
using Newtonsoft.Json;

namespace MS.RestApi.Client;

public class AttachmentConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => writer.WriteNull();
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => null;
    public override bool CanConvert(Type objectType) => typeof(IAttachment).IsAssignableFrom(objectType) || typeof(IEnumerable<IAttachment>).IsAssignableFrom(objectType);
}