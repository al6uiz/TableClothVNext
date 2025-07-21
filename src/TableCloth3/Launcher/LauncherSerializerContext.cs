using System.Text.Json.Serialization;
using TableCloth3.Launcher.Models;

namespace TableCloth3.Launcher;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(LauncherSettingsModel))]
[JsonSerializable(typeof(FolderSettingsModel))]
internal partial class LauncherSerializerContext : JsonSerializerContext { }
