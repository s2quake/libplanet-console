// using System.Text.Json;
// using System.Text.Json.Serialization;
// using JSSoft.Communication;

// namespace LibplanetConsole.Common.Services;

// internal sealed class CommunicationSerializer : ISerializer
// {
//     private static readonly Dictionary<Type, JsonConverter> _converterByType =
//     [
//         typeof(BoundPeer)
//     ]

//     private static readonly JsonSerializerOptions Options = new()
//     {
//         IncludeFields = true,
//         Converters =
//         {
//             // new ArgumentExceptionConverter(),
//             // new ArgumentNullExceptionConverter(),
//             // new ArgumentOutOfRangeExceptionConverter(),
//             // new ExceptionConverter(),
//             // new IndexOutOfRangeExceptionConverter(),
//             // new InvalidOperationExceptionConverter(),
//             // new ObjectDisposedExceptionConverter(),
//             // new NotSupportedExceptionConverter(),
//             // new NullReferenceExceptionConverter(),
//             // new SystemExceptionConverter(),
//             // new ExceptionConverterFactory(),
//         },
//     };

//     private readonly ISerializer _serializer;

//     public CommunicationSerializer(ISerializer serializer)
//     {
//         _serializer = serializer;
//     }

//     public object? Deserialize(Type type, string text)
//     {
//         if (Options.Converters.con)
//     }

//     public string Serialize(Type type, object? data)
//     {
//         throw new NotImplementedException();
//     }
// }
