# Dirección artística autónoma — BancoCorresponsal

**Fecha:** 10 de noviembre de 2026 (Lima). **Rama:** `review/portal-dwell-feedback`. **Entorno:** Unity 6000.3.24f1, URP 17.3.0, objetivo Android / Google Cardboard. **Estado:** cambios visuales integrados en la escena principal y verificados en el Editor; prueba física Android pendiente. No se hizo commit, push ni merge.

## Evidencia y alcance

- [Comparación real de nueve perspectivas antes/después](../Previews/ArtDirectionPolish/README.md). Todas usan coordenadas y parámetros de cámara idénticos; la primera y última fase provienen del Editor principal, no de una simulación externa.
- [Variante recuperable](../../Assets/_Project/Scenes/Review/BancoCorresponsal_ArtDirection_20261110.unity) revisada antes de aplicar los cambios a [BancoCorresponsal](../../Assets/_Project/Scenes/BancoCorresponsal.unity).
- Respaldo byte por byte previo: `Artifacts/ArtDirectionPolish/Backup-20261110-185745/` (escena, `.meta`, datos y texturas de luz, reflection probe original). Hash SHA-256 de la escena previa: `DF79B449B7071E793C2AB326BB1543A004FD90CDCF0B9B5FD11D077F78BEB39A`; escena final: `3E0F24C058F141309D71944B6F713959BEC8794948DBFE2DB05AEC19CA0289C6`. Se dejó intacto `Assets/_Recovery/` preexistente.
- Los archivos de referencia disponibles en el proyecto fueron `REF_Portal_Operations.png`, `DiseñoVisual_Completo.png` y `DiseñoVisual_PC.png`; no se localizaron `REF_Office_OperationsRoom.png` ni `REF_Analyst_Workstation.png` en el árbol actual, por lo que no afirmo una coincidencia literal con ellas. Se consultaron auditoría del 8 de noviembre, informes inmersivos, catálogo de assets y el informe GraphicsPolishV2.

## Diagnóstico inicial observado

| Zona | Problema visible antes | Decisión aplicada |
| --- | --- | --- |
| Entrada y portal | El portal era el foco correcto, pero el techo oscuro y las superficies dispares restaban aspecto institucional. | Conservar panel y controles; ordenar la arquitectura y enfriar la luz general. |
| Cubículos y circulación | Maderas naranjas, separadores y mobiliario grises sin una familia material clara; cuadrícula de techo gruesa y ciudad repetitiva. | Unificar madera, metal, tela y arquitectura; sustituir la cuadrícula y puntos repetidos por geometría combinada existente. |
| Puesto del analista | El escritorio funcional y sus objetos ya estaban bien distribuidos, pero la madera y su iluminación cálida dominaban demasiado. | Desaturar la madera, conservar la única luz de tarea cálida y el pequeño spill existente del monitor. |
| Documentación, impresora y archivo | Metal, papel y paredes se confundían por tono y valor. | Diferenciar esmalte de archivo, aluminio, papel marfil y pared mineral. |
| Supervisión y exterior nocturno | La estación quedaba oscura y los puntos de ventana parecían patrón decorativo. | Luz de trabajo más neutra, marcos más definidos y ventanas lejanas más espaciadas. |

La estructura, las pantallas bancarias TMP, la lógica de los modos y el portal aprobado no se rediseñaron. Los textos pequeños de monitores auxiliares siguen siendo secundarios y se aprecian principalmente de cerca.

## Cambios implementados y criterio

