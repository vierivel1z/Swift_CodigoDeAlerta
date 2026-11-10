> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Documentos bancarios — Propuesta C

Fecha: 2026-11-02. Unity 6000.3.24f1, URP 17.3.0, proyecto local. Estado: implementado y comprobado en Editor; Android/visor y aprobación visual del usuario pendientes.

## Resultado

Se priorizaron Expedientes y Registros. Tras su primera revisión PASS en Play Mode se completaron Alertas, Procedimiento y Resultados. La presentación se reconstruye con Unity UI y TextMeshPro en el Canvas World Space existente. Ningún PNG vertical se usa como pantalla completa ni como plantilla de casillas para superponer datos.

| Pantalla | Referencia en Assets/_Project/Resources/DocsImgs | Implementación |
|---|---|---|
| Expediente | Imagen de ChatGPT 2 nov 2026, 09_25_19-1.png | Seis bloques: emisor, monto, beneficiario, cuenta, intermediario y fecha si existe. Identificador, modo y estado arriba. Observaciones existentes debajo. |
| Registro | Imagen de ChatGPT 2 nov 2026, 09_25_21-3.png | Cuenta consultada, beneficiario del expediente, titular registrado, estado y respuesta literal de InspectRegistry. Sin titular inventado cuando la cuenta no consta. |
| Alertas | Imagen de ChatGPT 2 nov 2026, 09_25_20-2.png | Filas Campo / Observación / Estado a partir de las anomalías existentes y del texto de TrainingData.Alerts. Coincidencia parcial sigue siendo una observación por revisar. |
| Procedimiento | Imagen de ChatGPT 2 nov 2026, 09_25_21-4.png | Cuatro tarjetas de lectura, contraste, documentación y decisión; títulos/textos editables en UIManager.ReviewStepTitles/ReviewStepTexts y renderizados con TMP. |
| Resultados | Imagen de ChatGPT 2 nov 2026, 09_25_22-5.png, solo inspiración | Tarjetas de decisiones coherentes, pérdida ficticia, evidencias, comparación ataque/defensa y seis decisiones realizadas. Explicación educativa y acceso a contexto histórico/créditos. |

Estilo: azul marino, acentos cian, documentos claros, tinta oscura y bloques con separación. Los botones APROBAR / RETENER / DEVOLVER siguen siendo los objetos originales con sus acciones y colliders, grandes e independientes. Pestañas de consulta permiten revisar desde el monitor; archivador/alertas físicos conservan las consultas existentes y abren su presentación correspondiente.

## Archivos de esta implementación

Modificados:

- `Assets/_Project/Scripts/UI/UIManager.cs`: instala la presentación, conserva la corrección KeepPanelInFrontOfComputer y expone los cuatro textos de procedimiento.
- `Assets/_Project/Scripts/Interaction/BankInteractable.cs`: rutas de presentación después de las consultas existentes; acciones de navegación añadidas al final del enum para preservar valores serializados.
- `Assets/_Project/Scripts/Interaction/VRInteractionManager.cs`: propiedades de solo lectura para foco, progreso, tipo de entrada y bloqueo de selección. Algoritmo de activación sin cambios.

Nuevos, con sus `.meta` generados por Unity:

- `Assets/_Project/Scripts/UI/BankDocumentUI.cs`: vistas y datos de presentación.
- `Assets/_Project/Scripts/UI/DocumentFocus.cs`: resaltado de foco.
- `Assets/_Project/Scripts/UI/GazeProgressVisual.cs`: anillo de progreso sin collider; reutiliza el shader de la retícula y no cambia URP.
- `Assets/_Project/Editor/DocumentUIReview.cs`: revisión local bajo demanda, capturas y comprobaciones de Play Mode. No guarda escenas ni cierra el Editor.

Documentación/evidencia: este informe, actualización de KANBAN y CHANGELOG, capturas y manifiesto en `Docs/Previews/DocumentUIReview/20261102T143810058Z/`, registros en `Artifacts/DocumentUIReview/`.

## Protección de los avances

- Se revisó git status antes de editar. Rama existente: `review/unity-scene-designer`, base `3bef77be8ea73637f37e150b4884d67905609735`. Había numerosos cambios locales previos; no deben confundirse con los archivos de esta tarea.
- Respaldo externo completo de Assets, Packages, ProjectSettings y Docs: `D:\UnityGames\SWIFT_Backups\DocumentUI-20261102-093519`.
- La restauración debe ser selectiva: comparar el archivo con su equivalente bajo ese respaldo y recuperar únicamente el archivo deseado, conservando antes su versión actual. No reemplazar el proyecto completo ni los cambios posteriores del usuario.
- 144 archivos protegidos comparados por SHA256 contra ese respaldo: todos idénticos. Recibo: `Artifacts/DocumentUIReview/protected-files.json`. Incluye la escena bancaria, prefabs revisados, PNG de referencia, dominio bancario, GameManager, escenario, configuraciones de proyecto/paquetes y assets revisados de Cardboard/URP.
- SHA256 de BancoCorresponsal.unity: `41467B15703BFF8B588C7F36C6A76C684483B1F44CA3A315074961B59C3C3D92`.
- No se sobrescribió ningún prefab, no se reconstruyó la oficina y no se hizo commit, push, merge, reset ni clean.

## Pruebas ejecutadas

### Dominio .NET

