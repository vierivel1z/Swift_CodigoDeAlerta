> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Integración de artes restantes — gráfica institucional y prólogo

Fecha: 2026-11-02. Unity 6000.3.24f1 / URP 17.3.0. Escena: `Assets/_Project/Scenes/BancoCorresponsal.unity`. Estado: implementado y comprobado en Editor; aceptación visual del usuario y Android pendientes.

## Recursos encontrados y decisión de integración

| Recurso original | Dimensiones | Uso |
|---|---|---|
| `Assets/_Project/Art/WallGraphics/ART_BankWallGraphic.png` | 1672 × 941 | Textura del único cuadro institucional, conservando su proporción. |
| `Assets/_Project/Art/Prologue/Reference/prologo_01_impresora_origen.png` | 1920 × 1080 | Primer momento, impresora del banco de origen. |
| `Assets/_Project/Art/Prologue/Reference/prologo_02_impresora_detenida.png` | 1920 × 1080 | Segundo momento, misma impresora y empleado genérico del banco de origen. |
| `Assets/_Project/Art/Prologue/Reference/prologo_03_banco_corresponsal.png` | 1920 × 1080 | Conservado como referencia conceptual; la oficina ilustrada no coincide con el juego. No se usa en el prólogo final. |
| `Assets/_Project/Art/Prologue/Reference/PROLOGUE_Storyboard_Reference.png` | 1672 × 941 | Referencia compuesta conservada. Ya existían imágenes individuales: no fue necesario extraer otras del compuesto. |

La tercera imagen final es `Assets/_Project/Art/Integrated/Prologue_BancoReal.png`, un render real de BancoCorresponsal a 1600 × 1000. Se actualizó en Play Mode para incluir la interfaz nativa actual, sin modificar sus scripts ni recursos documentales.

Los cinco PNG proporcionados permanecen idénticos por SHA256 al respaldo. Se ajustaron únicamente los importadores de la gráfica y los dos primeros momentos: textura Default, NPOT sin reescalado a potencias de dos y Clamp. Así se evita que el importador altere la relación de aspecto. Se mantienen mipmaps y la configuración de compresión existente.

## Gráfica institucional

- Objeto único: `ART - Bank Wall Graphic`.
- Ubicación del grupo: **(-4,84; 1,95; 0,60)**, pared oeste/lateral, entre bienvenida y archivadores. Giro Y: **−90°**, cara visible hacia el interior (+X).
- Imagen: **2,20 × 1,238 m**; marco: 2,26 × 1,298 m, grosor 0,035 m. Quad y cubo sencillos, sin colliders.
- Materiales propios nuevos: `MAT_BankWallGraphic.mat` y `MAT_BankWallFrame.mat`, ambos URP Lit, Metallic 0, Smoothness 0,08, emisión negra y keyword de emisión desactivada. Usa iluminación existente/probes; no se añadieron luces ni se rehízo el bake.
- La primera ubicación quedaba detrás del cartel Welcome. Se corrigió desplazando solo el grupo nuevo; no se movió el cartel ni mobiliario existente. La primera captura se conserva en `iteration-wall-first/`.
- Orientación, proporción, visibilidad y separación respecto al recorrido se revisaron en capturas reales. Las sondas posteriores comprobaron raycasts y teletransportes. No se duplicó el cuadro en otras paredes.

## Prólogo ilustrado

`IntroSequence` conserva sus tres momentos, **4 segundos por momento**, aproximadamente **12 segundos** en total, y su callback de finalización. El rig/cámara no se anima ni se reparenta. El panel queda fijo en el mundo a 2,5 m de la posición/dirección inicial de la cámara; el jugador conserva la libertad de mirar.

La presentación usa RawImage y TMP: encabezado, ilustración, subtítulos y un control Omitir grande e independiente. El Canvas mide 1200 × 800 a escala 0,002, unos 2,4 × 1,6 m (aproximadamente 51° × 35,5° a 2,5 m). Esa geometría no certifica comodidad en el teléfono.

Los dos primeros individuales incluyen amplias franjas de relleno. Se muestran mediante UV, sin reescribir el PNG ni estirar la ilustración:

| Imagen | Área útil en píxeles, origen arriba a la izquierda | UV (origen inferior de Unity) |
|---|---|---|
| Primer momento | X [0,1920), Y [372,707) | X 0, Y 373/1080, ancho 1, alto 335/1080 |
| Segundo momento | X [0,1920), Y [375,708) | X 0, Y 372/1080, ancho 1, alto 333/1080 |
| Banco real | Imagen completa | (0,0,1,1) |

Se inspeccionaron visualmente los límites y las diferencias de filas de los originales para excluir relleno/borde. La imagen se ajusta dentro del marco conservando su aspecto, con espacio sobrante cuando corresponde. No se amplía de forma desigual para llenar el panel.

La ilustración aparece/desaparece suavemente durante unos **0,3 s** en cada extremo del momento, incluidos dentro de los 4 s existentes. Títulos, subtítulos y Omitir no se desvanecen. No hay destellos ni movimientos de cámara.

