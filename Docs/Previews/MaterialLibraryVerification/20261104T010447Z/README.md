# Verificación de materiales en Unity

Prueba nueva sobre el catálogo de c4a1278, con Unity6000.3.24f1 / Direct3D11. Reapertura y Play Mode: PASS, cero errores y advertencias capturados durante las pruebas. Perfil activo: Mobile_RPAsset; plataforma seleccionada Android. Esto sigue siendo ejecución de Editor, no de teléfono.

14 capturas nuevas: siete en Editor y las mismas siete en Play. Se conservan las capturas del commit anterior. [Resultados, configuración y hashes](verification.json), [comprobaciones](checks.txt), [recibos](receipt.txt).

| Vista | Editor | Play Mode |
|---|---|---|
| General | [PNG](Editor/00.png) | [PNG](Play/00.png) |
| Madera | [PNG](Editor/01.png) | [PNG](Play/01.png) |
| Plástico | [PNG](Editor/02.png) | [PNG](Play/02.png) |
| Metal pintado | [PNG](Editor/03.png) | [PNG](Play/03.png) |
| Tela | [PNG](Editor/04.png) | [PNG](Play/04.png) |
| Papel | [PNG](Editor/05.png) | [PNG](Play/05.png) |
| Arquitectura | [PNG](Editor/06.png) | [PNG](Play/06.png) |

Se revisaron visualmente las siete vistas de Editor y la general en Play: materiales diferenciados, sin rosa, etiquetas legibles y sin recortes. Los 1073 archivos previos de Assets/Packages/ProjectSettings permanecen idénticos por SHA-256. BancoCorresponsal no se modifica. No se mide FPS, GPU, moiré binocular o comodidad Cardboard. Las sondas bancarias quedan fuera del alcance de esta verificación del catálogo.
