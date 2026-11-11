# Kanban del equipo 05

Estado disponible: `PENDIENTE` | `EN_PROGRESO` | `BLOQUEADO` | `HECHO`.
No marcar `HECHO` sin evidencia (captura, archivo/script, commit o prueba realizada). Asignar responsables cuando lo acuerde el equipo.

| ID | Estado | Prioridad | Tarea | Criterio de aceptación | Responsable |
|---|---|---|---|---|---|
| K01 | BLOQUEADO | Alta | Confirmar pestañas Funcional/Técnico del escenario 5 | Requisitos particulares incorporados a PROJECT_CONTEXT | Por asignar |
| K02 | HECHO | Alta | Validar pipeline actual de Unity y configuración Android | Editor abre sin errores y plataforma Android disponible | Por asignar |
| K03 | EN_PROGRESO | Alta | Inicializar repositorio GitHub y gitignore de Unity | Proyecto compartible, commits de cada integrante | Por asignar |
| K04 | EN_PROGRESO | Alta | Instalar Cardboard y probar `HelloCardboard` | APK probado en Android real | Por asignar |
| K05 | HECHO | Alta | Crear sala banco MVP con primitivas | Piso, paredes, escritorio y pantalla visibles | Por asignar |
| K06 | EN_PROGRESO | Alta | Cámara de desarrollo para Editor | Mouse/teclado permiten revisar escena | Por asignar |
| K07 | EN_PROGRESO | Alta | Retícula y selección por mirada | Un objeto responde a enfoque temporizado | Por asignar |
| K08 | EN_PROGRESO | Alta | Portal de entrada | Jugador inicia experiencia desde portal | Por asignar |
| K09 | EN_PROGRESO | Alta | Teletransporte por mirada | Dos puntos funcionales, sin movimiento brusco | Por asignar |
| K10 | HECHO | Alta | `GameManager` y `ScenarioData` | Arquitectura de estados y datos conectada | Por asignar |
| K11 | HECHO | Alta | Métodos obligatorios de evidencias y defensa | `RegisterEvidence`, `ApplyDefense`, `EndScenario` con uso real | Por asignar |
| K12 | EN_PROGRESO | Alta | Panel de expediente en monitor (World Space) | Texto legible y datos ficticios | Por asignar |
| K13 | HECHO | Alta | Mecánica Aprobar/Retener/Devolver | Se registran decisiones y se muestra resultado | Por asignar |
| K14 | EN_PROGRESO | Alta | Cinco objetos VR interactivos | Los cinco tienen feedback y función propia | Por asignar |
| K15 | HECHO | Alta | Modo ataque | Resultado acorde al resumen histórico | Por asignar |
| K16 | HECHO | Alta | Modo defensa | Cambia controles y resultado simulado | Por asignar |
| K17 | EN_PROGRESO | Media | Modelos low-poly e iluminación | Estilo coherente, sin sobrecargar Android | Por asignar |
| K18 | EN_PROGRESO | Media | Audio, textos y pantalla final | Audio funcional, créditos y comparación visibles | Por asignar |
| K19 | EN_PROGRESO | Alta | Probar APK e integrar todos los componentes | Pruebas documentadas en teléfono real | Por asignar |
| K20 | PENDIENTE | Media | Entrega: video, ficha y presentación | Video 2–3 min, ficha ≤4 páginas, exposición 10 min | Por asignar |

## Primer sprint sugerido
1. K01/K02/K03: confirmar requisitos, pipeline y repositorio.
2. K04: primer APK Cardboard.
3. K05/K06/K07: una sala vacía y un objeto seleccionable en Editor.
4. K10/K12/K13: una sola orden de prueba y una decisión.

## Registro de pruebas
| Fecha | Equipo/Dispositivo | Prueba | Resultado | Evidencia |
|---|---|---|---|---|
| Pendiente | Pendiente | Primer APK Android | No probado | — |
| 2026-10-27 | PC de desarrollo | Tools/Test-ProjectConfiguration.ps1 con EditorPath local | Sin excepciones; manifest/lock coherentes, SDK/NDK/JDK presentes; compilación y teléfono pendientes | INITIAL_AUDIT.md |


## Inspección inicial — 2026-10-27
- K02: EN_PROGRESO; diagnóstico estático realizado, pendiente de validación en Unity.
- Dependencias pendientes: verificar manualmente Console actual, plataforma Android y rutas activas de herramientas.
- Evidencia: INITIAL_AUDIT.md y Tools/Test-ProjectConfiguration.ps1 (desde la raíz).
- Cambios: diagnóstico reproducible, informe, contexto y este tablero. Sin cambios de paquetes, escenas ni pipeline.
- K01 sigue pendiente del material del profesor. K03 no iniciado: no existe repositorio ni .gitignore.
- K04–K20 siguen pendientes; no hay implementación del juego ni evidencia de APK/entrega.
- Siguiente: cerrar validación manual de K02; K03 puede prepararse de forma independiente.




## Sesión autónoma — 2026-10-27 (estado posterior al diagnóstico inicial)

La misión autónoma amplió el alcance del primer sprint. Se conservan IDs, nombres y
criterios de aceptación. HECHO aquí exige evidencia de Unity o pruebas de lógica; no
supone aceptación de la APK/visor, que sigue en K04/K19.

