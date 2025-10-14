/**
 * Visualizador.js
 * Dibuja la disposición de bloques en el elemento SVG y maneja la interacción.
 */

// Obtiene la configuración estática (offset, tolerancia)
function getStaticConfig() {
    if (typeof GKS_CONFIG !== 'undefined') {
        return GKS_CONFIG;
    }
    console.error("No se encontró la configuración estática GKS_CONFIG.");
    return null;
}

// ------------------------------------------
// Lógica Principal de Generación
// ------------------------------------------

function runGenerator() {
    const staticConfig = getStaticConfig();
    
    if (!staticConfig) return;

    // 1. Obtener valores DINÁMICOS del usuario
    const anchoArea = parseInt(document.getElementById('anchoArea').value) || 1000;
    const altoArea = parseInt(document.getElementById('altoArea').value) || 500;
    const junta = parseFloat(document.getElementById('junta').value) || 2;
    
    // Convertir la cadena de tamaños (ej: "300, 400, 500") a un array de números
    const tamanosBloqueStr = document.getElementById('tamanosBloque').value.split(',');
    const tamanosBloque = tamanosBloqueStr
        .map(s => parseInt(s.trim()))
        .filter(n => !isNaN(n) && n > 0);

    if (tamanosBloque.length === 0) {
        alert("Por favor, ingresa al menos un tamaño de bloque válido.");
        return;
    }
    
    // 2. Crear la configuración final, priorizando los valores del usuario
    const finalConfig = {
        ...staticConfig, // Copia valores estáticos (offset, tolerancia)
        anchoArea: anchoArea,
        altoArea: altoArea,
        junta: junta,
        tamanosBloque: tamanosBloque, // Sobreescribe con el input del usuario
        // Calculamos el alto del bloque para optimizar el área (5 filas)
        altoBloque: Math.floor((altoArea / 5) - junta) || 98 // Si el cálculo falla, usamos 98mm
    };

    // 3. Ajustar el contenedor SVG al nuevo tamaño del muro
    const svg = document.getElementById('app');
    svg.setAttribute('width', anchoArea);
    svg.setAttribute('height', altoArea);


    try {
        // 4. Ejecutar el motor
        const motor = new MotorBloques(finalConfig);
        const bloquesResultados = motor.run();
        
        // 5. Dibujar los resultados
        drawBlocks(bloquesResultados, finalConfig);
        
        console.table(bloquesResultados);
        console.log("Configuración Ejecutada:", finalConfig);

    } catch (error) {
        console.error("Error al ejecutar o visualizar el motor de bloques:", error);
        // Mostrar error en el área de visualización
        svg.innerHTML = `<text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" fill="red">Error al generar la disposición: Revise la consola. Mensaje: ${error.message}</text>`;
    }
}


// Función principal de dibujo (sin cambios de funcionalidad)
function drawBlocks(bloques, config) {
    const svg = document.getElementById('app');
    if (!svg) return;

    svg.innerHTML = '';
    
    const { anchoArea, altoArea } = config;

    // Definir colores para diferenciar los tamaños de bloque
    const COLOR_MAP = {
        '300': '#FFC0CB', // Rosa
        '400': '#ADD8E6', // Azul
        '500': '#90EE90'  // Verde
        // Si hay otros tamaños, se dibujarán grises por defecto.
    };
    
    // 1. Dibujar el contorno del área total
    const areaRect = document.createElementNS("http://www.w3.org/2000/svg", 'rect');
    areaRect.setAttribute('x', 0);
    areaRect.setAttribute('y', 0);
    areaRect.setAttribute('width', anchoArea);
    areaRect.setAttribute('height', altoArea);
    areaRect.setAttribute('fill', 'transparent');
    areaRect.setAttribute('stroke', '#333');
    areaRect.setAttribute('stroke-width', 3);
    svg.appendChild(areaRect);

    // 2. Dibujar cada bloque
    bloques.forEach(bloque => {
        const rect = document.createElementNS("http://www.w3.org/2000/svg", 'rect');
        const color = COLOR_MAP[bloque.tipo] || '#CCCCCC';
        
        rect.setAttribute('x', bloque.x);
        rect.setAttribute('y', bloque.y);
        rect.setAttribute('width', bloque.ancho);
        rect.setAttribute('height', bloque.alto);
        rect.setAttribute('fill', color);
        rect.setAttribute('stroke', '#666'); 
        rect.setAttribute('stroke-width', 1);
        rect.setAttribute('title', `Bloque: ${bloque.ancho} x ${bloque.alto} mm`);
        
        svg.appendChild(rect);
    });

    console.log(`Dibujados ${bloques.length} bloques en el SVG.`);
}


// ------------------------------------------
// Lógica de Ejecución (Inicialización)
// ------------------------------------------

document.addEventListener('DOMContentLoaded', () => {
    // 1. Conectar el botón para que ejecute el generador
    const runButton = document.getElementById('runMotor');
    runButton.addEventListener('click', runGenerator);
    
    // 2. Ejecutar una vez al inicio con los valores por defecto
    runGenerator();
});