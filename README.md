# 🧱 MotorBloques GKS

> **Motor de Disposición Automática de Bloques Estructurales para Integración BIM**

[![.NET Version](https://img.shields.io/badge/.NET-8.0+-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Proprietary-red?style=flat)]()
[![Status](https://img.shields.io/badge/Status-Prototype-orange?style=flat)]()

---

## 📋 Tabla de Contenidos

- [Descripción General](#-descripción-general)
- [Características Principales](#-características-principales)
- [Arquitectura del Sistema](#-arquitectura-del-sistema)
- [Requisitos](#-requisitos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Guía de Uso](#-guía-de-uso)
- [Configuración Técnica](#-configuración-técnica)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Flujos de Ejecución](#-flujos-de-ejecución)
- [Integración con Revit](#-integración-con-revit)
- [Desarrollo y Contribución](#-desarrollo-y-contribución)
- [Equipo](#-equipo)

---

## 🎯 Descripción General

**MotorBloques** es un prototipo técnico desarrollado para **GKS** que automatiza la disposición óptima de bloques estructurales en entornos BIM. El sistema implementa un algoritmo híbrido (Greedy + No-Corte) programado en **C# (.NET 8.0)** que garantiza:

- ✅ **Traba estructural** mediante el sistema Mata-Junta
- ✅ **Cumplimiento normativo** con las reglas de construcción GKS
- ✅ **Optimización de cortes** minimizando desperdicios
- ✅ **Salida BIM-Ready** para integración directa con Revit

El motor procesa configuraciones de muros y genera coordenadas precisas de cada bloque, listas para ser consumidas por la API de Revit y crear geometría paramétrica en el modelo 3D.

---

## ✨ Características Principales

### Núcleo Algorítmico

| Característica | Tecnología | Descripción |
|:---|:---|:---|
| **Lógica No-Corte** | C# (.NET) | Algoritmo que minimiza cortes de bloques respetando tolerancias definidas |
| **Sistema Mata-Junta** | Offset Calculation | Desplazamiento alternado entre filas para garantizar traba estructural |
| **Manejo de Unidades** | Auto-Conversion | Conversión automática entre m, cm y mm con validación y alerta |
| **Optimización Greedy** | Hybrid Algorithm | Selección inteligente de bloques para maximizar eficiencia |

### Salidas y Visualización

| Componente | Formato | Propósito |
|:---|:---|:---|
| **Salida BIM** | JSON | Coordenadas (X, Y) y dimensiones en milímetros para Revit API |
| **Demo Interactiva** | HTML/JS/SVG | Validación visual sin recompilar código |
| **Consola Debug** | Terminal | Tabla de coordenadas para verificación inmediata |
| **Exportación Flexible** | JSON | El framework soporta exportación de datos estructurados para análisis externo |

### Integraciones

- 🔧 **Revit API Ready**: Esqueleto de add-in preparado para conexión directa con el núcleo C#
- 📊 **Salida Estructurada**: JSON con datos completos de cada bloque
- 🎨 **Visualización Web**: Demo interactiva para pruebas rápidas

---

## 🏗 Arquitectura del Sistema
```
┌─────────────────────────────────────────────────────────────┐
│                     MOTORBLOQUES SYSTEM                     │
└─────────────────────────────────────────────────────────────┘
                              │
              ┌───────────────┴───────────────┐
              │                               │
         📁 CONFIG                       🖥️ ENTORNOS
     (config.json)                            │
              │                   ┌───────────┴──────────┐
              │                   │                      │
              │              🔷 C# CORE            🌐 WEB DEMO
              │              (.NET 8.0)           (HTML/JS)
              │                   │                      │
              └──────────┬────────┘                      │
                         │                               │
                    ⚙️ MOTOR                        📊 VISUALIZER
                 (MotorBloques.cs)               (Visualizador.js)
                         │                               │
                    📐 ALGORITMO                    🎨 SVG RENDER
                 (No-Corte + Offset)                     │
                         │                               │
                         └──────────┬────────────────────┘
                                    │
                              📤 OUTPUT
                         ┌──────────┴──────────┐
                         │                     │
                   📄 JSON FILE          🏢 REVIT API
                (output.json)         (Future Integration)
```

---

## 🔧 Requisitos

### Software Requerido

- **.NET SDK 8.0 o superior** - [Descargar](https://dotnet.microsoft.com/download)
- **Navegador Web Moderno** (Chrome, Firefox, Edge) para la demo interactiva
- **(Opcional) Autodesk Revit 2023+** para integración BIM completa

### Conocimientos Recomendados

- C# básico para modificaciones del motor
- JSON para configuración de parámetros
- Conceptos BIM para integración con Revit

---

## 📥 Instalación y Configuración

### 1. Clonar el Repositorio
```bash
git clone https://github.com/RetaCode/MotorBloques_GKS.Latam
cd MotorBloques
```

### 2. Configurar Parámetros Iniciales

Edita el archivo `MotorBloques/config/config.json` con tus especificaciones:
```json
{
  "AnchoArea": 1000,
  "AltoArea": 500,
  "Junta": 2,
  "Tolerancia": 1,
  "TamanosBloque": [300, 400, 500],
  "OffsetMataJunta": 150,
  "AltoBloque": 98,
  "UnidadEntrada": "mm",
  "GrosorParedFinal": 150
}
```

### 3. Compilar el Proyecto
```bash
cd MotorBloques/MotorBloques
dotnet build
```

---

## 🚀 Guía de Uso

### Opción A: Ejecutar Demo en Consola (C#)

**Propósito**: Validar la lógica del motor y generar salida JSON para Revit.
```bash
cd MotorBloques/MotorBloques/ConsoleDemo
dotnet run
```

⚠️ **Alerta de Unidad**: Si la `UnidadEntrada` es diferente de `"mm"`, el programa mostrará una advertencia, pero procederá con la conversión y el cálculo en milímetros.

**Salidas Generadas:**

| Archivo | Ubicación | Contenido |
|:---|:---|:---|
| Terminal | Console Output | Tabla resumen con primeras coordenadas |
| `output.json` | `ConsoleDemo/output/` | **Datos completos para Revit API** |

**Ejemplo de Salida en Terminal:**
```
╔═══════════════════════════════════════════════════════╗
║         MOTORBLOQUES GKS - RESULTADOS                ║
╚═══════════════════════════════════════════════════════╝

📐 Configuración del Muro:
   - Dimensiones: 1000mm x 500mm
   - Bloques disponibles: 300, 400, 500 mm
   - Junta: 2mm | Offset: 150mm

🧱 Bloques Generados: 12

┌──────┬────────┬────────┬────────┬────────┐
│  #   │   X    │   Y    │ Ancho  │  Tipo  │
├──────┼────────┼────────┼────────┼────────┤
│  1   │    0   │    0   │  500   │  500   │
│  2   │  502   │    0   │  400   │  400   │
│  3   │  904   │    0   │   96   │  96    │
└──────┴────────┴────────┴────────┴────────┘

✅ Archivo generado: output/output.json
```

### Opción B: Demo Interactiva Web

**Propósito**: Probar diferentes configuraciones visualmente sin recompilar.

1. Abre `web/index.html` en tu navegador
2. Ajusta los parámetros en el panel de control:
   - Ancho y Alto del muro
   - Tamaños de bloques disponibles
   - Grosor de junta
   - Offset de Mata-Junta
3. Clic en **"GENERAR MURO"**
4. Observa la disposición en el visor SVG

**Características de la Demo:**

- ✅ Visualización en tiempo real
- ✅ Ajuste de parámetros sin código
- ✅ Validación visual de Mata-Junta
- ✅ Verificación de tolerancias

---

## ⚙️ Configuración Técnica

### Archivo `config.json`

Todos los parámetros del motor se controlan desde este archivo central.
```json
{
  "AnchoArea": 1000,
  "AltoArea": 500,
  "TamanosBloque": [300, 400, 500],
  "Junta": 2,
  "OffsetMataJunta": 150,
  "Tolerancia": 1,
  "AltoBloque": 98,
  "UnidadEntrada": "mm",
  "GrosorParedFinal": 150
}
```

### Descripción de Parámetros

| Parámetro | Tipo | Descripción | Valor Típico |
|:---|:---|:---|:---|
| `AnchoArea` | `int` | Dimensión horizontal total del muro | 1000 - 10000 mm |
| `AltoArea` | `int` | Dimensión vertical total del muro | 500 - 5000 mm |
| `TamanosBloque` | `int[]` | Lista de tamaños de bloques disponibles | [300, 400, 500] |
| `Junta` | `int` | Espacio entre bloques y filas | 1 - 5 mm |
| `OffsetMataJunta` | `int` | Desplazamiento horizontal para traba | 100 - 250 mm |
| `Tolerancia` | `int` | Error máximo permitido en fin de fila | 0 - 2 mm |
| `AltoBloque` | `int` | Altura del bloque estándar | 95 - 100 mm |
| `UnidadEntrada` | `string` | Unidad de medida de entrada | "mm", "cm", "m" |
| `GrosorParedFinal` | `int` | Espesor final del muro ensamblado | 120 - 200 mm |

### Reglas Estructurales GKS

El motor implementa las siguientes normativas:

| Regla | Implementación |
|:---|:---|
| **No-Corte** | Algoritmo optimizado para ajustar el final de la fila dentro de la `Tolerancia` |
| **Mata-Junta (Offset)** | Desplazamiento horizontal de filas alternas (`OffsetMataJunta`) |
| **Unidades** | Conversión automática de `UnidadEntrada` a milímetros en el constructor de `MotorBloques.cs` |

#### Regla No-Corte

Busca combinaciones de bloques que minimicen el desperdicio al final de cada fila.
```
Espacio Restante ≤ Tolerancia → ✅ Fila Válida
Espacio Restante > Tolerancia → ⚠️ Optimizar con Greedy
```

#### Regla Mata-Junta (Offset)

Desplaza horizontalmente las filas alternas para garantizar la traba estructural.
```
Fila Par (0, 2, 4...):   X_inicio = 0
Fila Impar (1, 3, 5...): X_inicio = OffsetMataJunta
```

#### Manejo de Unidades

**Unidad Interna Estándar**: Milímetros (mm)
```csharp
// Conversión automática al cargar configuración
switch (UnidadEntrada) {
    case "m":  factor = 1000; break;
    case "cm": factor = 10; break;
    case "mm": factor = 1; break;
}
```

⚠️ **Importante**: Todas las salidas están siempre en milímetros, independientemente de la unidad de entrada.

### Estructura de Salida JSON

El motor C# devuelve una lista de objetos, todos en **MILÍMETROS**.
```json
[
  {
    "X": 0,
    "Y": 0,
    "Ancho": 500,
    "Alto": 98,
    "Tipo": "500" 
  },
  {
    "X": 502,
    "Y": 0,
    "Ancho": 400,
    "Alto": 98,
    "Tipo": "400"
  }
]
```

---

## 📁 Estructura del Proyecto
```
MotorBloques/
│
├── MotorBloques/              # 🔷 Solución Principal C# (.NET)
│   ├── Models/                # Clases de datos (Bloque, Configuracion)
│   │   ├── Bloque.cs         # Definición del bloque estructural
│   │   └── Configuracion.cs  # Modelo de configuración
│   │
│   ├── Motor/                 # ⚙️ Núcleo del Algoritmo
│   │   └── MotorBloques.cs   # Lógica principal (No-Corte + Mata-Junta)
│   │
│   ├── ConsoleDemo/           # 🖥️ Demo de Consola
│   │   ├── Program.cs        # Punto de entrada
│   │   └── output/           # Carpeta de salida JSON
│   │
│   ├── config/                # ⚙️ Configuración Global
│   │   └── config.json       # Parámetros del motor
│   │
│   └── revit/                 # 🏢 Add-in para Revit (Futuro)
│       ├── RevitAddin.cs     # Esqueleto del add-in
│       └── manifest.addin    # Manifiesto de Revit
│
├── web/                       # 🌐 Demo Web Interactiva
│   ├── index.html            # Interfaz principal
│   ├── css/
│   │   └── styles.css        # Estilos de la aplicación
│   └── js/
│       ├── MotorBloques.js   # Motor en JavaScript
│       ├── Visualizador.js   # Renderizado SVG
│       └── config.js         # Configuración web
│
├── docs/                      # 📚 Documentación Técnica
│   └── TECHNICAL.md          # Documentación detallada
│
├── README.md                  # Este archivo
└── LICENSE                    # Licencia del proyecto
```

---

## 🔄 Flujos de Ejecución

### Flujo 1: Producción BIM (C#)
```
┌─────────────┐
│ config.json │
└──────┬──────┘
       │ 1. Carga de configuración
       ↓
┌─────────────────────┐
│ MotorBloques.cs     │
│ - ConvertToInternal │  2. Conversión de unidades
│ - ValidateConfig    │  3. Validación de parámetros
└──────┬──────────────┘
       │ 4. Ejecución algoritmo
       ↓
┌─────────────────────┐
│ Run() Method        │
│ - No-Corte Logic    │  5. Cálculo de disposición
│ - Mata-Junta Offset │  6. Aplicación de traba
└──────┬──────────────┘
       │ 7. Generación de lista
       ↓
┌─────────────────────┐
│ List<Bloque>        │  8. Salida en memoria
└──────┬──────────────┘
       │
       ├──→ 📄 output.json      (Development)
       └──→ 🏢 Revit API        (Production)
```

### Flujo 2: Visualización Web (JavaScript)
```
┌──────────────┐
│ index.html   │
│ (User Input) │
└──────┬───────┘
       │ 1. Captura de parámetros
       ↓
┌────────────────────┐
│ Visualizador.js    │  2. Recopilación de datos
└──────┬─────────────┘
       │ 3. Instanciación del motor
       ↓
┌────────────────────┐
│ MotorBloques.js    │  4. Ejecución de algoritmo
│ (JS Port)          │  5. Generación de coordenadas
└──────┬─────────────┘
       │ 6. Retorno de array
       ↓
┌────────────────────┐
│ Visualizador.js    │  7. Renderizado SVG
│ (Render Layer)     │  8. Dibujo de bloques
└──────┬─────────────┘
       │ 9. Actualización de vista
       ↓
┌────────────────────┐
│ SVG Canvas         │  10. Visualización final
└────────────────────┘
```

---

## 🏢 Integración con Revit

### Estado Actual: Prototipo Ready

El proyecto incluye un esqueleto de add-in preparado para la integración final:
```
revit/
├── RevitAddin.cs      # Clase principal del add-in
├── manifest.addin     # Descriptor de Revit
└── README.md          # Guía de integración
```

### Próximos Pasos

1. **Instalación del Add-in**:
   - Compilar el proyecto como biblioteca .NET
   - Copiar el .dll a la carpeta de add-ins de Revit
   - Registrar el manifest.addin

2. **Consumo de Datos**:
```csharp
   // El add-in consumirá directamente la lista de bloques
   var motor = new MotorBloques(config);
   List<Bloque> bloques = motor.Run();
   
   // Crear geometría en Revit
   foreach (var bloque in bloques) {
       CrearBloqueEnRevit(bloque.X, bloque.Y, bloque.Ancho, bloque.Alto);
   }
```

3. **Validación**:
   - Verificar coordenadas en modelo 3D
   - Confirmar traba estructural
   - Validar dimensiones y tolerancias

### Estructura de Datos para Revit

Cada bloque incluye toda la información necesaria:
```csharp
public class Bloque {
    public double X { get; set; }        // Coordenada X (mm)
    public double Y { get; set; }        // Coordenada Y (mm)
    public double Ancho { get; set; }    // Ancho del bloque (mm)
    public double Alto { get; set; }     // Alto del bloque (mm)
    public string Tipo { get; set; }     // Identificador de tamaño
}
```

---

## 💻 Desarrollo y Contribución

### Requisitos para Desarrollo

- **.NET SDK 8.0+**
- **IDE recomendado**: Visual Studio 2022 / VS Code + C# Extension
- **Git** para control de versiones

### Compilación desde Código Fuente
```bash
# Clonar repositorio
git clone https://github.com/RetaCode/MotorBloques_GKS.Latam
cd MotorBloques

# Restaurar dependencias
cd MotorBloques/MotorBloques
dotnet restore

# Compilar
dotnet build --configuration Release

# Ejecutar tests (si existen)
dotnet test
```

### Modificar el Motor

El archivo principal es `Motor/MotorBloques.cs`:
```csharp
// Ejemplo: Cambiar el algoritmo de selección
private int SeleccionarBloque(int espacioDisponible) {
    // Tu lógica personalizada aquí
    return tamaño;
}
```

---

## 👥 Equipo

### Desarrolladores Principales

<table>
  <tr>
    <td align="center">
      <strong>Esteban Retana</strong><br>
    </td>
    <td align="center">
      <strong>René Soto</strong><br>
    </td>
  </tr>
</table>

### Áreas de Responsabilidad

| Área | Descripción |
|:---|:---|
| **💻 Algoritmo Core** | Desarrollo, optimización y mantenimiento del motor de cálculo |
| **🏗️ Integración BIM** | Pruebas, validación y compatibilidad con Revit |
| **📊 Exportación de Datos** | Formatos de salida y procesamiento de resultados |
| **🧪 Testing & QA** | Validación de reglas estructurales GKS |

> 💬 **Nota**: Ambos desarrolladores colaboran en todas las áreas según la fase del proyecto.

---

## 📄 Licencia

**Propietario**: GKS  
**Tipo**: Uso Interno / Propietario  
**Restricciones**: Este es un prototipo técnico para uso exclusivo de GKS.

---

## 🎓 Documentación Adicional

- 📘 [Documentación Técnica Completa](docs/TECHNICAL.md)
- 🏗️ [Guía de Integración con Revit](revit/README.md)
- 🌐 [Manual de la Demo Web](web/README.md)

---

## 🐛 Reporte de Issues

Si encuentras problemas o tienes sugerencias:

1. Verifica que tu configuración `config.json` sea válida
2. Revisa los logs de salida en la consola
3. Contacta al equipo de desarrollo con:
   - Descripción del problema
   - Archivo `config.json` usado
   - Salida de error (si existe)

---

## 📊 Estado del Proyecto

- ✅ **Núcleo Algorítmico**: Completo y validado
- ✅ **Demo Interactiva**: Funcional
- ✅ **Salida JSON**: Implementada
- 🟡 **Add-in de Revit**: En desarrollo
- 🟡 **Exportación Excel**: En desarrollo
- ⏳ **Tests Unitarios**: Pendiente
- ⏳ **Documentación API**: Pendiente

---

<div align="center">

**MotorBloques GKS** - Automatización de Disposición de Bloques para BIM

Desarrollado por Esteban Retana y René Soto

</div>