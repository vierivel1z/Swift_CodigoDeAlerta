# Registro de revisiones visuales

## Dirección artística autónoma — 2026-11-10

- Escena BancoCorresponsal respaldada y variante aislada revisada en Unity 6000.3.24f1 antes de integrar. No se borraron escenas ni objetos interactivos.
- Reutilizados materiales GraphicsPolishV2 en 180 renderers; añadidas cinco mallas combinadas existentes, sin colliders; retocadas 15 luces horneadas y regenerado el lightmap. URP, Cardboard y reglas bancarias sin cambios.
- Nueve pares de capturas reales desde cámaras idénticas: [galería](../Previews/ArtDirectionPolish/README.md). Las iteraciones intermedias se conservan; se rechazó una captura aislada con importación incompleta.
- Pruebas de dominio 27/27 PASS; flujo, oficina y humo PASS tanto en variante como en principal; portapapeles y teléfono PASS en variante. Editor solamente; HONOR sin probar.
- Informe y coste preliminar: [ART_DIRECTION_AUTONOMOUS_REPORT.md](../Context/ART_DIRECTION_AUTONOMOUS_REPORT.md). Respaldo local en `Artifacts/ArtDirectionPolish/Backup-20261110-185745/`. Sin commit, push ni merge.

## Auditoría de arte y audio — 2026-11-02

- Petición: evaluar iluminación, materiales, colores, composición, Propuesta C, audio y assets futuros, sin implementar cambios de presentación.
- Rama/base: `review/unity-scene-designer`, `3bef77b`, con cambios locales previos conservados.
- Informe: [ART_AUDIO_NEEDS.md](../Context/ART_AUDIO_NEEDS.md), con gravedad, inventario propuesto, prompts y plan de implementación.
- Capturas nuevas reales del principal: [Before-20261102T140647380Z](../Previews/LightingReview/Before-20261102T140647380Z/README.md); cuatro vistas más iluminación, perfil PC, Unity 6000.3.24f1, Direct3D11. Inspeccionadas las cinco imágenes; la planta oculta temporalmente techo/luminarias.
- Escena capturada SHA256: `41467b15703bff8b588c7f36c6a76c684483b1f44ca3a315074961b59c3c3d92`. Cola local existente utilizada solo con inspect/before. Sin guardar ni regenerar escena, sin cambios de materiales, prefabs, luces, URP, XR o paquetes. Por no editar esos recursos, no hubo que reemplazarlos ni restaurarlos.
- Se contrastó con capturas anteriores y con `Artifacts/Functional-20261102-083259/office-inspection.png` para distinguir UI de Edición y UI runtime recolocada delante del collider.
- Inspección: un lightmap, siete spots baked activos, una direccional activa, una direccional antigua inactiva, un grupo de probes y cero reflection probes. Audio: solo tono runtime; no archivos de audio o mixer localizados en Assets. Sin audición en HONOR ni mediciones móviles nuevas.
- Evidencia local y comparación de hashes: `Artifacts/ArtAudioAudit-20261102/`. No se generaron recursos IA, se descargó audio o se implementaron las propuestas.
- Estado: diagnóstico entregado para revisión; dirección visual y correcciones propuestas pendientes de aprobación. Sin commit, push, PR ni merge.

## Preparación del flujo — 2026-10-29

- Rama local: `review/unity-scene-designer`.
- Petición: skill reutilizable, cuatro capturas reales y revisión desde el teléfono.
- Añadidos: skill, herramienta Editor de cámaras temporales, carpeta de previews, procedimiento GitHub e investigación MCP.
- Escena, prefabs, modelos, materiales, URP, XR y paquetes: sin cambios. No se requiere restaurar una escena porque no se modificó.
- Capturas: **pendientes; Unity no está instalado ni activado en esta nube**.
- Verificación en nube: 27 comprobaciones de dominio .NET aprobadas; enlaces relativos de siete documentos, frontmatter de skill y concordancia manifiesto/lock comprobados. Esto no valida compilación C# del Editor ni renders.
- Pendiente local: compilar herramienta, validar URP/render y encuadres, generar cuatro PNG y revisar legibilidad. MCP no instalado ni validado en ejecución.
- Publicación: sin push, PR ni merge automático.

## Documentos bancarios / Propuesta C — 2026-11-02

