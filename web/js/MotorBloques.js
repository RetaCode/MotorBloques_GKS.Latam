class Bloque {
    constructor(x, y, ancho, alto, tipo) {
        this.x = x;
        this.y = y;
        this.ancho = ancho;
        this.alto = alto;
        this.tipo = tipo; // Ej: '300', '400', '500'
    }
}

class MotorBloques {
    constructor(config) {
        this.config = config;
        this.bloquesColocados = [];
    }

    /**
     * Motor principal: Genera la disposición de bloques fila por fila.
     */
    run() {
        this.bloquesColocados = [];
        const { anchoArea, altoArea, junta, tamanosBloque, offsetMataJunta, altoBloque } = this.config;
        
        // Convertir array de tamaños a números enteros y ordenar de mayor a menor
        const ANCHOS_DISPONIBLES = tamanosBloque.map(Number).sort((a, b) => b - a);
        const ANCHO_MINIMO_REQUERIDO = ANCHOS_DISPONIBLES[ANCHOS_DISPONIBLES.length - 1] + junta;
        
        let yActual = 0; // Coordenada Y de la fila actual

        // Iterar fila por fila hasta exceder la altura del área
        let numFila = 0;
        while (yActual + altoBloque <= altoArea) { 
            
            let xActual = 0; // Coordenada X (inicio) de la fila
            
            // Implementar Mata-Junta (Offset)
            const offset = (numFila % 2 !== 0) ? offsetMataJunta : 0;
            xActual += offset;
            
            // Si el espacio restante es menor que el bloque más pequeño, terminamos
            if (anchoArea - xActual < ANCHO_MINIMO_REQUERIDO) {
                // Si la fila tiene espacio para el bloque más chico, pero no para un bloque completo, NO colocamos nada
                if (numFila === 0 || bloquesFila.length === 0) break; 
            }

            let bloquesFila = [];

            // Algoritmo: Llenar la fila
            while (xActual < anchoArea) { 
                
                // Calculamos cuánto espacio queda *hasta el final* del muro
                const espacioRestanteAlMuro = anchoArea - xActual;

                // 1. PRIMERA BÚSQUEDA: Encontrar ajuste perfecto (Regla No-Corte)
                let mejorBloqueNoCorte = null;
                let mejorSobrante = this.config.tolerancia + 1;

                for (const anchoBloque of ANCHOS_DISPONIBLES) {
                    const anchoTotalBloque = anchoBloque + junta;
                    const sobrante = espacioRestanteAlMuro - anchoTotalBloque; 

                    // Condición No-Corte: El sobrante debe ser positivo y caer dentro de la tolerancia
                    if (sobrante >= 0 && sobrante <= this.config.tolerancia) {
                        if (sobrante < mejorSobrante) {
                            mejorBloqueNoCorte = anchoBloque;
                            mejorSobrante = sobrante;
                            if (sobrante === 0) break; // Ajuste perfecto!
                        }
                    }
                }
                
                // 2. COLOCACIÓN
                if (mejorBloqueNoCorte) {
                    // Caso A: Se encontró un ajuste perfecto/tolerable (No-Corte)
                    const anchoTotal = mejorBloqueNoCorte + junta;
                    
                    bloquesFila.push(new Bloque(
                        xActual,
                        yActual,
                        mejorBloqueNoCorte,
                        altoBloque,
                        mejorBloqueNoCorte.toString()
                    ));
                    
                    // Mueve X al final del muro (salto) para terminar la fila
                    xActual = anchoArea; 
                    
                } else {
                    // Caso B: No se encontró ajuste perfecto. Usamos Greedy para seguir llenando el espacio restante.
                    let anchoGreedy = null;

                    // Buscamos el bloque MÁS GRANDE que cabe en el espacio restante.
                    for (const anchoBloque of ANCHOS_DISPONIBLES) {
                         const anchoTotalBloque = anchoBloque + junta;
                         
                         // Sólo colocamos si el bloque cabe Y si después de colocarlo, aún queda espacio
                         // o si el espacio restante es mayor que el bloque más chico (para evitar dejar huecos grandes)
                         if (xActual + anchoTotalBloque <= anchoArea) {
                            // Se encontró el más grande que cabe.
                            anchoGreedy = anchoBloque;
                            break; 
                         }
                    }

                    if (anchoGreedy) {
                        const anchoTotal = anchoGreedy + junta;
                        bloquesFila.push(new Bloque(
                            xActual, 
                            yActual, 
                            anchoGreedy, 
                            altoBloque, 
                            anchoGreedy.toString()
                        ));
                        xActual += anchoTotal;
                    } else {
                        // El espacio restante es demasiado pequeño para colocar incluso el bloque más chico.
                        break; 
                    }
                }
            }
            
            // Guardar los bloques de la fila completa
            this.bloquesColocados.push(...bloquesFila);

            // Mover a la siguiente fila
            yActual += altoBloque + junta;
            numFila++;
        }

        return this.bloquesColocados;
    }
}