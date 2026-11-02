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

Se conserva todo el historial anterior. Informe: [FUNCTIONAL_PROTOTYPE_VALIDATION.md](FUNCTIONAL_PROTOTYPE_VALIDATION.md). Prueba física pendiente: [HONOR_TEST_CHECKLIST.md](HONOR_TEST_CHECKLIST.md).

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
- Evidencia: [DOCUMENT_UI_IMPLEMENTATION.md](DOCUMENT_UI_IMPLEMENTATION.md) y `Docs/Previews/DocumentUIReview/20261102T143810058Z/README.md`.
- PENDIENTE N03/K19 para este cambio: generar otra APK que incluya la UI nueva. La APK funcional documentada anteriormente sigue siendo válida como evidencia de aquella compilación, pero no contiene estos cambios posteriores.
- PENDIENTE N04/N07–N09: instalar esa futura APK en HONOR; leer las cinco pantallas en ambos ojos, probar retícula/anillo/1,5 s y todas las pestañas/decisiones. Medir comodidad angular, FPS, memoria y temperatura.
- PENDIENTE: aceptación visual del usuario y audio según ART_AUDIO_NEEDS.md. N05/K01 mantiene su bloqueo por material docente ausente. Sin commit/push.

## Artes restantes — 2026-11-02

- HECHO en Editor: único cuadro institucional mate en pared oeste, entre bienvenida y archivo, sin colliders ni cambios de mobiliario/teletransportes. Proporción original preservada.
- HECHO en Editor: prólogo con los dos individuales de impresora y un render real del banco/UI actual como tercer momento; UV sin franjas de relleno, proporción conservada, fade suave, TMP y Omitir. Se mantienen 3 × 4 s, cámara libre e inicio único del modo seleccionado.
- HECHO en Editor: revisión específica PASS, 27 checks de dominio PASS y nuevas ejecuciones PASS de GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. Las aserciones de estas sondas se conservaron; solo se añadió una opción para evitar cerrar el Editor durante la revisión.
- Evidencia: [ART_INTEGRATION_REPORT.md](ART_INTEGRATION_REPORT.md), `Docs/Previews/ArtIntegrationReview/20261102T145831568Z/README.md` y `Artifacts/ArtIntegrationReview/`.
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