| ID | Evidencia / pendiente actual |
|---|---|
| K01 | BLOQUEADO: faltan las pestañas particulares del escenario 5 del docente. |
| K02 | HECHO: Unity 6000.3.24f1 ejecutó compilación, escena y Play Mode en copia aislada; Android disponible; configuración trasladada y comprobada. |
| K03 | Local preparado, .gitignore comprobado, sin commits ni remoto. Falta GitHub y contribuciones del equipo, fuera de la autorización de publicación. |
| K04 | Cardboard 1.36.0 y XR Management resueltos; loader Android asignado. Falta HelloCardboard y APK en teléfono real. |
| K05 | HECHO: BancoCorresponsal y CardboardSmokeTest generadas/reabiertas; primitivas y materiales; captura de cámara revisada. |
| K06 | Adaptador mouse/WASD y clic implementado/compilado. Falta prueba humana de controles PC. |
| K07 | Dwell, una activación y rearme comprobados en Play Mode simulado; falta mirada con pose real. |
| K08 | Portales por collider y acción para iniciar/reiniciar modos; validar uso físico/visual. |
| K09 | Dos destinos cambian la posición del rig en Play Mode; falta comodidad y mirada en visor. |
| K10 | HECHO: GameManager/ScenarioData conectados, sesiones separadas de entrada/UI, validación Unity. |
| K11 | HECHO: RegisterEvidence, ApplyDefense y EndScenario con uso real, pruebas dominio/Play Mode. |
| K12 | Canvas World Space y TMP con fuente válida, alertas/estado/botones. Revisión PC realizada; falta legibilidad en visor. |
| K13 | HECHO: decisiones actualizan estados, resultados y feedback en UI; 27 checks de dominio y prueba Unity. |
| K14 | Diez objetos/portales/puntos con BankInteractable, tres botones, retícula y audio/texto. Falta aceptación de las cinco interacciones en VR real. |
| K15 | HECHO: modo ataque ficticio + resumen histórico fijo conforme al contexto recibido; no se confunden ambas cifras. |
| K16 | HECHO: controles en defensa, diferencia de pérdidas ficticias 965000 frente a 0 en la prueba de aprobar todo; comparación en pantalla. |
| K17 | Pendiente: arte definitivo/optimización; solo primitivas del MVP. |
| K18 | Tono generado, créditos, resultados y textos implementados; pendiente escucha y revisión humana final. |
| K19 | APK final generada (0 errores), firma/manifiesto/ARM64 comprobados; ver SESSION_REPORT.md. Instalación, estéreo y rendimiento en teléfono pendientes. |
| K20 | Entregables académicos finales pendientes. |

Archivos y pruebas por bloque: SESSION_REPORT.md. Cómo jugar, regenerar y validar:
PROTOTYPE_GUIDE.md. Siguiente prioridad: comprobar el flujo PC y aportar prueba física
Cardboard/APK (K04/K19), manteniendo K01 pendiente del material docente.

### Evidencias de esta sesión
- Artifacts/domain-tests.txt: 27 comprobaciones aprobadas.
- Artifacts/unity-validation-retry.log: generación/reapertura y compilación Unity, salida 0.
- Artifacts/unity-playmode-releasecheck.log y playmode-smoke.txt: UI, decisiones, modos, evidencias, teletransporte y dwell aprobados.
- Artifacts/configuration-final.txt: manifest/lock coherentes y ajustes Android del proyecto principal.
- Artifacts/prototype-preview.png: captura renderizada revisada (no evidencia de teléfono).
- APK real / visor: no probado.

### Cierre técnico de sesión
APK final: Artifacts/SWIFT-prototype.apk, 66,528,319 bytes; build Unity Succeeded con 0 errores,
firma v2 válida, API 26/35, ARM64 y biblioteca Cardboard incluida. Prueba física pendiente.
Última prueba de Play Mode: PASS. Código fuente sincronizado por hash con la copia validada.
Licencias completas incluidas en Resources/ThirdPartyNotices.txt.


## Oficina importada y preparación GitHub — 2026-10-28
- K17 EN_PROGRESO: primera oficina visual implementada con modelos reales; quince prefabs
  propios, materiales URP, cuatro zonas y 10601 triángulos activos (74 mesh renderers).
  Pendiente rendimiento/legibilidad/comodidad en teléfono. Ver OFFICE_SCENE_REPORT.md.
- K12: Canvas World Space/TMP guardado y anclado a la computadora, datos y decisiones
  conectados al sistema existente; revisión PC realizada, visor pendiente.
- K14: computadora, impresora, archivadores y ambos paneles con Collider y BankInteractable;
  acciones verificadas en Play Mode. Cuatro teletransportes verificados; hardware pendiente.
- K05: se conserva el hito original; respaldo de escena y geometría anterior desactivada.
- Siete ZIP inspeccionados; seis paquetes importados; Office Pack sin licencia no utilizado.
  Originales conservados por hash. Créditos completos en ASSET_CREDITS.md y Resources.
- Compilación C#, reapertura, referencias, materiales y Play Mode: PASS en Unity 6000.3.24f1.
  Evidencia local: Artifacts/Office20261028 (build-office.log, play-office-final.log y capturas).
- La APK de 2026-10-27 no contiene esta oficina nueva. No se ha realizado nueva prueba Android.
- K03: el usuario autorizó explícitamente el primer commit y publicación PRIVADA en GitHub
  el 2026-10-28. Preparación en main para vierivel1z/SWIFT_CodigoDeAlerta. Resultado de
  publicación y hash se registran en el resumen final y Artifacts/GitHub local.
  Las contribuciones del resto del equipo siguen pendientes; no se cierra ese criterio.