Los momentos 1–2 dicen explícitamente **BANCO ANDINO — BANCO DE ORIGEN / RECREACIÓN EDUCATIVA**; el segundo conserva la aclaración de que no es el puesto del jugador. El tercero presenta el **Banco Corresponsal Internacional ficticio**, con la oficina real del juego. No se trasladó al empleado del banco de origen al rol del analista.

## Cambios de archivos y escena

Modificados:

- `Assets/_Project/Scripts/Core/IntroSequence.cs`: ilustraciones asignables, UV, ajuste de proporción, transición suave y presentación TMP. Conserva omisión/final automático y duración.
- `Assets/_Project/Scenes/BancoCorresponsal.unity`: grupo decorativo nuevo y componente IntroSequence con tres referencias de imagen/UV en el objeto de sistemas existente. Unity serializó también los campos predeterminados de procedimiento que UIManager ya tenía de la tarea anterior; no se alteró su código ni su composición documental.
- Los tres `.png.meta` mencionados: importación de textura, NPOT/Clamp.
- `GameplayFlowProbe.cs`, `BankOfficeProbe.cs`, `PrototypeSmokeProbe.cs`: únicamente opción `ExitEditorOnComplete`, predeterminada true, para poder ejecutarlas bajo coordinación sin cerrar el Editor abierto. Se conservaron las aserciones y el comportamiento batch predeterminado.
- Documentación: este informe, créditos, KANBAN y CHANGELOG.

Nuevos:

- `Assets/_Project/Editor/ArtIntegrationReview.cs` y `.meta`: cola local explícita de instalación/revisión, capturas y ejecución de las sondas existentes. No instala servicios ni dependencias.
- `Assets/_Project/Art/Integrated/` y `.meta`, dos materiales con sus `.meta`, render `Prologue_BancoReal.png` con su `.meta` y copia de recuperación fechada de escena en `Snapshots/`.
- Capturas, README y manifiesto bajo `Docs/Previews/ArtIntegrationReview/20261102T145831568Z/`.
- Logs, comprobaciones, hashes y evidencias anteriores conservadas en `Artifacts/ArtIntegrationReview/`.

No se modificaron los scripts de documentos, PNG ficticios, GameManager, dominio bancario, ScenarioData/TrainingScenario, prefabs, modelos, luces, lightmaps, paquetes, configuración URP/Cardboard ni escenas de compilación.

## Protección y respaldo

Se revisó Git antes de editar. Rama existente `review/unity-scene-designer`, base `3bef77be8ea73637f37e150b4884d67905609735`; había numerosos cambios locales previos. Sin commit, merge, push, reset ni clean.

Respaldo completo previo de Assets, Packages, ProjectSettings y Docs:

`D:\UnityGames\SWIFT_Backups\ArtIntegration-20261102-095628`

Restaurar selectivamente comparando cada archivo con ese respaldo y conservando antes su versión actual; no reemplazar todo el proyecto ni avances posteriores. Las evidencias anteriores de las tres sondas se copiaron a `Artifacts/ArtIntegrationReview/PreviousEvidence/` antes de volver a ejecutarlas.

La comparación de **838 archivos originales** encontró **cero eliminados** y solo los ocho cambios esperados: escena, IntroSequence, tres importadores y tres sondas. Todos los archivos originales restantes siguen idénticos. `preservation.json` contiene la lista; `scene-object-diff.json` y `scene-full.diff` muestran que no se eliminaron objetos serializados originales ni se cambiaron transformaciones existentes.

Unity marcó transitoriamente la escena como modificada tras la revisión. Se conservó su estado en `Art/Integrated/Snapshots/Memory-20261102T151513952Z.unity` y otra copia fuera del checkout. La comparación de ese estado con la escena de disco fue **idéntica** (`memory-vs-disk.diff` vacío); se guardó ese mismo estado antes de continuar. No se descartaron datos diferentes.

## Pruebas ejecutadas y evidencia

| Prueba | Resultado y alcance |
|---|---|
| DomainTests | **27 PASS**, reglas y cálculos existentes. `Artifacts/ArtIntegrationReview/domain-tests.txt`. |
| Revisión específica de arte | **PASS**: cuadro único sin colliders, NPOT/aspecto, tres ilustraciones, TMP sin desbordes, Omitir alcanzable por Physics.Raycast desde la cámara real y activación programática funcional. |
| Inicio único | **PASS**: omisión inicia ataque una vez; volver a llamar Skip no inicia otra sesión. Final automático inicia defensa una vez, sin iniciar antes del final. |
| Duración / cámara | **PASS**: tres momentos con tiempo de producción, fin automático aproximado de 12 s, posición/rotación de la cámara sin cambios durante la secuencia. |
| GameplayFlowProbe | **PASS**: omisión, 12 s reales, ambos modos, comparación, ciclo de botones y Aprobar/Devolver/Retener sobre seis expedientes, 6/6. |
| BankOfficeProbe | **PASS**: una cámara, Canvas, cinco funciones, nueve roles alcanzables por raycasts, decisión y cuatro teletransportes. |
| PrototypeSmokeProbe | **PASS**: TMP/fuentes, seis decisiones por modo, evidencias/registro/alertas, controles, resultados, teletransportes y selección por mirada con bloqueo/rearme. Su prueba de dwell usa el umbral acelerado propio de esa sonda; no es una medición física de Cardboard. |
| Compilación / runtime | Scripts compilados por Unity 6000.3.24f1; sin errores de compilación observados. Las revisiones finales terminaron PASS sin errores runtime registrados durante sus ejecuciones. |
| Android / HONOR | **NO PROBADO**. Sin instalación, estéreo físico, giroscopio, confort, FPS, memoria, temperatura ni audición de dispositivo. |