1. **Materiales.** Se reasignaron materiales compartidos en 180 renderers mediante la [herramienta de Editor](../../Assets/_Project/Editor/ArtDirectionPolish.cs). Se reutilizó la biblioteca existente `GraphicsPolishV2`: `Wood_DesaturatedOak`, `Wood_WalnutInset`, `Metal_GraphitePaint`, `Metal_ArchiveEnamel`, `Metal_AluminiumTrim`, `Plastic_EquipmentMatte`, `Plastic_Keycaps`, `Fabric_AcousticSlate`, `Fabric_SeatCharcoal`, `Paper_Ivory`, `Paper_StackEdges` y materiales de pared, techo, alfombra y pasillo. No se crearon materiales ni texturas nuevos. Las capturas muestran madera menos saturada y familias más reconocibles.
2. **Arquitectura.** Se añadieron cinco mallas combinadas **ya disponibles**: juntas de techo, alféizares, zócalos de cubículo, collares de columnas y luces de ciudad dispersas. Se desactivaron los renderers de la cuadrícula antigua y de los puntos de ciudad repetidos; no se borraron objetos. Las piezas nuevas no tienen colliders, no alteran teletransportes ni crean indicadores de interacción. [Antes](../Previews/ArtDirectionPolish/Before/02-general.png) / [después](../Previews/ArtDirectionPolish/FinalMain/02-general.png).
3. **Iluminación.** Se reajustaron 15 de las 17 luces horneadas por zona y se rehizo el bake. La luz de tarea del analista conserva un acento cálido moderado; techo, circulación, cubículos y supervisión son más neutros. Se mantuvieron las dos luces en tiempo real ya existentes, incluida la contribución tenue del monitor principal; no se añadió ninguna. También se mantuvo el único reflection probe. [Antes](../Previews/ArtDirectionPolish/Before/05-puesto-sentado.png) / [después](../Previews/ArtDirectionPolish/FinalMain/05-puesto-sentado.png).
4. **Pantallas.** Se conservaron los monitores World Space y TMP funcionales, el material de pantalla auxiliar y el spill local existente. La luz ambiente y los materiales vecinos mejoran su lectura como equipo encendido; no se atribuye iluminación física a la emisión o al Canvas. No se añadió bloom ni shader nuevo.

La etapa intermedia de materiales mantenía el escritorio demasiado naranja por el lightmap anterior; la etapa de arquitectura todavía mostraba el techo demasiado oscuro. Por eso ambas se conservaron solo como comparación y se integró **únicamente** la variante final tras el nuevo bake. Una captura preliminar en un proyecto Unity aislado mostró materiales magenta por importación incompleta; se descartó como evidencia visual y se guardó en `Artifacts/ArtDirectionPolish/RejectedIsolatedCapture/`. Las nueve capturas `Before/` válidas se obtuvieron después directamente en el Editor principal.

## Revisión visual final

- **Entrada/portal:** foco y barras legibles; la nueva arquitectura no invade el portal. La escala desde la entrada continúa dependiendo de la distancia real del jugador.
- **Cubículos/pasillos:** lectura más clara de suelos, separadores, sillas y escritorios; el paso sigue libre. Los puestos aún se parecen demasiado entre sí.
- **Analista de pie/sentado:** teclado, ratón, portapapeles, teléfono y dos monitores siguen en su ubicación funcional. Las fotos de [pie](../Previews/ArtDirectionPolish/FinalMain/04-puesto-pie.png) y [sentado](../Previews/ArtDirectionPolish/FinalMain/05-puesto-sentado.png) muestran que no se agregó clutter delante de los controles.
- **Documentación/impresora/archivo:** señalización y objetos existentes se ven con una separación material más clara. [Vista final](../Previews/ArtDirectionPolish/FinalMain/06-documentacion.png).
- **Seguridad/supervisión/resultados:** los monitores y sus interacciones permanecen; la luz secundaria es contenida. [Vista final](../Previews/ArtDirectionPolish/FinalMain/07-supervision.png). Las pantallas de resultados se comprobaron por prueba Play, no mediante una nueva captura de Android.
- **Ventanas, suelo y techo:** patrón exterior menos repetitivo y techo modular de lectura más fina. [Vista nocturna](../Previews/ArtDirectionPolish/FinalMain/08-nocturna.png) y [planta](../Previews/ArtDirectionPolish/FinalMain/09-planta.png).

El resultado se acerca a una oficina bancaria estilizada y coherente; aún se percibe el nivel low poly de varios muebles, archivadores, pantallas y objetos de fondo. No afirmo que iguale la ilustración conceptual.

## Verificación funcional