- Se excluyen ambos nombres ExternalAssets/ y External Assets/, PruebaVR (builds de prueba),
  cachés y APK. Assets/Models, prefabs, texturas y .meta se incluyen.

GitHub Desktop creó durante la tarea un repositorio anidado con el commit inicial 6ba7e8d71b7b5bf3579b5079719701d5836a26c1. Se conserva la carpeta y se adopta ese historial en la raíz del proyecto, reutilizando el remoto privado existente sin duplicarlo ni forzar push.

## Arte UI y base de prólogo — 2026-10-29
- Docs/ArtPrompts: tres propuestas de pantallas/botones/estados y prompts del prólogo. Solo texto, sin imágenes generadas ni recursos externos nuevos.
- Docs/Context/TWO_MODES.md: alcance, flujo y aceptación de ataque/defensa según material disponible. K01 continúa bloqueado por pestañas docentes no recibidas.
- IntroSequence: storyboard runtime de tres momentos, papel animado, subtítulos, duración aproximada 12 s y omisión por mirada/clic. Se abre la primera vez desde un portal; inicia el modo elegido al terminar/omitir. No mueve cámara ni modifica escena guardada. Base provisional, no cinemática final.
- BankInteractable: ignora acciones del banco/teletransporte durante el prólogo; mantiene SkipIntro operativo. Enumeración extendida al final para conservar valores serializados.
- UIManager: oculta decisiones sin expediente y al finalizar; las restaura al iniciar otro turno. Resultados con más espacio y texto mayor, feedback separado. Créditos breves reconocen modelos actuales.
- GameplayFlowProbe/GameplayFlowValidation: prueba de omisión, final temporizado, resultados de ambos modos, comparación y estados de botones; método para copia aislada Swift.EditorTools.GameplayFlowValidation.Run (sin -quit).
- Evidencia: 27 pruebas de dominio PASS. Compilación C# contra bibliotecas locales Unity: 0 errores; advertencia por referencia al proyecto de samples ausente en la copia del csproj de comprobación. Esto no acredita Play Mode ni Android.
- Primera importación Unity aislada detenida al permanecer en Initial Refresh; segundo intento sin render registrado en Artifacts/Gameplay20261029/play.log. Consultar informe final para resultado; no dar aceptación VR por estas comprobaciones.
- Conservados cambios locales previos en ProjectSettings y documentación/capturas. No se cambió escena, pipeline, XR ni paquetes del proyecto principal.

### Resultado final de comprobación
- Artifacts/Gameplay20261029/code-check.txt: compilación final de scripts contra bibliotecas locales Unity, 0 errores y 0 advertencias (retirada la referencia innecesaria al proyecto de samples solo en el csproj temporal de comprobación).
- 27 comprobaciones de dominio aprobadas.
- Dos intentos de Unity sobre copia aislada, con y sin render, permanecieron en AssetDatabase Initial Refresh. Se detuvieron exclusivamente los procesos de validación creados para esta tarea. Play Mode, final temporizado real, disposición visual y visor quedan PENDIENTES; no existe resultado PASS de GameplayFlowProbe en esta sesión.
- No se guardó ni reconstruyó BancoCorresponsal. El Editor original del usuario permanece abierto.

## Cierre del prototipo funcional — 2026-11-02 (estado posterior a PROJECT_STATUS_AUDIT)

Se conserva todo el historial anterior. Informe: [FUNCTIONAL_PROTOTYPE_VALIDATION.md](../Archive/2026-11-10/Context/FUNCTIONAL_PROTOTYPE_VALIDATION.md). Prueba física pendiente: [HONOR_TEST_CHECKLIST.md](../Archive/2026-11-10/Context/HONOR_TEST_CHECKLIST.md).

| ID | Estado vigente | Evidencia y alcance |
|---|---|---|
| N01 / K02 | HECHO | BancoCorresponsal primera y única escena habilitada. HelloCardboard y SampleScene conservadas como pruebas deshabilitadas; lista verificada en disco y build. |
| N02 / K07–K16 | HECHO para pruebas automatizadas Unity/PC | GameplayFlowProbe PASS: omisión y final real de 12 s, ambos modos, comparación y tres decisiones 6/6. BankOfficeProbe PASS: cinco funciones, nueve roles con raycasts y cuatro destinos. PrototypeSmokeProbe PASS: dwell/latch/rearme. No certifica visor ni revisión humana de controles PC. |
| Corrección de interacción | HECHO en Play Mode | El collider de computadora bloqueaba los botones; UIManager coloca el panel por delante en runtime. Raycasts desde inspección llegan a Aprobar/Retener/Devolver. Escena guardada intacta. |
| N03 / K19 | HECHO para generación APK; K19 EN_PROGRESO | Nueva APK 48851914 bytes, Unity Succeeded, 0 errores/1 advertencia de símbolos de diagnóstico, firma v2 válida. APK anterior conservada. Instalación/ejecución en teléfono pendientes. |
| N04 / K04 / K19 | PENDIENTE | Instalar APK nueva en HONOR y verificar banco, estéreo, sensores, QR/recenter, pausa/reanudación, audio y final. |
| N07–N09 / K07–K16 | PENDIENTE en dispositivo | Mirada física, cuatro pads, cinco funciones, tres decisiones y ambos resultados. No cerrar por las pruebas PC. |
| N05 / K01 | BLOQUEADO | Continúan ausentes las pestañas particulares completas del escenario 5. |
| N06 / K03 | PENDIENTE | Revisar/versionar el conjunto de cambios del equipo en una sesión autorizada; no hubo commit, merge ni push. |

