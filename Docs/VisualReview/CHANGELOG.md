# Registro de revisiones visuales

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

## 2026-11-03 — Biblioteca de materiales y estudio neutro

Preparación en review/environment-immersive / base1074fdd. Catálogo aislado: 12 materiales URP/Lit reutilizados, dos por familia; un gris neutro nuevo. Escena MaterialLibrary/Showcase/MaterialLibrary_Studio.unity, índice JSON, cubemap neutro 128 persistente y helper de Editor. Siete capturas finales reales; informe MATERIAL_LIBRARY_REPORT.md. Las iteraciones iniciales quedan solo en disco local.

Las comprobaciones se realizaron durante la preparación anterior: 1058 archivos previos de Assets/Packages/ProjectSettings intactos. No se ejecutan nuevas pruebas Unity ni Android para este commit. BancoCorresponsal, URP y Cardboard no se modifican.

Commit local autorizado por el usuario en la rama nueva feat/material-library-catalog. Se incluyen los 12 materiales y tres texturas compartidas necesarios para resolver referencias, con sus metas. Los demás cambios locales de la oficina siguen sin versionar. Sin push ni merge.
