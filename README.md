# 🧱 MotorBloques – Generador de Disposición de Bloques

Proyecto desarrollado para René y Esteban.  
Este motor coloca bloques dentro de un área definida, respetando juntas, tolerancias y tamaños, con salida visual y archivo de configuración.

---

## 🗂 Estructura

MotorBloques/
├── MotorBloques/ # Código fuente principal
│ ├── Models/ # Modelos de datos (Bloque, Configuracion, Resultado)
│ ├── Motor/ # Lógica del motor y utilidades
│ ├── ConsoleDemo/ # Demo de consola (ejecución rápida)
│ ├── config/ # Archivo de configuración principal
│ └── revit/ # Esqueleto de add-in para integración Revit
│
├── web/ # Visualizador HTML de la salida
└── README.md

---

## ⚙️ Instalación

1. **Requisitos:**
   - .NET 6.0 o superior instalado.
   - (Opcional) Revit 2023+ si se va a usar el add-in.

2. **Clonar o descomprimir:**
unzip MotorBloques.zip
cd MotorBloques/MotorBloques/ConsoleDemo


3. **Ejecutar la demo:**
dotnet run


Salida esperada:
- `output/output.json` → coordenadas de bloques colocados.  
- `output/output.svg` → vista esquemática.

4. **Visualizar en navegador:**
- Abre `web/preview.html` en cualquier navegador.
- Carga el archivo `output/output.svg` para ver el resultado gráfico.

---

## ⚒️ Archivos Clave

| Archivo | Descripción |
|----------|--------------|
| `config/config.json` | Configura área, tamaños, juntas y tolerancias. |
| `Motor/MotorBloques.cs` | Núcleo del algoritmo (greedy + backtracking). |
| `ConsoleDemo/Program.cs` | Ejecuta el motor con los parámetros del JSON. |
| `web/preview.html` | Interfaz simple para visualizar la disposición. |
| `revit/AddinSkeleton.cs` | Punto de inicio para conectar con Revit. |

---

## 🧠 Lógica del Motor

1. Carga `config.json`.
2. Calcula cuántos bloques pueden colocarse según tamaño y juntas.
3. Usa heurística *greedy* para llenar el área.
4. Si hay huecos, aplica *backtracking limitado*.
5. Exporta resultados a JSON y SVG.

---

## 📋 Ejemplo de `config.json`

```json
{
"anchoArea": 1000,
"altoArea": 500,
"anchoBloque": 50,
"altoBloque": 20,
"junta": 2,
"tolerancia": 1
}
🧩 Integración Revit
En revit/AddinSkeleton.cs encontrarás un ejemplo básico de cómo:

Cargar el motor desde un add-in.

Llamar a MotorBloques.Run(config).

Exportar los puntos generados a una familia o dibujo dentro de Revit.

📦 Salidas
Archivo	Tipo	Uso
output.json	Datos numéricos de posiciones.	Para depuración o análisis.
output.svg	Visualización 2D.	Se carga en preview.html.

📞 Contacto
Desarrollado por Esteban y René (prototipo técnico).
Para pruebas o ajustes del algoritmo, contactar con René para integración Revit.

🧾 Licencia
Uso académico y de desarrollo interno. No distribuir sin autorización.