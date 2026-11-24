#:property PublishAot=false
#:property JsonSerializerIsReflectionEnabledByDefault=true

using System.Text.Json;

const string Archivo = "tareas.json";
var opciones = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

if (args.Length == 0) return Ayuda();

var comando = args[0].ToLowerInvariant();
var resto = args[1..];

try
{
    switch (comando)
    {
        case "add" or "añadir" or "anadir":
            return Añadir(string.Join(' ', resto).Trim());
        case "list" or "listar":
            return Listar(resto);
        case "done" or "completar":
            return Completar(resto);
        case "delete" or "borrar":
            return Borrar(resto);
        default:
            return Ayuda();
    }
}
catch (JsonException)
{
    Console.Error.WriteLine($"Error: '{Archivo}' no contiene un JSON válido.");
    return 1;
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"Error de E/S: {ex.Message}");
    return 1;
}

int Ayuda()
{
    Console.WriteLine("""
        Uso: dotnet run app.cs -- <comando>
          add <texto>   (añadir)    Añade una tarea
          list [estado] (listar)    Lista las tareas; estado: all|pending|done
                                    (también todas|pendientes|completadas)
          done <id>     (completar) Marca una tarea como completada
          delete <id>   (borrar)    Elimina una tarea
        """);
    return 1;
}

List<TodoItem> Cargar()
{
    if (!File.Exists(Archivo)) return [];
    var json = File.ReadAllText(Archivo);
    if (string.IsNullOrWhiteSpace(json)) return [];
    return JsonSerializer.Deserialize<List<TodoItem>>(json, opciones) ?? [];
}

void Guardar(List<TodoItem> tareas) =>
    File.WriteAllText(Archivo, JsonSerializer.Serialize(tareas, opciones));

int Añadir(string titulo)
{
    if (titulo.Length == 0)
    {
        Console.Error.WriteLine("Error: indica el texto de la tarea.");
        return 1;
    }
    var tareas = Cargar();
    var id = tareas.Count == 0 ? 1 : tareas.Max(t => t.Id) + 1;
    tareas.Add(new TodoItem(id, titulo, false, DateTime.Now));
    Guardar(tareas);
    Console.WriteLine($"Tarea {id} añadida.");
    return 0;
}

int Listar(string[] a)
{
    bool? hechas = (a.Length == 0 ? "all" : a[0].ToLowerInvariant()) switch
    {
        "all" or "todas" => null,
        "pending" or "pendientes" => false,
        "done" or "completadas" => true,
        _ => throw new ArgumentException("estado no válido (all|pending|done)")
    };
    var tareas = Cargar().Where(t => hechas is null || t.Done == hechas).ToList();
    if (tareas.Count == 0)
    {
        Console.WriteLine("No hay tareas.");
        return 0;
    }
    foreach (var t in tareas)
        Console.WriteLine($"[{(t.Done ? 'x' : ' ')}] {t.Id}: {t.Title}");
    return 0;
}

int Completar(string[] a)
{
    if (a.Length != 1 || !int.TryParse(a[0], out var id))
    {
        Console.Error.WriteLine("Error: uso 'done <id>' con un id numérico.");
        return 1;
    }
    var tareas = Cargar();
    var i = tareas.FindIndex(t => t.Id == id);
    if (i < 0)
    {
        Console.Error.WriteLine($"Error: no existe la tarea {id}.");
        return 1;
    }
    tareas[i] = tareas[i] with { Done = true };
    Guardar(tareas);
    Console.WriteLine($"Tarea {id} completada.");
    return 0;
}

int Borrar(string[] a)
{
    if (a.Length != 1 || !int.TryParse(a[0], out var id))
    {
        Console.Error.WriteLine("Error: uso 'delete <id>' con un id numérico.");
        return 1;
    }
    var tareas = Cargar();
    if (tareas.RemoveAll(t => t.Id == id) == 0)
    {
        Console.Error.WriteLine($"Error: no existe la tarea {id}.");
        return 1;
    }
    Guardar(tareas);
    Console.WriteLine($"Tarea {id} eliminada.");
    return 0;
}

record TodoItem(int Id, string Title, bool Done, DateTime CreatedAt);