`dotnet run --project Tools/DomainTests/DomainTests.csproj`: **27 comprobaciones PASS**, salida 0. Conserva las reglas, datos ficticios, cálculo de pérdidas, controles reforzados, puntuación, evidencias y resumen histórico.

### Unity Editor / Play Mode

Revisión final: **PASS**, sin errores runtime registrados durante la prueba. Resultados y comprobaciones: `Artifacts/DocumentUIReview/final-editor-result.txt`, `checks.txt`, `domain-result.txt`. El registro acumulado conserva también iteraciones anteriores; la revisión final se identifica por fecha y manifiesto.

- Expediente y Registro con las seis transacciones del escenario guardado.
- Consulta con titular coincidente, nombre diferente y cuenta no encontrada; consulta original conservada.
- Todos los TMP activos del monitor sin desbordes en los estados probados.
- Rectángulos de bloques hermanos sin solapamientos en las vistas comprobadas; revisión visual manual de los ocho PNG finales.
- Physics.Raycast desde inspección llega a los tres botones originales y las pestañas habilitadas; el collider de computadora no los tapa. Acceso a contexto/créditos comprobado por raycast al terminar.
- Aprobar / Devolver / Retener mediante BankInteractable: turno coherente de **6/6**, pérdida ficticia 0.
- Aprobar todo en ataque: pérdida **USD 965.000**. Defensa con controles y aprobar todo: **USD 0**. No se reescriben los cálculos. En este último ejemplo la puntuación permanece **1/6**, pues los controles corrigen cinco decisiones del jugador; no debe maquillarse como 6/6.
- Cámara real del rig con SimulateGazeInEditor: foco por raycast, progreso visible antes de activación, sin activar antes del umbral, activación con DwellSeconds **1,5 s**, una sola activación mientras se mantiene el foco y reinicio al apartar la mirada. Se restaura cámara/entrada al terminar.
- Retícula central conservada, anillo cian y resaltado del control enfocado. Captura adicional del progreso en Editor.
- Prólogo: omisión y final automático con su duración normal aproximada de **12 s**.
- Cuatro destinos de teletransporte conservados y activados; restauración de posición después de cada prueba.

El Canvas conserva 1100 × 800 y escala 0,0015: unos **1,65 × 1,20 m**. Desde inspección (0; 1,60; −0,65), la distancia perpendicular al panel corregido es aproximadamente 1,47 m: unos **59° horizontales × 44,5° verticales**. Es una estimación geométrica; no acredita comodidad, nitidez estereoscópica ni tamaño percibido dentro del HONOR. No se redujo el panel para acomodar textos.

Los probes antiguos que cierran todo el Editor no se ejecutaron otra vez en esta sesión. Se ejecutaron las pruebas de dominio existentes y el nuevo revisor de documentos dentro del Editor abierto. No confundir el PASS previo del prototipo con pruebas adicionales de esta interfaz en Android.

## Capturas reales

Carpeta: `Docs/Previews/DocumentUIReview/20261102T143810058Z/`.

1. `01-expediente-anterior.png`: interfaz real antes de modificar la presentación.
2. `02-expediente-mejorado.png`: campos separados y controles originales.
3. `03-consulta-registros.png`: resultado de la consulta existente.
4. `04-panel-alertas.png`: SIM-006, coincidencia parcial sin conclusión de fraude.
5. `05-procedimiento.png`: cuatro pasos.
6. `06-resultados.png`: defensa con comparación ataque/defensa y decisiones efectivas.
7. `07-contexto-creditos.png`: contexto original y atribuciones.
8. `08-progreso-mirada-editor.png`: anillo y foco durante selección por mirada simulada.

Son renders reales de Unity en Play Mode, mediante una cámara temporal de revisión a 1600 × 1000. No son capturas del HONOR ni de los dos ojos de Cardboard. No se guardó esa cámara en la escena. `capture-receipt.txt` registra Unity, calidad y posición; `capture.json` registra hashes y fechas. `iteration-1/` conserva la primera revisión antes de ampliar etiquetas y alertas.

## Pendiente Android VR / siguientes tareas

1. Generar una APK nueva con estos scripts: la APK funcional anterior **no contiene** esta interfaz. No se generó APK durante esta tarea de UI.
2. Instalarla en el HONOR y verificar las cinco pantallas dentro de Cardboard: leer cuenta, beneficiario, intermediario y monto sin acercarse ni forzar la vista; comprobar bordes y contraste en ambos ojos.
3. Comprobar retícula y anillo cian: avance durante 1,5 s, confirmación una vez y reinicio al mirar fuera. Llegar a cada pestaña y a las tres decisiones desde el pad de inspección sin oclusión del monitor.
4. Revisar los seis expedientes, discrepancia de nombre, cuenta ausente y coincidencia parcial; confirmar que esta última no aparece como fraude confirmado.
5. Completar ambos modos, revisar comparación, decisiones, contexto/créditos y repetir un turno. Comprobar que no quedan controles inaccesibles ni textos solapados en estéreo.
6. Medir FPS, memoria y temperatura durante 10 minutos y comprobar recenter, pausa/reanudación y comodidad angular. Sin medición de dispositivo en esta sesión.

No hay errores bloqueantes observados en la revisión final del Editor. Siguen pendientes la compilación Android de esta UI, pruebas físicas y aceptación visual del usuario; no se marcan como completadas.