APK: `Artifacts/Functional-20261102-083259/SWIFT-BancoCorresponsal-current.apk`.
SHA256: `F59EA07AE5019C2C67A1B6025E9B8DB993DB7E67B7B2B4DD39012DD6C793FB6E`.
Logs, resultados y copia de seguridad: `Artifacts/Functional-20261102-083259/`.
Siguiente: N04, N07, N08 y N09 en el HONOR; después legibilidad/audio/rendimiento y material docente. No se añadieron niveles, modelos ni decoración.

## Documentos bancarios / Propuesta C — 2026-11-02

- HECHO en Editor: expediente y registro reconstruidos con UI/TMP, seguidos de alertas existentes, cuatro pasos de procedimiento, resultados y contexto/créditos. Se usaron las cinco imágenes únicamente como referencias de composición.
- HECHO en Editor: pruebas de la presentación sin desbordes/solapamientos de bloques, raycasts desde inspección, tres decisiones, consultas, anillo y activación de mirada a 1,5 s con bloqueo/rearme. Prólogo y cuatro teletransportes sin regresión observada. 27 checks de dominio PASS.
- Evidencia: [DOCUMENT_UI_IMPLEMENTATION.md](../Archive/2026-11-10/Context/DOCUMENT_UI_IMPLEMENTATION.md) y `Docs/Previews/DocumentUIReview/20261102T143810058Z/README.md`.
- PENDIENTE N03/K19 para este cambio: generar otra APK que incluya la UI nueva. La APK funcional documentada anteriormente sigue siendo válida como evidencia de aquella compilación, pero no contiene estos cambios posteriores.
- PENDIENTE N04/N07–N09: instalar esa futura APK en HONOR; leer las cinco pantallas en ambos ojos, probar retícula/anillo/1,5 s y todas las pestañas/decisiones. Medir comodidad angular, FPS, memoria y temperatura.
- PENDIENTE: aceptación visual del usuario y audio según ART_AUDIO_NEEDS.md. N05/K01 mantiene su bloqueo por material docente ausente. Sin commit/push.

## Artes restantes — 2026-11-02

- HECHO en Editor: único cuadro institucional mate en pared oeste, entre bienvenida y archivo, sin colliders ni cambios de mobiliario/teletransportes. Proporción original preservada.
- HECHO en Editor: prólogo con los dos individuales de impresora y un render real del banco/UI actual como tercer momento; UV sin franjas de relleno, proporción conservada, fade suave, TMP y Omitir. Se mantienen 3 × 4 s, cámara libre e inicio único del modo seleccionado.
- HECHO en Editor: revisión específica PASS, 27 checks de dominio PASS y nuevas ejecuciones PASS de GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. Las aserciones de estas sondas se conservaron; solo se añadió una opción para evitar cerrar el Editor durante la revisión.
- Evidencia: [ART_INTEGRATION_REPORT.md](../Archive/2026-11-10/Context/ART_INTEGRATION_REPORT.md), `Docs/Previews/ArtIntegrationReview/20261102T145831568Z/README.md` y `Artifacts/ArtIntegrationReview/`.
- PENDIENTE N03/K19: nueva APK con UI y artes actuales; no se compiló una APK durante esta tarea. La APK anterior permanece conservada.
- PENDIENTE N04/N07–N09/N12/N18: HONOR/Cardboard, legibilidad y confort del prólogo/cuadro, mirada física de 1,5 s, inicio único, botones/recorrido y mediciones térmicas/rendimiento. Sin validación Android ni aprobación visual del usuario.
- Recursos: no faltan imágenes imprescindibles; opcionales dos ilustraciones 16:9 sin relleno si se quiere más altura de imagen. Audio sigue como tarea separada. N05/K01 conserva su dependencia docente; sin commit, merge o push.


## UX VR y preparación de publicación — 2026-11-02

- HECHO en Editor: retícula con contraste, progreso de 1,5 s, confirmación y aviso de rearme; controles grandes y campos TMP; tutorial opcional de cinco pasos sin puntuación; audio por categorías y eventos, con tonos TEMP identificados.
- PASS: DomainTests 27, revisión UX, GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. Prólogo omitido/automático, ambos modos, decisiones, cuatro teletransportes y raycasts comprobados. Android NO probado.
- Escena guardada mediante Unity, referencias nativas, scripts y lightmaps comprobados; sin generadores de oficina. Respaldo externo conservado.
- N06/K03: conjunto revisado preparado para commit y push autorizados únicamente a feature/vr-visual-improvements. No se autoriza merge ni cambios en main; ver PULL_REQUEST_DRAFT.md y REPRODUCIBILITY.md.
- Pendientes N03/K19 y N04/N07–N09: APK nueva y validación HONOR con UI/artes/UX actuales. Sustituir audios TEMP y comprobar confort/rendimiento. El bloqueo docente N05/K01 se conserva.
- Evidencia: UX_GAMEPLAY_PROGRESS.md y Docs/Previews/UXGameplayReview/20261102T205325756Z/.

## Rediseño inmersivo de oficina — 2026-11-02

Se conserva el historial anterior. Rama local `review/environment-immersive`, base `1074fdd`. Esta nueva tarea no incluye commit, push ni merge.

