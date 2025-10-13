/**
 * Visualizador.js
 * Dibuja la disposición de bloques en el elemento SVG.
 */

// Función para cargar la configuración JSON
function loadConfig() {
    // Lee la variable global definida en config.js
    // (Asume que GKS_CONFIG fue cargado previamente)
    if (typeof GKS_CONFIG !== 'undefined') {
        return GKS_CONFIG;
    }
    console.error("No se encontró la variable GKS_CONFIG.");
    return null;
}

// Función principal de dibujo
function drawBlocks(bloques, config) {
    const svg = document.getElementById('app');
    if (!svg) return;

    // Limpiar el SVG antes de dibujar
    svg.innerHTML = '';
    
    const { anchoArea, altoArea } = config;

    // Definir colores para diferenciar los tamaños de bloque
    const COLOR_MAP = {
        '300': '#FFC0CB', // Rosa claro
        '400': '#ADD8E6', // Azul claro
        '500': '#90EE90'  // Verde claro
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
        
        // Atributos de posición y tamaño
        rect.setAttribute('x', bloque.x);
        rect.setAttribute('y', bloque.y);
        rect.setAttribute('width', bloque.ancho);
        rect.setAttribute('height', bloque.alto);
        
        // Estilo basado en el tipo (tamaño)
        const color = COLOR_MAP[bloque.tipo] || '#CCCCCC';
        rect.setAttribute('fill', color);
        rect.setAttribute('stroke', '#666'); // Líneas de junta más oscuras
        rect.setAttribute('stroke-width', 1);
        
        // Agregar tooltip para demostrar el tamaño
        rect.setAttribute('data-ancho', bloque.ancho);
        rect.setAttribute('title', `Bloque: ${bloque.ancho} x ${bloque.alto} mm`);
        
        svg.appendChild(rect);
    });

    console.log(`Dibujados ${bloques.length} bloques en el SVG.`);
}

// ------------------------------------------
// Lógica de Ejecución (Se ejecuta al cargar el HTML)
// ------------------------------------------

document.addEventListener('DOMContentLoaded', () => {
    const config = loadConfig();
    
    if (config) {
        try {
            // 1. Ejecutar el motor
            const motor = new MotorBloques(config);
            const bloquesResultados = motor.run();
            
            // 2. Dibujar los resultados
            drawBlocks(bloquesResultados, config);
            
            console.table(bloquesResultados);

        } catch (error) {
            console.error("Error al ejecutar o visualizar el motor de bloques:", error);
            // Mostrar error en el área de visualización
            document.getElementById('app').innerHTML = `<text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" fill="red">Error al generar la disposición: Revise la consola. Mensaje: ${error.message}</text>`;
        }
    }
});