Recibos finales: `prologue-result.txt`, `flow-result.txt`, `office-result.txt`, `smoke-result.txt`; detalle literal en `*-probe.txt`, `checks.txt` y `unity-editor.log`.

Se conservaron los diagnósticos de intentos cancelados y errores de coordinación anteriores en el log: comandos leídos durante una escritura y cancelaciones por estado Play/dirty. Los comandos finales se enviaron mediante renombrado atómico y el revisor espera a terminar las transiciones de Play. Se corrigió además la salida de Play para limitarla a ejecuciones iniciadas por el propio revisor. No se presenta toda la Console histórica como vacía.

La comprobación de formato de fuentes/documentos no encontró problemas de espacios. `git diff --check` sobre la escena serializada sí señala espacios finales en campos vacíos como `m_Name:`/`value:` escritos por Unity y presentes en el conjunto de cambios locales. No se normalizó manualmente ese YAML para silenciar el aviso; no es un error de compilación ni de las sondas. Registro: `Artifacts/ArtIntegrationReview/git-diff-check.txt`.

## Capturas reales generadas

Galería: `Docs/Previews/ArtIntegrationReview/20261102T145831568Z/README.md`. Todas son renders reales de Unity a 1600 × 1000, con cámaras temporales que no quedan en la escena. No son capturas Android.

1. `01-grafica-pared.png`: ubicación final del cuadro, visible entre bienvenida y archivadores — Edición.
2. `02-prologo-impresora.png`: primer momento — Play Mode.
3. `03-prologo-detenida.png`: segundo momento — Play Mode.
4. `04-prologo-banco-real.png`: tercer momento, con render del banco y su UI actual — Play Mode.
5. `05-oficina-partida.png`: oficina tras iniciar defensa, interfaz de expediente activa — Play Mode.
6. `06-oficina-general.png`: ubicación relativa del cuadro, entrada, escritorio y circulación — Edición.
7. `07-escritorio.png`: relación entre cuadro y puesto de trabajo — Edición.
8. `08-computadora.png`: encuadre próximo del monitor serializado — Edición; no sustituye la evidencia de la UI creada en Play.
9. `09-distribucion-superior.png`: distribución, cuatro pads y circulación, techo oculto solo durante captura — Edición.

`capture-receipt.txt` distingue Play/Edición. `capture.json` registra fechas, hashes, escena y fuentes. Las carpetas de iteración conservan la primera ubicación y la primera versión del render del banco; no se presentan como finales.

## Calidad, pendientes y recursos adicionales

- No se observaron texturas rosas, imagen invertida, estiramiento, oclusión de la gráfica por el cartel ni subtítulos desbordados en las capturas finales.
- Las ilustraciones 1–2 son panoramas muy anchos: el área útil ronda 5,7:1. Conservar toda su composición limita su altura en el panel. Se reconoce la impresora/empleado, pero no se pretende leer el papel ilustrado; los textos relevantes permanecen en TMP.
- El render del banco incluye textos pequeños dentro de la imagen, usados como contexto visual. Las instrucciones operativas legibles son los subtítulos TMP, no el texto de ese render.
- Los PNG de revisión desactivan AA/postprocesado de sus cámaras; sus bordes no acreditan la nitidez del perfil móvil. No se cambió calidad global para mejorar capturas.
- No falta ningún recurso imprescindible para esta integración. **Opcional**, si se busca una ilustración más alta: dos imágenes independientes 16:9 de la misma impresora/personaje, sin franjas de relleno. No se generaron ni se necesitan para ejecutar esta versión.
- Pendiente generar una APK nueva con interfaz y artes actuales. La APK funcional previa no contiene estos cambios posteriores; no se compiló otra APK en esta tarea.
- En HONOR: comprobar orientación y tamaño del cuadro al entrar/acercarse al archivo; verificar tres momentos, libertad de mirada, Omitir por fijación real de 1,5 s, inicio único de ambos modos, cuatro pads y tres decisiones; medir rendimiento, calor y confort. Audio de impresora/ambiente queda como mejora separada según ART_AUDIO_NEEDS.md.

La implementación no equivale a aprobación artística final del usuario ni a aprobación Android VR.