| Tarea | Estado vigente | Evidencia / límite |
|---|---|---|
| E01 Entorno y circulación | HECHO en Editor | 16 × 14 m, cuatro cubículos, zonas de impresión/archivo/seguridad, red de navegación validada |
| E02 Consola de modos | HECHO en Editor | Dos acciones originales, foco/confirmación y selección por mirada de 1,5 s; prólogo preservado |
| E03 Puesto y UI Propuesta C | HECHO en Editor | Monitor con marco proporcional, expediente documental, observaciones separadas, iconos nativos y decisiones alcanzables |
| E04 Silla | HECHO en Editor | Sentarse/levantarse bajo fade, offset de pose compensado, rechazo de destino ocupado y bloqueo/restauración de pads |
| E05 Consulta y libreta | HECHO en Editor | TMP de referencia/datos/alertas existentes; libreta abre procedimiento, sin cambiar expediente ni revelar decisión |
| E06 Materiales e iluminación | HECHO en Editor | Familias compartidas y nuevo bake de tres lightmaps; URP/Cardboard intactos |
| E07 Validación | PASS Editor | 27 dominio, flujo, oficina, smoke y revisión específica; ver recibos UTC de la galería |
| E08 Exploración | HECHO en Editor | Cuatro destinos originales conservados y dos adicionales por el mismo sistema de mirada; retorno a entrada comprobado |
| E09 / N03 / K19 APK actual | PENDIENTE | La APK anterior no contiene este rediseño; generar archivo nuevo |
| E10 / N04 / N07–N09 / N12 HONOR | NO PROBADO | Estéreo, seguimiento, fundido binocular, poses, giro lateral, lectura, seis pads, ambos modos |
| E11 / N18 Rendimiento | PENDIENTE dispositivo | Profiler/Frame Debugger, batches, memoria de lightmaps, FPS y temperatura; inventario geométrico no es benchmark |
| E12 Recursos / aprobación | PENDIENTE | Audios finales y aceptación visual; no falta ningún modelo obligatorio. Texturas de roble/tejido opcionales |

Informes: [ENVIRONMENT_REDESIGN_PROGRESS.md](../Archive/2026-11-10/Context/ENVIRONMENT_REDESIGN_PROGRESS.md) y [ENVIRONMENT_ASSET_NEEDS.md](ENVIRONMENT_ASSET_NEEDS.md). Galería: `Docs/Previews/EnvironmentRedesign/20261102T212047822Z/README.md`. El bloqueo docente N05/K01 se conserva.


## Ampliaci?n arquitect?nica real ? 2026-11-03

Se conserva el historial anterior; este registro actualiza E01?E12 para el estado de esta sesi?n.

| Tarea | Estado | Evidencia / pendiente |
|---|---|---|
| EA01 Arquitectura | HECHO Editor | Suelo 16 ? 14 ? 20 ? 18 m; paredes/techo/entrada extendidos sin escalar muebles; +136 m? |
| EA02 Circulaci?n | HECHO Editor | 140,625 ? 260,4375 m? conectados con entrada, c?psula/ret?cula id?nticas; cuatro cub?culos asim?tricos y seis zonas |
| EA03 Consola | HECHO Editor | Carcasa s?lida baja e inclinada 25?, originales ATAQUE/DEFENSA y dwell/pr?logo preservados |
| EA04 Puesto | HECHO Editor | UI horizontal 1400 ? 800, consulta derecha, libreta y silla; decisiones por raycast verificadas |
| EA05 Navegaci?n | HECHO Editor | Cuatro destinos originales exactos + dos previos de cub?culos + Reception/Rear Gallery; clearance y retorno |
| EA06 Luz | HECHO Editor | 12 baked + una realtime existente; bake renovado y 75 sondas para planta ampliada |
| EA07 Validaci?n | PASS Editor | Dominio 27, flow/office/smoke, revisi?n espec?fica y capturas reales; ver informe/recibos finales |
| EA08 Aceptaci?n visual | PENDIENTE usuario | Capturas revisadas; no equivalencia literal con referencias ni aceptaci?n autom?tica |
| EA09 / N03 / K19 APK | PENDIENTE | Generar APK nueva con logs/hash sin sobrescribir la antigua |
| EA10 / E10 HONOR | NO PROBADO | Ocho pads, lectura binocular, silla/fade, giro a consulta, pose/recenter y ambos modos |
| EA11 / N18 Rendimiento | PENDIENTE Android | 27.572 tri?ngulos y 270 MeshRenderer activos son inventario, no FPS/draw calls |
| EA12 Recursos | Sin modelo obligatorio | Texturas opcionales y audio final; especificaciones en ENVIRONMENT_ASSET_NEEDS |

Informe: [EXPANDED_BANK_OFFICE_REPORT.md](../Archive/2026-11-10/Context/EXPANDED_BANK_OFFICE_REPORT.md). Galer?a real: `Docs/Previews/ExpandedBankOffice/20261103-OperationsRoom/README.md`. Sin commit/push/merge; main intacta. El bloqueo docente N05/K01 permanece.


## Pulido art?stico profesional ? 2026-11-03

Se conserva todo el historial anterior.

