# Progreso autónomo inmersivo — SWIFT: Código de Alerta

10/11/2026. Unity 6000.3.24f1, URP 17.3.0, Android/Cardboard. Cambios integrados localmente. Sin commit, push, merge ni publicación. La aprobación visual del usuario y las pruebas físicas siguen pendientes.

## Resultado y alcance

Se implementaron portapapeles inspeccionable, teléfono contextual de prueba DEFENSA/SIM-003, inspección del documento físico, feedback de impresora/silla, disparadores de audio y revisión educativa posterior. Se conservan el segundo monitor, el único raycast/dwell de 1,5 s, el fade original y las reglas bancarias. BancoCorresponsal no se reemplazó ni se rehízo la oficina.

Leídos íntegramente: [VR_GAMEPLAY_RESEARCH](../Research/VR_GAMEPLAY_RESEARCH.md), [IMMERSIVE_GAMEPLAY_DESIGN](../Research/IMMERSIVE_GAMEPLAY_DESIGN.md), [CARDBOARD_FEASIBILITY](../Research/CARDBOARD_FEASIBILITY.md) y [CODEX_IMPLEMENTATION_ROADMAP](../Research/CODEX_IMPLEMENTATION_ROADMAP.md), además del contexto y la auditoría del 8/11. Las tareas y dependencias se consolidaron en [IMMERSIVE_IMPLEMENTATION_TRACKER](IMMERSIVE_IMPLEMENTATION_TRACKER.md).

Ya existían portal funcional, retícula con progreso, prólogo, tutorial, monitores activo/reposo, asiento con fade, destinos, decisiones, evidencia y cinco categorías de audio. Se reutilizaron; no se presentan como sistemas nuevos. La libreta física antes abría Procedimiento y no tenía una inspección documental propia. El teléfono contextual no existía.

## Portapapeles

Sentado, enfocar el objeto de madera existente y completar el dwell abre una vista world-space a la izquierda. No se fija rígidamente a la cabeza ni sustituye el monitor secundario. Su posición se ajustó para mostrar cabecera y botones sin que la mesa o el propio portapapeles bloquearan el rayo.

Tres hojas: formato/procedimiento, datos del expediente y observaciones/fuentes. Las marcas son manuales, independientes por expediente/sesión; no validan ni puntúan. No se muestran Recommended ni SimulatedFraud. Los datos largos tienen páginas TMP reales antes de avanzar de hoja, con cuerpo de 36 puntos. ANTERIOR/SIGUIENTE, MARCAR y VOLVER usan el mismo VRInteractionManager. Cerrar conserva el objeto y la estación. Perfil ausente mantiene el comportamiento anterior Notebook→Procedure.

Tamaño nominal: panel de aproximadamente 1,05 × 0,84 m a 1,48 m del ojo, unos 39° × 32°; el cuerpo equivale aproximadamente a 1,5° por em. Es geometría de Editor, no certificación óptica del visor. Las dimensiones cercanas equivalentes de la llamada conservan un tamaño angular de lectura similar.

## Teléfono SIM-003

Prefab nativo ligero de tres meshes, aproximadamente 204 triángulos, con plástico/metal ya existentes. Ubicado a la derecha del escritorio, separado del rayo a los papeles. Solo habilitado sentado, en DEFENSA y SIM-003 pendiente; no depende de activar Defense.Active ni aparece como consulta válida en otros expedientes.

Flujo: SOLICITAR → señal pendiente → RESPONDER → tres intervenciones TMP → REGISTRAR → cerrar/reabrir. El collider primario conserva identidad al cambiar de etiqueta: sostener la mirada tras solicitar no contesta automáticamente y tras registrar no duplica la evidencia. Apartar la mirada rearma el dwell existente.

Catálogo separado `Assets/_Project/DeskTools/Prototypes/SIM003_CallPrototype.asset`. El texto de Astra era propuesto, por lo que se muestra GUION DE PRUEBA y la evidencia lo indica. No se inventó una identidad real. El nombre comunicado usa RegisteredName del dominio; el jugador lo contrasta con el beneficiario original SIM-003. No se indica Aprobar/Retener/Devolver ni se confirma fraude o autorización. **Falta aprobación editorial del guion final.**

La evidencia se registra mediante el API existente, con identidad de sesión + objeto de expediente + ID y deduplicación. Cambiar de expediente/sesión invalida la consulta anterior. NoAnswer e Inconclusive se probaron como estados puros .NET; no se añadieron como nuevos expedientes o diálogos de entrega.

## Objetos y estación