- Petición: reconstruir expediente, registro, alertas, procedimiento y resultados mediante UI/TMP a partir de cinco referencias; sin cambios de lógica bancaria, URP o Cardboard.
- Rama/base: `review/unity-scene-designer` / `3bef77be8ea73637f37e150b4884d67905609735`; se conservó el trabajo local previo sin commit ni push.
- Recuperación: copia externa `D:\UnityGames\SWIFT_Backups\DocumentUI-20261102-093519`. Restauración selectiva por archivo, conservando cambios posteriores.
- Implementación: tres scripts modificados (UIManager, BankInteractable, VRInteractionManager), tres componentes nuevos de presentación (BankDocumentUI, DocumentFocus, GazeProgressVisual) y revisor Editor DocumentUIReview. Sin escena/prefab/modelo/material persistente sobrescrito.
- Primero Expedientes/Registros: PASS antes de continuar con las otras vistas. Bloques claros, marino/cian y datos dinámicos TMP. Tres decisiones originales preservadas; anillo de progreso y foco.
- Evidencia específica de UI: `Docs/Previews/DocumentUIReview/20261102T143810058Z/` con anterior/mejorado, registro, alertas, procedimiento, resultados, contexto/créditos y progreso. Son renders reales de Play Mode desde inspección, revisados visualmente; no una nueva sesión de cuatro perspectivas de arquitectura ni capturas Android. Escena sin guardar/cambiar, SHA256 41467B15703BFF8B588C7F36C6A76C684483B1F44CA3A315074961B59C3C3D92.
- Pruebas: 27 checks de dominio PASS; revisión final Unity PASS (14:48:55–14:49:10 UTC), TMP, solapamientos de bloques, raycasts, consultas, decisiones 6/6, ataque 965000/defensa 0, mirada simulada 1,5 s con bloqueo/rearme, prólogo omitido/12 s y cuatro teletransportes. Sin errores runtime durante esa revisión.
- Integridad: 144 archivos protegidos idénticos al respaldo; recibo en `Artifacts/DocumentUIReview/protected-files.json`.
- Android pendiente: APK con esta interfaz, legibilidad de ambos ojos, comodidad angular, selección física y perfilado. No se generó APK durante esta tarea; no hay métricas de rendimiento del teléfono.
- Informe: `Docs/Context/DOCUMENT_UI_IMPLEMENTATION.md`. Estado: revisión de Editor completada, pendiente aceptación visual del usuario y pruebas en HONOR.

## Artes restantes / cuadro institucional y prólogo — 2026-11-02

- Petición: integrar ART_BankWallGraphic y prólogo ilustrado, conservando documentos, lógica, URP y Cardboard.
- Rama/base: `review/unity-scene-designer` / `3bef77be8ea73637f37e150b4884d67905609735`; cambios locales previos preservados. Sin commit, merge o push.
- Respaldo externo completo: `D:\UnityGames\SWIFT_Backups\ArtIntegration-20261102-095628`. Copia adicional de memoria conservada y comprobada idéntica a escena guardada antes de continuar.
- Escena mediante APIs Unity/Undo: único grupo nuevo `ART - Bank Wall Graphic`, posición (-4,84; 1,95; 0,60), marco/quad sin colliders, imagen 2,20 × 1,238 m. IntroSequence añadido a sistemas con referencias de tres imágenes. No se movieron objetos originales ni se cambiaron luces/lightmaps/prefabs.
- Materiales nuevos URP Lit mate, emisión desactivada. Importadores de tres PNG ajustados a NPOT sin reescalado y Clamp; archivos PNG originales intactos.
- Prólogo: dos individuales con UV que excluye relleno; tercer momento usa render real de Play Mode con UI actual, en lugar de oficina conceptual diferente. Fade de ilustración, títulos/subtítulos/Omitir estables; cámara sin animación, 3 × 4 s.
- Capturas: `Docs/Previews/ArtIntegrationReview/20261102T145831568Z/`: cinco solicitadas, cuatro perspectivas adicionales de Edición, README/manifiesto y primeras iteraciones conservadas. Revisadas visualmente. La vista superior oculta techo temporalmente; vistas de Edición muestran el monitor serializado, mientras las de Play muestran su presentación actual.
- Pruebas nuevas: 27 dominio PASS; revisor de arte PASS; GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe PASS. Sin errores de compilación observados ni errores runtime en validaciones finales. Se conserva log histórico con intentos cancelados/diagnósticos de coordinación, sin afirmar Console históricamente vacía.
- Integridad: 838 originales comparados, cero eliminados y ocho cambios esperados; ningún cambio en documentos/PNG, dominio bancario, GameManager, prefabs, paquetes o configuración URP/Cardboard.
- Pendiente: APK nueva, pruebas HONOR, nitidez/estéreo/confort/rendimiento y aceptación visual del usuario. No faltan recursos imprescindibles; 16:9 sin relleno opcional para ilustraciones de mayor altura.
- Informe: `Docs/Context/ART_INTEGRATION_REPORT.md`. Estado: implementación y revisión Editor completas; Android y aprobación del usuario pendientes.

