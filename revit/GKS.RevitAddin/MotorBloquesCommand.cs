using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorBloques.Models;
using MotorBloques.Motor;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class MotorBloquesCommand : IExternalCommand
{
    // ---------------- Helpers ---------------------------------------------

    // Factor de conversión: 1 mm = 0.00328084 pies (aprox).
    private const double MM_TO_FEET_FACTOR = 0.00328084; 

    private static double MmToFeet(double mm)
    {
        // Conversión directa a pies, necesaria porque Revit usa pies como unidad interna.
        return mm * MM_TO_FEET_FACTOR;
    }

    /// <summary>
    /// Dibuja el esquema de un bloque como cuatro líneas de detalle en la vista activa.
    /// </summary>
    private static void CreateRectangleDetailCurves(Document doc, View view, XYZ origin, double widthFeet, double heightFeet)
    {
        XYZ p0 = origin;
        XYZ p1 = new XYZ(origin.X + widthFeet, origin.Y, origin.Z);
        XYZ p2 = new XYZ(origin.X + widthFeet, origin.Y + heightFeet, origin.Z);
        XYZ p3 = new XYZ(origin.X, origin.Y + heightFeet, origin.Z);

        var edges = new List<Curve>
        {
            Line.CreateBound(p0, p1),
            Line.CreateBound(p1, p2),
            Line.CreateBound(p2, p3),
            Line.CreateBound(p3, p0)
        };

        foreach (var c in edges)
            doc.Create.NewDetailCurve(view, c);
    }
    
    // ----------------------------------------------------------------------


    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIDocument uiDoc = commandData.Application.ActiveUIDocument;
        Document doc = uiDoc?.Document;
        if (doc == null)
        {
            message = "No hay documento activo.";
            return Result.Failed;
        }

        // 1) CARGA CONFIG Y VALIDACIÓN DE UNIDADES --------------------------
        Configuracion config;
        string configPath = null;

        try
        {
            // Busca config.json en la misma carpeta que la DLL del Add-in
            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            configPath = Path.Combine(asmDir ?? string.Empty, "config.json");

            if (!File.Exists(configPath))
            {
                TaskDialog.Show("GKS • MotorBloques",
                    $"No se encontró el archivo de configuración.\nRuta:\n{configPath}");
                return Result.Failed;
            }

            string jsonString = File.ReadAllText(configPath);
            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            config = JsonSerializer.Deserialize<Configuracion>(jsonString, jsonOpts);
            if (config == null)
            {
                TaskDialog.Show("GKS • MotorBloques", "No se pudo deserializar la configuración.");
                return Result.Failed;
            }

            // Validación de Unidad de Entrada y Alerta
            if (string.IsNullOrWhiteSpace(config.UnidadEntrada))
            {
                TaskDialog.Show("GKS • MotorBloques", "UnidadEntrada no definida en config.json.");
                return Result.Failed;
            }

            if (!config.UnidadEntrada.Equals("mm", StringComparison.InvariantCultureIgnoreCase))
            {
                TaskDialog.Show("GKS • MotorBloques",
                    $"Unidad de entrada: '{config.UnidadEntrada}'. Se convertirá a mm antes del cálculo.");
            }
        }
        catch (JsonException jx)
        {
            TaskDialog.Show("GKS • MotorBloques", $"Error JSON en {configPath}:\n{jx.Message}");
            return Result.Failed;
        }
        catch (IOException iox)
        {
            TaskDialog.Show("GKS • MotorBloques", $"Error de E/S al leer {configPath}:\n{iox.Message}");
            return Result.Failed;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("GKS • MotorBloques", $"Fallo al cargar configuración:\n{ex.Message}");
            return Result.Failed;
        }

        // 2) EJECUCIÓN DEL MOTOR GKS -----------------------------------------
        List<Bloque> bloques;
        try
        {
            var motor = new MotorBloques.Motor.MotorBloques(config);
            bloques = motor.Run() ?? new List<Bloque>();
        }
        catch (Exception ex)
        {
            TaskDialog.Show("GKS • MotorBloques", $"Fallo en el motor de disposición:\n{ex.Message}");
            return Result.Failed;
        }

        if (!bloques.Any())
        {
            TaskDialog.Show("GKS • MotorBloques",
                "El motor no generó bloques. Revise área disponible y tamaños mínimos en la configuración.");
            return Result.Cancelled;
        }

        // 3) DIBUJO EN REVIT (DetailCurves) -----------------------------------
        View activeView = doc.ActiveView;
        if (activeView == null)
        {
            TaskDialog.Show("GKS • MotorBloques", "No hay vista activa para dibujar.");
            return Result.Failed;
        }

        int creados = 0;
        XYZ firstOrigin = XYZ.Zero; // Inicializamos el punto de origen (0, 0, 0)
        bool firstBlock = true;
        Transaction tx = null; 

        try 
        {
            // Iniciar la transacción de Revit
            tx = new Transaction(doc, "GKS • Generar Bloques");
            tx.Start();

            foreach (var b in bloques)
            {
                // Conversión del resultado (ya en MM) a la unidad interna de Revit (PIES)
                double x = MmToFeet(b.X);
                double y = MmToFeet(b.Y);
                double w = MmToFeet(b.Ancho);
                double h = MmToFeet(b.Alto);

                // Dibuja el esquema del bloque usando líneas de detalle
                CreateRectangleDetailCurves(doc, activeView, new XYZ(x, y, 0), w, h);

                // Capturar el origen del primer bloque para el mensaje resumen
                if (firstBlock) 
                {
                    firstOrigin = new XYZ(x, y, 0);
                    firstBlock = false;
                }
                creados++;
            }

            // Confirmar los cambios
            tx.Commit();
        }
        catch (Exception ex)
        {
            // Deshacer la transacción si hubo un error de dibujo
            if (tx != null && tx.HasStarted())
                tx.RollBack();

            TaskDialog.Show("GKS • MotorBloques", $"Error al crear geometría:\n{ex.Message}");
            return Result.Failed;
        }
        finally
        {
            // Asegurar la liberación de recursos
            if (tx != null)
                tx.Dispose();
        }

        // Resumen
        TaskDialog.Show("GKS • MotorBloques",
            $"Éxito: {creados} bloque(s) dibujado(s) como líneas de detalle en '{activeView.Name}'.\n" +
            $"Primer origen (pies): X={firstOrigin.X:F3}, Y={firstOrigin.Y:F3}");

        return Result.Succeeded;
    }
}