- Documento físico existente: inspección de solo lectura con datos reales de la simulación; conserva sus reglas y su posición.
- Impresora: foco/tinte compartido y placa local de estado. Confirmación solo tras una evidencia nueva de la acción de impresora. Dos selecciones por mirada producen una entrada; una llamada no enciende esa confirmación. No se inventa una impresión histórica ni se anima una evidencia falsa.
- Silla: respuesta visual de 3,5 cm del mesh sin collider dentro del fade original. No se modificaron AnalystSeat, HeadPoseAdapter, destinos, cámara ni tracking. Disparador Chair preparado.
- SALIR DE ESTACIÓN cierra inspección, restaura reposo y destinos, conserva expediente y permite reentrada con ambos monitores.
- Guarda modal común: bloquea botones bancarios ocultos incluso con Activate directo; salir sigue permitido. Un único sistema de raycast y un único reloj de selección.

## Audio

Se extendieron FeedbackAudio y AudioCueLibrary existentes, conservando UI, Objects, Alerts, Ambience y Music. Nuevas referencias: PhoneRing, PhoneAnswer, PhoneEnd, PageTurn, Chair y Printer. El timbre tiene una fuente controlada dentro del mismo gestor, detenida al responder/cerrar/pausar. No se añadió otro manager ni se necesitó un AudioMixer nuevo.

Los eventos nuevos quedan silenciosos sin clip final; los ocho tonos TEMP anteriores conservan su carácter provisional. Diez actualizaciones de mensaje no generan spam. No se descargó ni generó audio. Especificaciones y prompts: [IMMERSIVE_AUDIO_ASSET_REQUEST](IMMERSIVE_AUDIO_ASSET_REQUEST.md). La mezcla audible y el comportamiento del timbre real quedan pendientes de clips y pruebas de dispositivo.

## Feedback educativo y visual

DecisionCommitted es un evento descriptivo emitido después de que el dominio acepte la decisión. El diario auxiliar registra elección original, estado efectivo, intervención del control, fuentes abiertas y evidencia telefónica. No cambia cálculos. Resultados muestran, por ejemplo, «Aprobar → Devuelta (control)»; la revisión posterior explica cada expediente y pagina el texto. Abrir una fuente no significa haberla leído o validado.

Tratamiento institucional reutilizado: petróleo, cian, gris papel, TMP y marcos existentes; sin bloom, shaders nuevos, materiales duplicados ni texturas nuevas. Foco de objetos mediante MaterialPropertyBlock. No se modificó la iluminación para mejorar artificialmente una captura.

## Archivos afectados

| Grupo | Archivos / alcance |
|---|---|
| Nuevos runtime | `Scripts/DeskTools/DeskReviewState.cs`, `DeskToolsProfile.cs`, `VerificationCallCatalog.cs`, `DeskActionTarget.cs`, `DeskInspectionView.cs`, `DeskObjectFeedback.cs`, `SeatPresentationFeedback.cs`, `DeskInteractionCoordinator.cs` y sus .meta |
| Runtime existente | `Core/GameManager.cs` (evento); `Interaction/BankInteractable.cs` (guardas/despacho); `VRInteractionManager.cs` (elegibilidad); `UI/UIManager.cs` (perfil opt-in); `BankDocumentUI.cs` (resumen/revisión); `Audio/AudioCueLibrary.cs` y `FeedbackAudio.cs` (cues) |
| Recursos nuevos | `Assets/_Project/DeskTools/DeskTools_Trial.asset`, catálogo Prototypes, tres prefabs y tres meshes de teléfono, con .meta generados por Unity |
| Aislamiento | `Scenes/Review/BancoCorresponsal_Immersive_20261110.unity`, creada mediante AssetDatabase.CopyAsset y GUID independiente |
| Integración | `Scenes/BancoCorresponsal.unity`: referencia UIManager.DeskTools, dos valores predeterminados de FormatReferencePanel serializados por Unity y metadatos predeterminados UniversalAdditionalLightData de la luz existente Main monitor - subtle desk spill. Intensidad y configuración global sin cambios; diff exacto en Artifacts |
| Verificación | `Editor/ImmersiveEditorBridge.cs`, `Scripts/Testing/ImmersivePlayProbe.cs` (solo UNITY_EDITOR), `Tools/DeskToolsTests/` y comprobador estático local |
| Documentación | Matriz, este informe, checklist de dispositivo, solicitud de audio, galería; historial Kanban/changelog y créditos conservados mediante anexos |

Las rutas de scripts son relativas a `Assets/_Project/`. El perfil conserva el nombre Trial para identificar la función de prueba; PhoneEnabled/AllowPrototypeCalls permiten desactivar el catálogo sin tocar el dominio. No se sobrescribió ningún prefab anterior.

## Pruebas reales