## Plantilla para la siguiente revisión

```text
Fecha / autor:
Petición / comentario:
Rama / commit base:
Estado recuperable y cómo restaurarlo:
Escena / prefabs / materiales / modelos afectados:
Cambios realizados:
Commit de escena capturado / sesión Docs/Previews:
Comparación de las cuatro perspectivas con los criterios:
Pruebas ejecutadas / resultado:
Perfil activo / métricas Android antes y después:
Problemas pendientes / capturas que faltan:
Estado: borrador / cambios solicitados / aprobado por el usuario:
```


## UX VR y preparación de publicación — 2026-11-02

- HECHO en Editor: retícula con contraste, progreso de 1,5 s, confirmación y aviso de rearme; controles grandes y campos TMP; tutorial opcional de cinco pasos sin puntuación; audio por categorías y eventos, con tonos TEMP identificados.
- PASS: DomainTests 27, revisión UX, GameplayFlowProbe, BankOfficeProbe y PrototypeSmokeProbe. Prólogo omitido/automático, ambos modos, decisiones, cuatro teletransportes y raycasts comprobados. Android NO probado.
- Escena guardada mediante Unity, referencias nativas, scripts y lightmaps comprobados; sin generadores de oficina. Respaldo externo conservado.
- N06/K03: conjunto revisado preparado para commit y push autorizados únicamente a feature/vr-visual-improvements. No se autoriza merge ni cambios en main; ver PULL_REQUEST_DRAFT.md y REPRODUCIBILITY.md.
- Pendientes N03/K19 y N04/N07–N09: APK nueva y validación HONOR con UI/artes/UX actuales. Sustituir audios TEMP y comprobar confort/rendimiento. El bloqueo docente N05/K01 se conserva.
- Evidencia: UX_GAMEPLAY_PROGRESS.md y Docs/Previews/UXGameplayReview/20261102T205325756Z/.

## Rediseño inmersivo — 2026-11-02

- Petición: ampliar oficina, cuatro cubículos, consola compartida de modos, puesto con consulta/libreta, silla de mirada y UI Propuesta C, manteniendo lógica bancaria y XR.
- Rama/base: `review/environment-immersive` / `1074fdd313eac8220451f11559e6076ba4fb4e2c`. Sin commit, push ni merge; main intacta.
- Respaldo completo: `D:/UnityGames/SWIFT_Backups/EnvironmentRedesign-20261102-161323`, con checkpoints posteriores. No se borraron recursos ni se ejecutaron generadores anteriores.
- Escena BancoCorresponsal editada mediante APIs Unity/Undo, GUID conservado. Sala 16 × 14 m, cuatro cubículos y corredores; cuatro destinos originales conservados y dos adicionales con el mismo teletransporte.
- Prefabs originales intactos; dos nuevos de monitor/lámpara normalizados desde Kenney existente. Familias de materiales compartidas; paneles de consulta/clipboard mediante UI/TMP.
- Consola de modos reutiliza las dos acciones originales y el dwell de 1,5 s. Silla mueve el origen del rig bajo fade opaco y conserva HeadPoseAdapter; bloquea pads y WASD al sentarse.
- UI mantiene KeepPanelInFrontOfComputer, tres botones y cálculos. Referencias de formato/observaciones sin instrucciones directas de decisión por transferencia; núcleo bancario sin cambios.
- Bake final: tres lightmaps, nueve luces baked y una direccional realtime existente. 26.004 triángulos / 177 MeshRenderer / 39 materiales de malla distintos: inventario Editor, no medición Android.
- Galería real: `Docs/Previews/EnvironmentRedesign/20261102T212047822Z/`, antes/después comparables, planta, consola, cubículos, pie/sentado, monitores/libreta y LEVANTARSE. Iteraciones anteriores conservadas. Capturas PC de Edición y Play, no IA ni estéreo Android.
- PASS: 27 dominio, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe y revisión específica de consola/silla/rutas/clearance/TMP/offset/consulta. Dependencias, scripts y lightmaps comprobados; recibos UTC en galería.
- Integridad: 67 archivos protegidos idénticos al respaldo, incluyendo paquetes, URP/XR, prefabs originales, GameManager/dominio/pose/prólogo y PNG originales.
- Pendientes: aceptación estética del usuario, APK actualizada, HONOR/Cardboard, confort, lectura binocular, pausa/recenter y perfilado. Audio final pendiente; no falta ningún modelo imprescindible.
- Informes: `Docs/Context/ENVIRONMENT_REDESIGN_PROGRESS.md` y `ENVIRONMENT_ASSET_NEEDS.md`. Estado: implementado y revisado en Editor; sin aprobación visual/hardware.