| Tarea | Estado | Evidencia / l?mite |
|---|---|---|
| VP01 Orientaci?n y apoyos | HECHO Editor | Cinco sillas originales corregidas seg?n eje real; sexta de supervisi?n; monitores/teclados/archivo, macetas y libros apoyados |
| VP02 Zonas y cierre | HECHO Editor | Recepci?n agrupada y terminal End integrada en escritorio; rechazo de turno incompleto y gaze/latch verificados |
| VP03 Materiales/arquitectura | HECHO Editor | Familias reconocibles, cuatro texturas peque?as propias, remates y techo combinados; planta 20 ? 18 sin ampliar |
| VP04 Luz | HECHO Editor | Bake final 17 baked + 1 realtime, 81 sondas y reflexi?n baked 128?; sin cambios URP/XR |
| VP05 Navegaci?n | HECHO Editor | Ocho destinos/colliders conservados; aros con foco, sin temporizador nuevo; regreso/clearance |
| VP06 Assets/licencias | REVISADO | AVAILABLE_3D_ASSETS: siete ZIP; cinco mallas duplicadas recuperables por fuente individual, 120 pendientes de licencia |
| VP07 Validaci?n/capturas | PASS Editor | Dominio 27, flow/office/smoke/entorno y revisi?n espec?fica; galer?a Before/After/Play/Profiles |
| VP08 Aceptaci?n art?stica | PENDIENTE usuario | Revisar comparaci?n real; no equivalencia literal con referencias |
| VP09 APK/HONOR | NO EJECUTADO | APK nueva, lectura binocular, silla/fade/pose, aros, ambos modos y recenter |
| VP10 Rendimiento | PENDIENTE Android | Contadores globales Editor y geometr?a no son perfilado del dispositivo; Frame Debugger/GPU/memoria/temperatura pendientes |

Informe: VISUAL_POLISH_REPORT.md. Galer?a: Docs/Previews/VisualPolishReview/20261103-BankPolish/README.md. Sin commit/push/merge; main intacta. No hay modelos imprescindibles faltantes. Audio final y bloqueo docente N05/K01 conservan su estado.


## Graphics Polish V2 ? 2026-11-03

Se conserva todo el historial anterior. Revisi?n aislada por concurrencia sin confirmar; principal sin modificar.

| Tarea | Estado | Evidencia / l?mite |
|---|---|---|
| GPV2-01 Materiales | HECHO en copia | 18 materiales compartidos; madera desaturada, metal, pl?stico, tela, papel y arquitectura |
| GPV2-02 Remates | HECHO en copia | Techo 1,2 m, alf?izares, plintos, columnas y marco de entrada; misma planta 20 ? 18 |
| GPV2-03 Luz y exterior | PASS Editor | 17 baked + 1 realtime, bake propio, ciudad opaca de bajo coste geom?trico |
| GPV2-04 Capturas | HECHO | 45 PNG reales; siete parejas Before/After y UI Play, hashes y README |
| GPV2-05 Funcionalidad | PASS Editor | Dominio 27/27, flow/office/smoke/entorno/Play V2, acciones/colliders/destinos id?nticos al origen |
| GPV2-06 Integraci?n | PENDIENTE concurrencia | BancoCorresponsal_GraphicsV2 en Scenes/Review; principal byte por byte conservada; men? preparado con guard de hashes |
| GPV2-07 Aceptaci?n visual | PENDIENTE usuario | Est?tica low-poly, aliasing y contraste por revisar; no equivalencia literal con referencia |
| GPV2-08 APK / HONOR / rendimiento | NO EJECUTADO | Integrar, validar principal, compilar APK nueva y medir legibilidad, CPU/GPU, overdraw y temperatura |

Informe: [GRAPHICS_POLISH_V2_REPORT.md](../Archive/2026-11-10/Context/GRAPHICS_POLISH_V2_REPORT.md). Galer?a: ../Previews/GraphicsPolishV2/README.md. No se cambian URP/Cardboard/reglas. Sin commit/push/merge. No requiere nuevos recursos obligatorios.


## 2026-11-03 - Portal de operaciones / dwell real

Se conserva el historial anterior. Rama review/portal-dwell-feedback; sin commit/push.

| Tarea | Estado | Evidencia |
|---|---|---|
| PORTAL-01 Barras independientes conectadas al dwell | PASS Editor | Image.fillAmount = VRInteractionManager.DwellProgress; sin contador adicional |
| PORTAL-02 Cancelar, cambiar tarjeta, latch y regreso | PASS Editor | Raycasts reales, 1.5 s, dos modos y teletransporte |
| PORTAL-03 Presentacion y prefab modular | IMPLEMENTADO | Canvas/TMP, banco/escudo UI, marco y acciones originales; respaldo externo |
| PORTAL-04 Capturas y regresion | PASS Editor | 0/50/100 %, DomainTests 27/27, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe |
| PORTAL-05 Aceptacion artistica y HONOR | PENDIENTE | Lectura binocular, comodidad y rendimiento; no APK en esta tarea |

Informe: [PORTAL_OPERATIONS_REPORT.md](../Archive/2026-11-10/Context/PORTAL_OPERATIONS_REPORT.md). Capturas: ../Previews/PortalOperationsReview/README.md. Cierre nativo del Editor documentado; reapertura y pruebas finales satisfactorias.


### 2026-11-03 - Correccion de carga uniforme del portal

Fallo PC reproducido: hover acumulaba dwell oculto y el clic confirmaba antes de cargar. Corregido: mantener boton izquierdo 1.5 s en tarjetas; soltar/cambiar reinicia. Reticula y barra comparten contador. Mirada Cardboard conserva dwell; prologo limpia carga real. Escena y prefabs conservados. PASS Editor: regresion de raton, mirada, GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. Capturas PC 0/49.204/0/100 en Docs/Previews/PortalOperationsReview/UniformFix-20261103. Informe: Docs/Context/PORTAL_UNIFORM_FILL_FIX.md. Respaldo: D:/UnityGames/SWIFT_Backups/PortalUniformFill-20261103-210524. HONOR pendiente. Sin commit/push.