| Prueba | Resultado / alcance |
|---|---|
| DomainTests | PASS 27/27, .NET real |
| DeskToolsTests | PASS 37/37, contexto, estados, tokens, duplicados, marcas y cálculos |
| Compilación estática con DLL instaladas | PASS 0 errores; complementaria, no sustituye Unity |
| Portapapeles | PASS Editor, principal: 109 comprobaciones; raycast, 0/50/100, mirar fuera, latch, páginas, datos largos, modal, retorno |
| Teléfono | PASS Editor, principal: 88 comprobaciones; elegibilidad, encuadre inicial, estados, registro único, contexto y reglas intactas |
| Objetos/asiento | PASS Editor, principal: 38 comprobaciones; documento, impresora por dwell real, duplicados, salir/reentrar, tracking habilitado y destinos |
| PC | PASS Editor, copia aislada: 14 comprobaciones; hover cero, media carga, soltar cero y hold único |
| Tutorial | PASS Editor, copia aislada: 43 comprobaciones reales; cinco pasos, latch, completar, reabrir y omitir sin sesión bancaria |
| GameplayFlowProbe | PASS Editor en arranque limpio de la principal: omisión, final automático real de 12 s, ambos modos, tres decisiones y 6/6 |
| BankOfficeProbe | PASS Editor en arranque limpio de la principal: estructura, fuentes, acciones alcanzables y destinos existentes |
| PrototypeSmokeProbe | PASS Editor en arranque limpio de la principal: fuentes/licencias, ambos modos, destinos y dwell/latch/reset |
| Regresión educativa | PASS Editor, principal: 42 comprobaciones; ataque 965000 y 1/6, defensa automática 0 y 1/6, estado frente a elección y revisión alcanzable |
| HONOR/Cardboard físico | NO PROBADO; `adb devices -l` vacío |
| Rendimiento/temperatura/binocularidad | NO PROBADO; no se deduce de PNG ni del Editor |

`Artifacts/ImmersiveGameplay/test-matrix.json` y carpetas por ejecución contienen checks/summary. Los nueve recorridos finales no registraron nuevos errores runtime en su ventana de observación; esto no significa borrar ni declarar vacía la Console histórica. Se conservaron los errores de intentos anteriores y los avisos del build.

## Fallos encontrados y corregidos

Se conservan en result-history.txt: primer runner situado bajo Editor no podía adjuntarse como MonoBehaviour; guardas de escena sucia evitaron descartar cambios; cabecera cortada y rayos bloqueados por portapapeles/mesa; captura Game View tomada un fotograma tarde; panel telefónico fuera del campo inicial; teléfono sobre el rayo al documento; confirmación de impresora vinculada erróneamente a cualquier evidencia; prueba PC proyectando con cámara hacia arriba; precargas residuales de build detectadas por comparación de hash. La primera guarda de limpieza se negó a actuar porque el API ya devolvía una lista vacía; se inspeccionó antes de restaurar la serialización exacta.

El primer recibo de tutorial PASS se invalidó porque el dispatcher no había ejecutado ese método; se añadió rechazo de modos desconocidos y se repitió el recorrido real con 43 comprobaciones. Las sondas antiguas se separaron en arranques limpios porque exigen sesión inicial nula. La prueba educativa se corrigió para recorrer páginas TMP antes de cambiar de expediente. No se debilitaron los criterios de producción para convertir fallos en PASS.

El controlador de ventanas y Node fallaron al inicializar el sandbox. Se leyeron las skills Unity/PC y se usaron APIs nativas del Editor ya abierto mediante órdenes locales en archivos; no hay servidor remoto, plugin nuevo ni automatización de ratón por PowerShell. El usuario realizó Refresh y después el runner permitió refrescos explícitos. Capturas y Play son reales; no se atribuyen a una herramienta de ventanas que no funcionó.

## Capturas inspeccionadas

[Galería y comparativa](../Previews/ImmersiveGameplay/README.md). Renders URP antes/después con pose coincidente, Game View de portapapeles/llamada/tutorial, estados 0/≈50/100, documento, impresora, salida/reentrada y resultados. Se conservaron capturas de la copia aislada y las iteraciones fallidas. Los recibos identifican la escena; hashes en capture-hashes.json.

También se abrieron las cuatro imágenes reales de `Docs/Previews/20261110T083754509Z-0bfe48`. Son edición, con textos serializados anteriores a Play; no muestran el teléfono runtime. La cenital cubre el núcleo y oculta temporalmente techo, sin prometer una auditoría completa de extremos de oficina. Ninguna captura certifica equivalencia artística con una referencia aprobada.

## Protección y reversión

Rama existente `review/portal-dwell-feedback`, HEAD base c4a1278. Cambios locales importantes anteriores preservados. Respaldo: `D:/UnityGames/SWIFT_Backups/Immersive-20261110-020437`, con escena/meta, fuentes y manifest inicial de 41 archivos; ruta en backup-path.txt. No se ejecutó reset, clean ni checkout. No hubo commit/push.