| Prueba | Variante visual | Escena principal final | Alcance |
| --- | --- | --- | --- |
| DomainTests | 27/27 PASS | Lógica compartida sin cambios | Datos, decisiones, evidencia y resultados. |
| GameplayFlowProbe | PASS | PASS | Prólogo omitido y completo, ambos modos, comparación y Aprobar/Retener/Devolver. |
| BankOfficeProbe | PASS | PASS | Monitor, impresora, archivo, alertas, defensa, nueve roles por raycast y cuatro teletransportes. |
| PrototypeSmokeProbe | PASS | PASS | TMP, seis decisiones por modo, registros, alertas, resultados y dwell con bloqueo/rearme. |
| Portapapeles | PASS | No repetida | Dwell real, 0/50/100 %, paginación, raycast y retorno a estación. [Captura Play](../Previews/ArtDirectionPolish/PlayVariant/clipboard/clipboard-open.png). |
| Teléfono | PASS | No repetida | Disponibilidad según modo, mirada, llamada y estado. [Captura Play](../Previews/ArtDirectionPolish/PlayVariant/phone/phone-ringing.png). |

Las pruebas Play fueron del **Editor con simulación de mirada**, no del visor Cardboard. La escena principal se reabrió desde su archivo guardado antes de repetir las pruebas: Unity había marcado temporalmente componentes TMP y MeshRenderer como dirty tras la compilación/captura, pero la copia guardada conservaba el hash esperado. Los recibos completos permanecen en `Artifacts/ArtDirectionPolish/` y `Artifacts/ImmersiveGameplay/`.

## Coste preliminar para Android VR

| Métrica de escena activa | Antes | Final | Cambio |
| --- | ---: | ---: | ---: |
| MeshRenderers | 282 | 285 | +3 netos |
| Materiales distintos usados | 48 | 53 | +5, todos preexistentes |
| Triángulos de MeshFilter | 31 204 | 32 632 | +1 428 (+4,6 %) |
| Luces horneadas | 17 | 17 | 0 |
| Luces en tiempo real | 2 | 2 | 0 |
| Lightmaps / reflection probes | 1 / 1 | 1 / 1 | 0 |
| Lightmap direccional en disco | 769 467 B | 764 935 B | −4 532 B |
| Lightmap color en disco | 8 501 692 B | 8 483 981 B | −17 711 B |

El número de renderers **no equivale a draw calls**. La malla se agrupó en piezas grandes, pero el uso de cinco materiales adicionales puede variar los lotes según el dispositivo. Las capturas se renderizaron en el Editor con `PC_RPAsset` y GPU NVIDIA GeForce GT 730; no son una captura del perfil de calidad Android. No se midieron draw calls, overdraw, memoria de GPU, tiempos por ojo, temperatura ni FPS en HONOR; el tamaño comprimido de lightmaps en disco no prueba uso de memoria en ejecución. No se cambió el perfil Android, calidad global, URP, XR/Cardboard ni paquetes. El siguiente gate es perfilar una APK en el teléfono antes de añadir más geometría o efectos.

## Pendientes priorizados

1. **HONOR / Cardboard:** comprobar vista estereoscópica, nitidez y lectura de ambos monitores, estabilidad a 60 FPS o al objetivo real del dispositivo, temperatura, draw calls, memoria de lightmaps, overdraw UI y comodidad desde pie/sentado. Verificar portal, prólogo, portapapeles, teléfono y teletransportes con movimientos reales de cabeza.
2. **Variación de cubículos:** diferenciar dos o tres puestos con los props ya disponibles y sin añadir colliders; comparar desde entrada y pasillo antes de integrarlos. Hoy siguen repetitivos.
3. **Mobiliario de fondo:** refinar siluetas de archivadores y soportes de monitores que se ven cuadrados, creando una variante de malla aislada. Mantener las piezas interactivas y sus colliders.
4. **Pantallas a distancia:** reducir textos diminutos en monitores auxiliares y confirmar escala angular desde posiciones reales de VR; no rehacer la lógica bancaria.
5. **Microanimación opcional:** un indicador breve de estado al consultar/activar un equipo, enlazado a un evento existente, sin bucle constante ni movimiento de cámara. Probar primero el coste y la distracción en dispositivo.
6. **Audio:** este trabajo no modifica sonido. La próxima pasada puede incorporar el ambiente y los cues ya inventariados en [ART_AUDIO_NEEDS.md](ART_AUDIO_NEEDS.md), con licencias verificadas y mezcla discreta.

No se encontró un error funcional nuevo en las pruebas ejecutadas. `git diff --check` señala seis líneas `value: ` con espacio final en el YAML de escena emitido por Unity; no se editó el YAML manualmente para ocultar ese formato generado. Quedan sin certificar el rendimiento y la comodidad Android VR y la equivalencia visual con las referencias de oficina/puesto ausentes del árbol local.
