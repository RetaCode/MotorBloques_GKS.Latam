// Motor visual JS para bloques GKS
fetch('config.json')
  .then(r => r.json())
  .then(config => {
    const svg = document.getElementById('muro');
    const info = document.getElementById('info');
    const anchoArea = 900;
    const altoArea = 400;
    let y = 0;
    let fila = 0;
    let bloques = [];
    while (y + config.tamanos[0] <= altoArea + config.tolerancia) {
      let x = 0;
      let tamanoBloque = config.tamanos[fila % config.tamanos.length];
      while (x + tamanoBloque <= anchoArea + config.tolerancia) {
        bloques.push({ x, y, w: tamanoBloque, h: tamanoBloque });
        x += tamanoBloque + config.anchoJunta;
      }
      y += tamanoBloque + config.anchoJunta;
      fila++;
    }
    info.innerHTML = `Bloques generados: <b>${bloques.length}</b>`;
    // Render SVG
    bloques.forEach((b, i) => {
      const rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
      rect.setAttribute('x', b.x);
      rect.setAttribute('y', b.y);
      rect.setAttribute('width', b.w);
      rect.setAttribute('height', b.h);
      rect.setAttribute('fill', i % 2 === 0 ? '#8ecae6' : '#219ebc');
      rect.setAttribute('class', 'block');
      svg.appendChild(rect);
    });
  });