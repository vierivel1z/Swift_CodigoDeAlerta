# Capturas para revisión

**Estado inicial: no se generaron capturas en la nube.** Unity no está instalado ni activado allí. Esta carpeta está preparada para PNG reales de BancoCorresponsal; no contiene imágenes ilustrativas ni copias de capturas anteriores.

En Unity local: `SWIFT > Revision visual > Camaras y capturas PNG`.

Cada ejecución completa crea una subcarpeta con fecha UTC e identificador:

| Archivo | Perspectiva |
| --- | --- |
| `01-oficina.png` | General de la oficina |
| `02-escritorio.png` | Escritorio principal |
| `03-computadora-expedientes.png` | Computadora y documentos |
| `04-distribucion-superior.png` | Planta ortográfica, techo oculto temporalmente |
| `capture.json` | Fecha, Unity/URP, pipeline activo, hashes y encuadres |
| `README.md` | Galería de esa revisión |

`LATEST.md` se crea solo después de una sesión completa. Para comentar en GitHub, usa la sesión específica del PR: LATEST puede apuntar después a otra revisión. No edites PNG para aparentar cambios que no existen en la escena.

Las imágenes del Editor no acreditan rendimiento o funcionamiento en Cardboard. Los encuadres iniciales se basan en la distribución actual, pero requieren revisión local y ajuste si objetos o paredes los tapan. La captura estática no ejecuta lógica de Play.

Consulta [el procedimiento completo](../VisualReview/README.md) y [la investigación MCP](../VisualReview/UNITY_MCP.md).
