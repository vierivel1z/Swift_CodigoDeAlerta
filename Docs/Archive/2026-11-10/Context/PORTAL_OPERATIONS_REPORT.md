> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Portal de operaciones: barras de mirada reales

**Corrección posterior, 2026-11-03:** se corrigió la acumulación invisible al pasar el cursor en PC y se unificó la pulsación mantenida con la barra y retícula. Consultar [PORTAL_UNIFORM_FILL_FIX.md](PORTAL_UNIFORM_FILL_FIX.md) para comportamiento y verificaciones actuales. El resto de este informe conserva el historial de implementación inicial.

Fecha local: 2026-11-03 (Lima). Las evidencias UTC corresponden a 2026-11-04.
Rama: `review/portal-dwell-feedback`, base `c4a1278`. Sin commit ni push.

## Protección del trabajo

Se revisó `git status` y se conservaron los cambios anteriores. Respaldo completo de Assets y Docs, incluidos escena, prefabs y metas, en:

`D:\UnityGames\SWIFT_Backups\PortalOperations-20261103-203448`

Para recuperar un archivo, cerrar Play Mode y copiar solamente ese archivo y su meta desde el respaldo. No restaurar toda la carpeta sobre avances posteriores.

## Implementación

- Las barras originales eran geometría de color con apariencia llena. `ModeStationVisual` animaba sus lámparas por foco mediante `MoveTowards`; ese brillo no representaba el dwell.
- Se reconstruyó la presentación mediante Canvas World Space, TMP, dos tarjetas y formas UI pequeñas para banco y escudo/candado. Los elementos antiguos se conservan desactivados.
- Cada `Image` usa `Filled / Horizontal / Left` y lee directamente `VRInteractionManager.DwellProgress` en `LateUpdate`. No hay un segundo contador ni animación autónoma de progreso.
- Solo la tarjeta enfocada por el raycast físico recibe progreso. Apartar la mirada lo deja a cero. El cambio de objetivo y el latch siguen perteneciendo al sistema original de interacción.
- El borde de la tarjeta enfocada se ilumina por cambio de color, sin luces adicionales.
- Después de activar, una confirmación de 0,35 s precede a la llamada existente a `GameManager.EnterScenario`. Esa espera es exclusivamente de presentación y ocurre después del dwell.
- Los colliders existentes se conservan y se ajustan a las tarjetas, delante del marco. No se añadieron botones UI que compitan con Physics.Raycast.
- En PC, **G** alterna ratón y simulación de mirada desde la cámara; mantener botón derecho permite orientar la cámara con el adaptador existente. El cambio de entrada reinicia la fijación anterior. En Android no se ejecuta esa alternancia.
- Prefab reutilizable: `Assets/_Project/OfficeVisual/PortalOperations/PortalOperations.prefab`, con controlador, marco, Canvas, barras enlazadas y ambos BankInteractable. Los Manager de escena se conservan; una instancia nueva puede resolver el Manager al iniciar. La escena conserva su jerarquía local, sin forzar conexión al prefab ni reemplazar la oficina.
- Sprite blanco de 4 × 4 píxeles para el llenado UI, sin texturas grandes ni dependencias.

## Archivos de esta intervención

| Archivo | Cambio |
|---|---|
| `Scripts/UI/ModeStationVisual.cs` | Presentación ligada al dwell real y confirmación posterior |
| `Scripts/Interaction/BankInteractable.cs` | Los dos modos pasan por la confirmación visual antes de EnterScenario |
| `Scripts/Interaction/VRInteractionManager.cs` | Reinicio controlado al alternar entrada de escritorio; mismo raycast y contador |
| `Scripts/Interaction/EditorInputAdapter.cs` | Atajo G, conservando cámara y ratón |
| `Scripts/UI/ArchitecturalFocusVisual.cs` | Inicialización defensiva del MaterialPropertyBlock tras recarga de scripts |
| `Scripts/Testing/BankOfficeProbe.cs` | Espera de la nueva confirmación visual antes de comprobar el prólogo |
| `Editor/PortalOperationsReview.cs` | Reconstrucción mediante API de Unity, capturas y pruebas con raycasts reales |
| `Scenes/BancoCorresponsal.unity` | Presentación del portal y áreas de selección |
| `OfficeVisual/PortalOperations/` | Prefab y sprite blanco con metas |

