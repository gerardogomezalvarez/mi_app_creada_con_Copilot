# Gestor de tareas

Aplicación de consola en un único fichero (`app.cs`) para gestionar una lista de tareas guardada en un JSON local.

## Instalación

1. Instala el [SDK de .NET 10](https://dotnet.microsoft.com/download) o superior.
2. Comprueba la versión:

   ```
   dotnet --version
   ```

3. Descarga `app.cs`. No hace falta proyecto (`.csproj`) ni restaurar paquetes.

## Uso

```
dotnet run app.cs -- <comando> [argumentos]
```

El `--` es obligatorio: separa las opciones de `dotnet` de los argumentos de la aplicación.

Las tareas se guardan en `tareas.json`, en el **directorio desde el que ejecutas** la aplicación. Si no existe, se crea al añadir la primera tarea.

| Comando | Alias | Descripción |
|---|---|---|
| `add <texto>` | `añadir`, `anadir` | Añade una tarea |
| `list [estado]` | `listar` | Lista las tareas |
| `done <id>` | `completar` | Marca una tarea como completada |
| `delete <id>` | `borrar` | Elimina una tarea |

Sin comando, o con uno desconocido, se muestra la ayuda.

## Ejemplos

### Añadir

```
$ dotnet run app.cs -- add Comprar pan
Tarea 1 añadida.
```

No hacen falta comillas, pero puedes usarlas: `add "Llamar al médico"`.

### Listar

```
$ dotnet run app.cs -- list
[ ] 1: Comprar pan
[ ] 2: Llamar al médico
```

`[ ]` es una tarea pendiente y `[x]` una completada.

Filtrar por estado:

| Estado | Alias | Muestra |
|---|---|---|
| `all` | `todas` | Todas (por defecto) |
| `pending` | `pendientes` | Solo pendientes |
| `done` | `completadas` | Solo completadas |

```
$ dotnet run app.cs -- list pending
[ ] 2: Llamar al médico
```

### Completar

```
$ dotnet run app.cs -- done 1
Tarea 1 completada.
```

### Borrar

```
$ dotnet run app.cs -- delete 2
Tarea 2 eliminada.
```

## Errores habituales

| Mensaje | Causa |
|---|---|
| `no existe la tarea N` | El id no está en la lista; revisa con `list` |
| `estado no válido (all\|pending\|done)` | Estado incorrecto en `list` |
| `'tareas.json' no contiene un JSON válido` | El fichero está dañado; corrígelo o bórralo |

Los errores devuelven código de salida `1`.
