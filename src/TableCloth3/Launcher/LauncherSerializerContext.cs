using System.Text.Json.Serialization;
using TableCloth3.Launcher.Models;

namespace TableCloth3.Launcher;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(LauncherSettingsModel))]
internal partial class LauncherSerializerContext : JsonSerializerContext { }