### 2026-11-03 - Prologo cercano con seguimiento de camara

Corregido el storyboard fijo a 2.5 m que quedaba detras de la consola. Ahora: distancia preferida 1.1 m, acercamiento ante geometria, seguimiento de posicion y recentrado angular suave con margen para seleccionar Omitir. Canvas del portal oculto durante el prologo y restaurado al terminar. PASS Unity: giro 55 grados y desplazamiento, encuadre completo, TMP, consola cercana, omision con dwell real 1.5 s, flujo con final automatico, oficina y smoke. Cuatro capturas reales y hashes en Docs/Previews/IntroCameraFollow. Informe: Docs/Context/INTRO_CAMERA_FOLLOW_FIX.md. Escena y prefabs sin guardar ni modificar; cambios pendientes preservados. Respaldo: D:/UnityGames/SWIFT_Backups/IntroCameraFollow-20261103-213513. HONOR pendiente; sin APK, commit ni push.


### 2026-11-03 - Afinado visual fuerte del portal

Rama review/portal-dwell-feedback, sin commit/push. Marco recortado de doble borde cian y halo UI, cabecera SWIFT destacada, sello institucional, facetas y gradientes, iconos de mayor presencia, titulos/descripciones ampliados, modulos de ojo y barras redondeadas de 48 unidades. Mismos objetos de progreso, acciones, colliders y dwell. Etiqueta duplicada de reticula oculta solo en el portal; punto/anillo conservados. PASS Unity: visual/TMP, mirada, raton, activacion unica, flow, office y smoke. Ocho PNG reales con hashes: Docs/Previews/PortalVisualPolish/20261103. Referencia comparada: REF_Portal_Operations.png. Aproximacion de composicion, sin equivalencia literal ni aceptacion artistica final. Seis scripts core/interaccion y 25 ProjectSettings identicos al respaldo. Respaldo completo y copia de los cambios sin guardar del Editor: D:/UnityGames/SWIFT_Backups/PortalVisualPolish-20261103-224008. Informe: Docs/Context/PORTAL_VISUAL_POLISH.md. HONOR/Android, lectura binocular y rendimiento pendientes.


### 2026-11-03 ? Afinado final del portal y pr?logo modal

Faceta ?nica, iconos con contornos unidos, estados de foco/progreso/confirmaci?n m?s claros y l?nea inferior vinculada al mismo dwell. Pr?logo ampliado con marco institucional y omisi?n m?s legible; portal activo al 48 % detr?s del modal y restaurado al terminar. PASS Editor: mirada, rat?n, activaci?n ?nica, silla real, TMP de los tres momentos, omisi?n y final autom?tico, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe. DomainTests 27/27. Capturas nuevas: Docs/Previews/PortalFinalReview/20261103. Informe: Docs/Context/PORTAL_FINAL_REVIEW.md. Respaldo: D:/UnityGames/SWIFT_Backups/PortalFinal-20261103-234535. Mismas reglas bancarias, URP y Cardboard; sin commit/push. Aceptaci?n visual del usuario y HONOR pendientes; sin APK.


### 2026-11-04 - Estacion inmersiva y reposo activo de PC

Implementados cuatro paneles bancarios con identidad compartida de REF_Operations_Expediente_Final, documento marfil, monto/observaciones, VER MAS/VOLVER y paginacion. Entrada contextual por el monitor, misma silla/fade, un SALIR DE ESTACION separado de CERRAR TURNO, pantallas detalladas ocultas de pie y teletransportes bloqueados sentado/restaurados al salir. Reentrada conserva sesion, expediente y pestana. Reposo corporativo en ambos monitores, UI luminosa y una luz real sin sombras (A/B comprobado). PASS Editor: ciclo completo por mirada y PC, cuatro pestanas, decisiones, textos, valores largos, paginacion, portal, GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. DomainTests 27/27. Informe: Docs/Context/OPERATIONS_STATION_FINAL.md. Galeria: Docs/Previews/OperationsStationFinal/20261104. Respaldo: D:\UnityGames\SWIFT_Backups\OperationsStation-20261104-004925. 45 fuentes/ajustes protegidos identicos. Sin commit/push, APK no generada. Aceptacion artistica del usuario, lectura binocular, comodidad y rendimiento HONOR pendientes.


### 2026-11-04 - Refinamiento visual de las tres pantallas

Reposo principal con jerarquia centrada, una instruccion y cuatro apps contenidas; documento mas respirado con beneficiario/cuenta en columnas, monto compacto, observaciones reducidas y decisiones con acentos propios. Secundario organizado en tres tarjetas de apoyo; reposo secundario simplificado. Mismos estados, dwell, silla y reglas bancarias. PASS Editor: antes/despues con cuatro poses coincidentes, ciclo completo de mirada y PC, textos, decisiones, flow, office y smoke. 49 archivos protegidos identicos, incluida BancoCorresponsal. Informe: Docs/Context/STATION_VISUAL_REFINEMENT.md. Capturas: Docs/Previews/StationRefinement/20261104. Respaldo: D:\UnityGames\SWIFT_Backups\StationRefinement-20261104-080938. Sin commit/push/APK. Aceptacion visual y HONOR pendientes.

### 2026-11-04 - Acabado institucional del expediente