## Ampliaci?n real ? 2026-11-03

- Petici?n: aumentar arquitectura y circulaci?n, usar referencias confirmadas directamente y conservar jugabilidad Cardboard. Skill aplicada: `.agents/skills/unity-scene-designer/SKILL.md`.
- Rama `review/environment-immersive`, base 1074fdd313eac8220451f11559e6076ba4fb4e2c. Main sin cambios; sin commit, staging, push ni merge.
- Recuperaci?n: `D:/UnityGames/SWIFT_Backups/ExpandedBankOffice-20261103-004324`, copia inicial completa y checkpoints de escena/meta/lightmaps. No se descartan cambios previos.
- Planta real 16 ? 14 ? 20 ? 18 m; +136 m?. Circulaci?n conectada aproximada 140,625 ? 260,4375 m?, m?todo de c?psula/ret?cula documentado. No se escala el conjunto de la escena.
- Cuatro cub?culos en grupos asim?tricos; seis zonas, ventanas opacas, divisi?n posterior de supervisi?n. Dos destinos adicionales, cuatro originales exactos y dos de cub?culos conservados.
- Consola baja de cu?a inclinada; puesto horizontal con consulta a la derecha y TMP din?mico. Silla, fade, mirada 1,5 s, KeepPanelInFrontOfComputer y reglas bancarias preservados.
- Cinco materiales compartidos y malla propia de consola; sin recursos externos ni prefabs originales sobrescritos. Iluminaci?n renovada: 12 baked + una realtime activa y 75 sondas; segundo bake tras corregir cobertura oscura.
- Capturas reales de Unity Edici?n/Play: `Docs/Previews/ExpandedBankOffice/20261103-OperationsRoom/`. Primera iteraci?n oscura conservada. Comparaci?n antes/despu?s con c?mara id?ntica y vistas adicionales, no renders IA ni estereoscop?a Android.
- PASS dominio 27, flujo/oficina/smoke y revisi?n espec?fica. Recibos antes, despu?s y finales en `Artifacts/ExpandedBankOffice/`; estado de captura en README/manifest de galer?a.
- Inventario: 27.572 tri?ngulos, 270 MeshRenderer, 45 materiales de malla distintos. No se midieron draw calls/FPS Android ni se certifica optimizaci?n.
- Informe: `Docs/Context/EXPANDED_BANK_OFFICE_REPORT.md`. Pendiente aceptaci?n visual del usuario, APK nueva, HONOR, confort/lectura/seguimiento/perfilado y audio final. No hay modelo adicional obligatorio.


## Pulido art?stico profesional ? 2026-11-03