Las rutas abreviadas de Assets parten de `Assets/_Project/`. El resto de cambios locales anteriores no forman parte de esta intervención.

## Verificación

Las pruebas específicas se ejecutan en Play Mode y orientan la cámara hacia los colliders reales, con `SimulateGazeInEditor=true`. No escriben `fillAmount` para fabricar los estados de captura: comparan el valor mostrado con el acumulado por el sistema en cada muestra.

Evidencia de ejecución: `Artifacts/PortalOperations/checks.txt`, `samples.txt`, `probe-result.txt`, logs del Editor y resultados de los probes existentes. Los resultados finales se enumeran en el README de capturas.

Resultado final: **PASS** del probe específico de mirada y de `GameplayFlowProbe`, `BankOfficeProbe` y `PrototypeSmokeProbe` en Unity. `DomainTests`: **27/27 PASS**. La muestra final de ATAQUE fue 49,07 % y la de DEFENSA 49,38 %; en ambas, el `fillAmount` coincidió con `DwellProgress`. La prueba de oficina inicialmente falló porque intentaba aplicar la defensa antes de terminar la confirmación del segundo modo; se corrigió la sincronización de la prueba y pasó conservando sus aserciones.

Comparación SHA-256 con el respaldo: 1043 archivos anteriores de Assets idénticos; siete modificados (escena y seis scripts), sin archivos anteriores faltantes. Detalle: `Artifacts/PortalOperations/asset-delta.json`. Los espacios finales que señala `git diff --check` en la escena corresponden a campos vacíos serializados por Unity; no se normalizó el YAML de la escena manualmente.

Se verifican 0 %, aproximadamente 50 % de ATAQUE y DEFENSA, reinicio al apartarse, ausencia de transferencia, selección al completar 1,5 s, confirmación antes del prólogo, activación única al mantener la mirada, ambos modos, retorno por teletransporte, ajuste de TMP, retícula y enlaces del prefab. La tolerancia de muestreo depende de un frame: no se afirma precisión subframe.

Hubo un cierre nativo del Editor después de una ejecución satisfactoria, con stack en `ujob_execute_job / lane_guts / worker_thread_routine`. Se conservó `Artifacts/PortalOperations/editor-crash-session.log` y se reabrió Unity. La causa raíz del cierre nativo no está determinada. Las excepciones de MaterialPropertyBlock observadas durante la recarga previa motivaron la inicialización defensiva indicada arriba; no se atribuye el cierre nativo a esas excepciones sin evidencia.

## Comparación visual real

Se abrieron la referencia `REF_Portal_Operations.png` y las capturas de Unity. Coinciden la composición de dos tarjetas, cabecera, azul noche, banco ámbar, escudo/candado cian, textos independientes y zonas inferiores de mirada. Las barras funcionan de manera distinta a las dos barras simultáneamente llenas de la ilustración: en el juego solo avanza el objetivo real.

La versión implementada simplifica el concepto: bordes rectos, iconos de líneas UI y ausencia de bevels, bloom y efectos holográficos. No se declara reproducción literal ni aprobación artística final. En el estado anterior capturado no se reprodujeron los iconos azules gigantes; por tanto no se afirma haber aislado su causa histórica. La nueva composición contiene iconos propios acotados y una única capa activa de presentación.

## Pendiente en Android / HONOR

No se generó APK ni se realizó prueba de dispositivo en esta tarea. Comprobar con Cardboard: lectura binocular desde la posición inicial y el punto de selección; retícula sobre texto; suavidad del llenado; cancelación al mover la cabeza; selección única; cambio entre tarjetas; confirmación antes del prólogo; regreso mediante teletransporte; rendimiento, temperatura y comodidad. No se modificaron URP, Cardboard ni reglas/cálculos bancarios.