31 archivos protegidos (ProjectSettings, Packages, meta principal, OperationsStation y AnalystSeat) coinciden por SHA-256 con el respaldo. BankingDomain no tiene diferencias Git; HeadPoseAdapter no fue modificado y su componente quedó habilitado en las pruebas. No se atribuye una comparación de respaldo a un archivo que no esté en el manifest. Diff de escena: `Artifacts/ImmersiveGameplay/scene-diff-vs-backup.txt`. Se utilizó Undo y EditorSceneManager; no se editó YAML de escena a ciegas. Para desactivar sin restauración destructiva, quitar el perfil DeskTools del UIManager o sus flags en una copia de revisión. Restaurar archivos selectivamente desde el respaldo solo después de revisar cambios posteriores; no copiar todo el backup encima de avances nuevos.

## Android

**PASS: APK de desarrollo generada con Unity real, IL2CPP/ARM64.** Resultado Succeeded, 0 errores y 1 advertencia; duración 13 min 34,9 s. AppVersion existente 0.1.0; identificador del artefacto Immersive-v1-20261110. No se cambió la versión de configuración para fabricar un número nuevo.

Ruta exacta: `D:/UnityGames/SWIFT_CodigoDeAlerta/Artifacts/ImmersiveGameplay/Build-20261110-083825/SWIFT-Immersive-v1-20261110.apk`.

Archivo final: **67.526.759 bytes** (aprox. 64,4 MiB). SHA-256 verificado independientemente:

`CBC7D66B2E980E7B6A76C392F82DA913215E4DB8FE19DF56BF3AE94D64FD4CEB`

Logs/reportes en esa carpeta: build-report.txt, unity-build.log, unity-editor-build.log, SHA256.txt y file-verification.txt. El totalSize de BuildReport incluye archivos de compilación y no se presenta como tamaño del APK; arriba figura el tamaño real en disco. Artefactos de símbolos/debug permanecen separados. La advertencia indica Diagnostics Data habilitado con símbolos incompletos para resolver stacktraces; se conservó la configuración existente. La eliminación de variantes de Rendering Debugger es un mensaje informativo. No se ocultaron avisos.

La compilación añadió temporalmente referencias preloadedAssets de Input/XR existentes. Los postprocesadores restauraron la lista vacía en memoria, pero dejaron dos referencias serializadas en disco. Se inspeccionó el API y se restauró únicamente ese bloque conocido, exigiendo que el SHA-256 del archivo completo resultante coincidiera con el respaldo inicial; no se alteraron otros campos ni los loaders. El APK ya generado conserva las dependencias que necesita. protected-files-final.json registra la verificación posterior. No hay instalación, arranque Android o medición de rendimiento real; es una APK Development y la aceptación en HONOR sigue pendiente.

Configuración Android existente conservada. BancoCorresponsal permanece primera y habilitada; SampleScene y HelloCardboard siguen presentes, deshabilitadas como pruebas. Build explícito de la principal; sin sustituir APK anteriores. Sin dispositivo conectado: no instalación, logcat de ejecución ni aprobación HONOR.

## Dependencias, riesgos y siguientes tareas

1. Probar APK en HONOR con [IMMERSIVE_DEVICE_CHECKLIST](IMMERSIVE_DEVICE_CHECKLIST.md): lectura binocular, herramientas, llamada, pausa, recentrado, entrada/salida, destinos y rendimiento sostenido.
2. Aprobar el guion definitivo SIM-003; el catálogo actual es explícitamente de prueba. Extender llamadas a otros casos requiere diseño aprobado y nuevas pruebas.
3. Proporcionar clips según la solicitud de audio; revisar mezcla y localización al integrarlos. Voz es opcional y siempre con subtítulos.
4. Ajustar altura/posición únicamente a partir del visor. Giro hacia herramientas, escala y placa de impresora necesitan validación física; componentes habilitados no prueban tracking real.
5. Medir iluminación/materiales/picos CPU/GPU y memoria en Android antes de más efectos. No se hicieron cambios globales para embellecer renders. Persistencia entre cierres de app no se añadió; el diario es de la sesión actual.
6. Identificar hardware de manos del salón antes de escoger proveedor XR. Mantener una variante separada del Cardboard; no hay manos articuladas falsas ni SDK instalado.
7. Piloto breve de comprensión con 3–5 estudiantes, con observación y preguntas; no presentarlo como evidencia estadística. Posponer sello obligatorio, físicas libres, manos sin hardware y eliminación del segundo monitor.

No se necesitan nuevas imágenes o modelos para que estas mecánicas funcionen. Las mejoras gráficas posteriores deben partir de las capturas y medición real, manteniendo materiales compartidos, luz horneada y legibilidad; arte IA solo si cubre una necesidad identificada, con texto dinámico en TMP.