- Solicitud: corregir orientaci?n, decoraci?n y cierre antes del pulido, usando tres capturas recibidas y referencias aprobadas. Skill unity-scene-designer aplicada.
- Rama/base: review/environment-immersive / 1074fdd. Main intacta, sin commit/push/merge ni staging. Respaldo completo VisualPolish-20261103-130232 y checkpoints por fase.
- Escena 20 ? 18 conservada. Cinco sillas corregidas seg?n geometr?a (+Z respaldo), sexta de supervisi?n, teclados/monitores/archivos orientados, plantas/espera agrupadas y superficies apoyadas. Cierre End existente integrado en escritorio/monitor de supervisi?n.
- Nuevos usos verificados de desk/bookcaseOpen Kenney; no se extrae Office Pack no autorizado. AVAILABLE_3D_ASSETS documenta siete ZIP, 145 fuentes Models y cuatro de Cardboard; 120 mallas pendientes de procedencia/licencia.
- Familias de superficies con patrones procedurales propios peque?os; z?calos/cornisa/ret?cula/perfiles combinados. Aros de navegaci?n reemplazan solo presentaci?n, mantienen ocho destinos/colliders; ArchitecturalFocusVisual consume el gestor de mirada existente.
- Puesto con teclado, rat?n y libreta delante del monitor, tama?o TMP conservado. Sentarse, stand, pose, fade y KeepPanelInFrontOfComputer intactos.
- Horneado final PASS: 17 baked + 1 realtime, 81 sondas, una reflexi?n baked 128?. Sin cambios de paquetes, URP/XR, fuentes/importadores o prefabs descargados. 488 archivos protegidos id?nticos al inicio.
- PASS: dominio 27, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe y revisi?n de entorno; revisi?n espec?fica final de seis sillas, cierre por mirada/turno incompleto, raycasts sentados, TMP y levantarse. Fallos de herramientas intermedios conservados y resueltos; no se confunden con resultados finales.
- Galer?a real: Docs/Previews/VisualPolishReview/20261103-BankPolish; Before/After con ocho c?maras equivalentes, detalle 07 corregido por oclusi?n, Play y dos perfiles existentes. Capturas mono PC, no Android ni IA.
- Inventario final: 284 MeshRenderer / 49 materiales distintos / 31.360 tri?ngulos activos (31.228 habilitados). Contadores globales Editor registrados con l?mites; Frame Debugger/Profiler Android no ejecutados.
- Pendiente: aprobaci?n visual del usuario, APK nueva, HONOR, lectura/confort/fade/pose, rendimiento y audio final. No hay modelo adicional imprescindible. Informe VISUAL_POLISH_REPORT.md.


## 2026-11-03 ? Graphics Polish V2 (revisi?n aislada)

Petici?n: pulir materiales, arquitectura y luz conservando oficina 20 ? 18 y mec?nicas. Rama review/environment-immersive, base 1074fdd. Respaldado en D:/UnityGames/SWIFT_Backups/GraphicsPolishV2-20261103-135942. Varios procesos Codex; exclusividad sin confirmar: no se edita la principal.

Escena nueva Scenes/Review/BancoCorresponsal_GraphicsV2.unity con GUID propio y bake propio; biblioteca OfficeVisual/GraphicsPolishV2 con 18 materiales y remates agrupados; helper solo Editor/GraphicsPolishV2Review.cs. Madera desaturada, archivo met?lico, pl?sticos/textiles/papel diferenciados, techo modular, ventanas/columnas/mamparas acabadas, libros en escritorios y luz administrativa neutra con puesto c?lido. Sin descarga ni modelos de licencia pendiente.

Galer?a real: Docs/Previews/GraphicsPolishV2/README.md, 45 PNG, siete parejas id?nticas antes/despu?s, planta y UI Play. Hashes y recibos incluidos. Inventario 31.228 ? 32.718 tri?ngulos; 284 ? 288 renderers; 49 ? 54 materiales visibles. 17 baked/1 realtime/81 sondas/1 reflexi?n baked 128/1 atlas1024. Configuraci?n URP/XR intacta.

PASS Editor: dominio27, GameplayFlowProbe, BankOfficeProbe, PrototypeSmokeProbe, entorno, mirada/cierre, asiento/fade, TMP y raycasts. Acciones/colliders/destinos comparados con origen: id?nticos. 995 archivos previos Assets/Packages/ProjectSettings conservados. Integraci?n preparada con respaldo/hash, NO ejecutada. APK, Frame Debugger/overdraw, HONOR y aceptaci?n art?stica pendientes. Informe GRAPHICS_POLISH_V2_REPORT.md. Sin commit/push/merge.


## 2026-11-03 — Biblioteca de materiales y estudio neutro