Aplicado papel gris frio, banda petroleo, campos agrupados con bordes discretos, monto crema/ambar y notas coherentes. Solo presentacion de Dossier en BankDocumentUI; fuentes y espacios conservados. PASS Editor final: captura comparativa con cuatro poses coincidentes, ciclo completo UI por mirada, decisiones ataque/defensa, TMP, valores largos y entrada/salida PC. Fallo inicial de espera conservado y repeticion completa PASS. Seis archivos protegidos con SHA identicos, incluida escena e interaccion. Informe: Docs/Context/DOSSIER_VISUAL_TREATMENT.md. Galeria: Docs/Previews/DossierTreatment/20261104. Respaldo: D:\UnityGames\SWIFT_Backups\DossierTreatment-20261104-114515. Sin commit/push/APK. HONOR y aceptacion visual pendientes.

### 2026-11-10 — Herramientas inmersivas verificadas en Editor

Integrados en BancoCorresponsal mediante perfil reversible: portapapeles TMP paginado y marcas manuales; teléfono de prueba exclusivo DEFENSA/SIM-003 con registro único/contexto seguro; documento de solo lectura; feedback de impresora/silla; nuevos cues en el gestor de audio existente; revisión posterior de elección original frente a estado efectivo y fuentes. Segundo monitor, reglas, URP y Cardboard conservados. Copia aislada y respaldo externo fechados, sin commit/push.

PASS reales: DomainTests 27/27, DeskToolsTests 37/37; portapapeles 109, teléfono 88 y objetos 38 comprobaciones en principal; PC 14 y tutorial real 43 en copia aislada; GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe en arranques limpios de principal; revisión educativa 42. Historial de fallos corregidos e invalidación del primer recibo de tutorial conservados. No se equipara existencia de scripts con verificación.

Android: PASS build Development IL2CPP/ARM64, 0 errores/1 aviso, APK nueva 67.526.759 bytes en Artifacts/ImmersiveGameplay/Build-20261110-083825; hash/logs guardados. HONOR, lectura binocular, pausa/tracking físico, FPS/calor y aceptación del usuario siguen NO PROBADOS. Guion SIM-003 y clips finales pendientes. Próximas prioridades: prueba física, aprobación editorial, integrar audio licenciado y medir rendimiento antes de ampliar mecánicas. Informe: Docs/Context/IMMERSIVE_AUTONOMOUS_PROGRESS.md. Matriz: IMMERSIVE_IMPLEMENTATION_TRACKER.md. Galería: Docs/Previews/ImmersiveGameplay/README.md.

## Seguimiento de la segunda ronda autónoma — 2026-11-10

No se cambian los estados globales K04/K07/K09/K12/K14/K18/K19 porque su aceptación en HONOR sigue pendiente. Evidencia nueva en [SECOND_AUTONOMOUS_POLISH_REPORT.md](SECOND_AUTONOMOUS_POLISH_REPORT.md) y [galería Play Mode](../Previews/SecondAutonomousPolish/README.md).

- K12/K18: expediente con datos largos sin desbordes en la sonda, resultados separados por acción, estado, control y fuentes; 20 avisos repetidos dejaron de crear 780 RectTransforms transitorios. Falta lectura binocular y audio final.
- K07/K14: tutorial, impresora, ratón PC, portapapeles, teléfono y objetos pasaron tras corregir la asociación visual de la impresora y la sonda de un objetivo destruido. Falta uso físico Cardboard.
- K19: DomainTests 27/27; flow, office, smoke, regression, clipboard, phone, objects, tutorial, stationpc y caso visual de estrés PASS en Editor. No se compiló ni se probó APK en esta ronda.
- Siguiente gate: ejecutar APK en HONOR y medir rendimiento, lectura, comodidad y audio; no marcar HECHO con las capturas monoscópicas.

## Seguimiento de pulido de oficina — 2026-11-11

- Cierre físico, rótulos, libros y estantes: integrados en BancoCorresponsal tras comparar ocho vistas reales. Detalles y respaldo en [OFFICE_DETAIL_POLISH_REPORT.md](OFFICE_DETAIL_POLISH_REPORT.md).
- Monitor principal: CERRAR TURNO ampliado y verificado por raycast/dwell en Editor. La última decisión sigue cerrando automáticamente el turno conforme a la lógica actual.
- Validación pendiente: Cardboard en HONOR, lectura binocular, distancia de la terminal de supervisión desde los teletransportes y FPS/calor. No se marcan como completadas las tareas que dependen del dispositivo.

## Controles de ciberseguridad y demo — 2026-11-11

- P0 Cardboard: escena, paquetes, loaders, URP y reglas bancarias conservados. PASS Editor flow/office/smoke; HONOR físico pendiente.
- P1/P2 explicación: matriz de vulnerabilidades y controles, guion de demo con mismas decisiones, y estado real de revisión reforzada en resultados TMP. DomainTests 37/37, DeskToolsTests 37/37 y regression Editor PASS. Ver Docs/Research/CYBERSECURITY_MODE_COMPARISON.md y ACADEMIC_DEMO_FLOW.md.
- P3/P4: teléfono SIM-003 y portapapeles pasaron de nuevo en Editor; guion telefónico sigue siendo prototipo editorial y no modifica pérdidas. Piloto educativo y óptica HONOR pendientes.
- P5–P8 Quest: estudio de simuladores y diseño de laboratorio en Docs/Research/QUEST_CONTROLLER_LAB.md. Sin XRI/OpenXR en manifest, escena Quest ejecutable, simulador ni hardware probado; esperar modelo del visor y gate Cardboard físico antes de instalar paquetes en aislamiento.
