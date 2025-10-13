// Permite subir y visualizar output.json personalizado
function handleFileUpload(evt) {
  const file = evt.target.files[0];
  if (!file) return;
  const reader = new FileReader();
  reader.onload = function(e) {
    try {
      const resultado = JSON.parse(e.target.result);
      renderSVG(resultado);
    } catch (err) {
      alert('Archivo JSON inválido');
    }
  };
  reader.readAsText(file);
}

function renderSVG(resultado) {
  const svg = document.getElementById('muro');
  svg.innerHTML = '';
  if (!resultado.Bloques) return;
  resultado.Bloques.forEach((b, i) => {
    const rect = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
    rect.setAttribute('x', b.X);
    rect.setAttribute('y', b.Y);
    rect.setAttribute('width', b.Ancho);
    rect.setAttribute('height', b.Alto);
    rect.setAttribute('fill', i % 2 === 0 ? '#8ecae6' : '#219ebc');
    rect.setAttribute('class', 'block');
    svg.appendChild(rect);
  });
}

document.getElementById('fileInput').addEventListener('change', handleFileUpload);