Preparación en review/environment-immersive / base1074fdd. Catálogo aislado: 12 materiales URP/Lit reutilizados, dos por familia; un gris neutro nuevo. Escena MaterialLibrary/Showcase/MaterialLibrary_Studio.unity, índice JSON, cubemap neutro 128 persistente y helper de Editor. Siete capturas finales reales; informe MATERIAL_LIBRARY_REPORT.md. Las iteraciones iniciales quedan solo en disco local.

Las comprobaciones se realizaron durante la preparación anterior: 1058 archivos previos de Assets/Packages/ProjectSettings intactos. No se ejecutan nuevas pruebas Unity ni Android para este commit. BancoCorresponsal, URP y Cardboard no se modifican.

Commit local autorizado por el usuario en la rama nueva feat/material-library-catalog. Se incluyen los 12 materiales y tres texturas compartidas necesarios para resolver referencias, con sus metas. Los demás cambios locales de la oficina siguen sin versionar. Sin push ni merge.


## 2026-11-03 - Material library verification in Unity

Unity6000.3.24f1 / DX11, same local project, Mobile_RPAsset. Catalog reopen and short Play Mode PASS; zero errors/warnings captured during checks. Persistent custom cubemap128, 12 opaque URP/Lit samples, 48 shapes, TMP and rendering verified. Fourteen new Editor/Play captures: Docs/Previews/MaterialLibraryVerification/20261104T010447Z. UTC receipts cross midnight; local date remains November3. All1073 existing Assets/Packages/ProjectSettings files unchanged. Main bank scene untouched; banking probes and Android not tested in this scope. Report MATERIAL_LIBRARY_VERIFICATION.md. No new commit/push/merge.


## 2026-11-03 - Portal de operaciones: progreso de mirada verificable

- Rama review/portal-dwell-feedback desde c4a1278. Cambios locales anteriores preservados; sin commit/push/merge.
- Respaldo: D:/UnityGames/SWIFT_Backups/PortalOperations-20261103-203448 (Assets, Docs y git status).
- Reconstruccion UI/TMP de tarjetas, iconos acotados y prefab modular. Barras Filled/Horizontal leen el dwell existente; confirmacion posterior de 0.35 s antes del flujo normal. PC: G + camara/raton existente.
- Escena BancoCorresponsal y seis scripts existentes afectados; helper Editor y recursos nuevos en OfficeVisual/PortalOperations, con metas. 1043 assets anteriores identicos respecto al respaldo; siete archivos anteriores modificados, sin faltantes. Detalle Artifacts/PortalOperations/asset-delta.json.
- Galeria real: Docs/Previews/PortalOperationsReview (9 PNG, recibos y hashes). Referencia revisada: REF_Portal_Operations.png. Bordes/iconos simplificados, sin equivalencia literal ni aprobacion artistica final.
- PASS Editor: prueba especifica con raycasts y dwell de produccion, DomainTests 27/27, flow, office y smoke. Office actualizado para esperar la confirmacion de ambos modos.
- Incidencia: cierre nativo del Editor despues de una ronda PASS; log conservado. Se reabrio y repitieron verificaciones con exito. Android/HONOR, rendimiento y comodidad pendientes; no APK.
- Informe: Docs/Context/PORTAL_OPERATIONS_REPORT.md. No se alteraron URP, Cardboard, paquetes ni reglas bancarias.


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

### 2026-11-10 — Inspección de herramientas y revisión educativa

Petición: aplicar investigación de inmersión Cardboard por fases, sin reglas nuevas ni rehacer oficina. Rama existente review/portal-dwell-feedback, base c4a1278, cambios locales previos preservados. Respaldo D:/UnityGames/SWIFT_Backups/Immersive-20261110-020437 y escena aislada creada con GUID propio.

Se reutilizan madera/clip/papel, biblioteca de materiales, TMP y marcos institucionales. Teléfono procedural de tres meshes y tres materiales existentes. Vista de inspección world-space con páginas, texto gris papel y controles grandes; foco por MaterialPropertyBlock, breve respuesta visual de silla y estado de impresora. Sin nuevas luces, texturas o efectos. Unity serializó valores predeterminados de consulta y metadatos URP de la luz de escritorio existente; diff conservado, pipeline/intensidad intactos.

