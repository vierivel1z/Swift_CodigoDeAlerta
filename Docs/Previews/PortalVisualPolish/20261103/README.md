# Portal de operaciones — afinado visual

Capturas reales de Unity 6000.3.24f1 con URP existente. PNG de 1600 × 900, FOV 60°, cámara temporal perpendicular al panel. Los estados de foco y carga proceden del raycast y dwell reales; no se escriben valores decorativos en las barras.

| Captura | Estado |
|---|---|
| [Antes](00-before.png) | Estado previo, mismo encuadre frontal |
| [Frontal mejorado](01-front-polished.png) | Marco, cabecera, tarjetas, iconos y módulos terminados |
| [Normal en Play](02-play-normal.png) | Ambas barras vacías |
| [ATAQUE enfocado](03-attack-focused.png) | Borde e instrucción de mirada activos |
| [ATAQUE parcialmente lleno](04-attack-partial.png) | 49,17 % real; DEFENSA vacía |
| [DEFENSA enfocada](05-defense-focused.png) | Foco independiente |
| [DEFENSA parcialmente llena](06-defense-partial.png) | 49,24 % real; ATAQUE vacía |
| [Posición inicial real](07-real-initial-position.png) | Posición del jugador, cámara de captura orientada al portal |

Se abrieron y revisaron las imágenes junto con `REF_Portal_Operations.png`. El resultado conserva el formato de la consola y se acerca al concepto mediante bordes recortados, jerarquía, fondos ámbar/cian, iconos de mayor presencia y módulos de ojo/barra. No reproduce literalmente los efectos cinematográficos o la proporción del concepto. La referencia muestra dos barras llenas; el juego solo llena el modo realmente enfocado.

El punto y el anillo de la retícula siguen presentes en las capturas de foco. Su etiqueta duplicada se oculta en las tarjetas, que ya contienen la instrucción. Los textos secundarios desde la posición inicial necesitan todavía revisión binocular en HONOR.

`receipt.txt`: fecha UTC, cámara, FOV y contadores agregados del Editor. `hashes.json`: SHA-256 de los ocho PNG. PASS en Unity: revisión visual, mirada, PC, flujo, oficina y smoke. Android no ejecutado.

[Nota de implementación, respaldo y pruebas](../../../Archive/2026-11-10/Context/PORTAL_VISUAL_POLISH.md).