Capturas reales inspeccionadas: Docs/Previews/ImmersiveGameplay/README.md (Play/Game View y comparativas con poses) y Docs/Previews/20261110T083754509Z-0bfe48/README.md (cuatro vistas en edición). Cenital parcial del núcleo con techo temporalmente oculto; vistas de edición muestran textos anteriores al arranque, no UI dinámica final. Se corrigieron clipping, oclusión de botones y apertura fuera de campo antes de integrar. No se afirma coincidencia cinematográfica ni aprobación visual del usuario.

Pruebas reales de Unity: herramientas, PC, tutorial y sondas flow/office/smoke PASS; dominio 27/27 y pruebas de escritorio 37/37. Logs, fallos intermedios y resultados en Artifacts/ImmersiveGameplay. El primer PASS de tutorial fue invalidado y repetido correctamente. Herramienta de ventanas indisponible; se usaron APIs del Editor y se abrieron las capturas reales.

Android: PASS build Development IL2CPP/ARM64, 0 errores/1 aviso, APK nueva 67.526.759 bytes en Artifacts/ImmersiveGameplay/Build-20261110-083825; hash/logs guardados. Pendientes HONOR, óptica/lectura, rendimiento, clips y guion final. Revisión visual pendiente del usuario. Sin commit, push, merge ni publicación.


### 2026-11-10 — Segunda ronda de calidad y optimización UI

Caché de reconstrucción documental para avisos idénticos, monto largo y previews con elipsis; resultados de dos líneas con decisión original, estado efectivo, intervención y fuentes; revisión educativa compacta. Corrección de asociación de respuesta visual de la impresora. Capturas Play Mode antes/después en siete poses y caso adicional de evidencia: Docs/Previews/SecondAutonomousPolish/README.md. PASS Editor: DomainTests 27/27 y sondas flow, office, smoke, regression, clipboard, phone, objects, tutorial, stationpc y estrés visual. 20 avisos idénticos: 780 RectTransforms transitorios antes, 0 después. Fallos intermedios preservados. Informe: Docs/Context/SECOND_AUTONOMOUS_POLISH_REPORT.md. HONOR y perfilado móvil pendientes. Escena, URP y Cardboard no tocados en esta ronda; sin commit/push/APK nueva.

### 2026-11-11 — Pulido de detalles de oficina y cierre

Rótulos institucionales azul petróleo; libros flotantes asentados y estantería ocupada con prefab existente; pantalla física de supervisión y cierre ampliada; fila inferior del monitor con CERRAR TURNO más ancho y tipografía mayor; feedback de objetos suavizado sin cambiar activaciones. Escena respaldada y variante v3 revisada antes de integrar. Capturas reales antes/después desde ocho poses y Play Mode del monitor: Docs/Previews/OfficeDetailPolish/README.md. Sondas Editor de objetos, cierre, estación PC y oficina PASS; DomainTests 27/27. Primer fallo de la sonda de cierre debió a expectativa equivocada de final manual; estación PC pasó al repetir. Informe: Docs/Context/OFFICE_DETAIL_POLISH_REPORT.md. Falta evaluación binocular y rendimiento en HONOR. Sin commit/push/APK nueva.

### 2026-11-11 — Estado educativo de controles

La cabecera de resultados distingue ATAQUE sin revisión automática, DEFENSA sin activar y DEFENSA activa usando el estado real de GameSession. No cambian cálculos ni decisiones. Nueva prueba de dominio con elecciones idénticas: 5/6 en ambos, USD 20 000 ficticios frente a 0 con control activo. PASS .NET DomainTests 37/37 y DeskToolsTests 37/37; PASS Editor regression, flow, office, smoke, phone y clipboard. Capturas reales: Docs/Previews/CybersecurityReview/README.md. Matriz, laboratorio Quest y guion docente en Docs/Research. Cardboard físico/Quest/simulador XR NO PROBADOS; sin paquetes, loaders, escena principal, commit o push adicionales.

## 2026-11-11 — Portapapeles físico reversible

Variante física sobre el tablero de madera existente, con elevación/retorno, texto TMP sobre el papel, controles por el dwell único y acceso `AMPLIAR` a la vista anterior. Sin cambios a escena, prefab, URP, Cardboard o reglas bancarias. PASS Unity Editor: `physical-before`, `physical-after`, `objects`, `stationpc` y `regression`; Android/Cardboard físico pendiente. [Informe](../Context/PHYSICAL_CLIPBOARD_REPORT.md) y [capturas](../Previews/PhysicalClipboard/README.md). Sin commit/push.
