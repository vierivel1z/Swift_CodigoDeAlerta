> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Afinado visual del portal de operaciones

2026-11-03, Lima. Unity 6000.3.24f1, URP 17.3.0 existente. Rama `review/portal-dwell-feedback`. Sin commit ni push.

## Cambios visuales

- Marco de esquinas recortadas, doble borde cian, sombra y halo discreto construido con capas UI. No se añadió bloom ni iluminación.
- Cabecera con «SWIFT /» de mayor peso, separación del título, línea cian, pequeñas diagonales y sello institucional integrado.
- Tarjetas paralelas ámbar/cian, bordes más gruesos, fondos con gradiente, facetas superiores y tonos internos que sugieren profundidad.
- Banco, escudo/candado y ojos reconstruidos con geometría UI acotada. Usan la ruta `Image` y el shader estándar `UI/Default`, sin shaders propios ni dependencias.
- Títulos de 118 unidades TMP, descripciones de 62 con mayor interlineado y módulos de mirada con contenedor, borde, ojo y más padding.
- Barras de 48 unidades de alto, frente a las 26 anteriores, con pista redondeada, borde y reflejo estático tenue. Los mismos componentes `Image` conservan `Filled / Horizontal / Left` y sus enlaces originales.
- La etiqueta redundante junto a la retícula se oculta únicamente sobre ATAQUE/DEFENSA, que ya muestran la instrucción en su módulo. El punto, anillo y progreso de la retícula se conservan. No se movieron los objetivos físicos.

La referencia `REF_Portal_Operations.png` se abrió y comparó con los PNG reales de Unity. Se acercaron la composición, jerarquía, identidad institucional, iconos y acabado de las tarjetas. Se conserva la proporción física de la consola existente, más ancha que el concepto. No se copiaron reflejos cinematográficos, biseles 3D, hologramas ni bloom intenso. No se declara coincidencia literal ni aprobación artística final.

## Funcionalidad conservada

Las identidades de ATAQUE, DEFENSA y sus imágenes de progreso se verificaron antes y después de aplicar el estilo. Los colliders mantienen tamaños y posiciones. No se reescribió el temporizador de dwell, el raycast ni la selección con ratón.

Comparación SHA-256 con el respaldo: `VRInteractionManager`, `BankInteractable`, `EditorInputAdapter`, `GameManager`, `IntroSequence` y `ModeStationVisual` son idénticos. Todos los archivos de ProjectSettings también son idénticos. Evidencia: `Artifacts/PortalVisualPolish/preservation-checks.json`.

El cambio en `GazeProgressVisual` es exclusivamente la visibilidad de su texto duplicado sobre las tarjetas. No cambia estados, acumulación, activación ni representación del anillo.

## Archivos afectados

| Archivo/recurso | Uso |
|---|---|
| `Assets/_Project/Scenes/BancoCorresponsal.unity` | Nueva presentación del portal; conserva el resto del estado abierto |
| `Assets/_Project/OfficeVisual/PortalOperations/PortalOperations.prefab` | Catálogo modular actualizado con la misma vinculación de acciones y barras |
| `Assets/_Project/OfficeVisual/PortalOperations/UI_ChamferFrame_256.png` y meta | Marco blanco reutilizable en 9 segmentos, 256 × 256 |
| `Assets/_Project/OfficeVisual/PortalOperations/UI_Capsule_128.png` y meta | Pista redondeada reutilizable, 128 × 64 |
| `Assets/_Project/Scripts/UI/PortalPanelGraphic.cs` y meta | Fondos y facetas mediante vértices UI |
| `Assets/_Project/Scripts/UI/PortalIconGraphic.cs` y meta | Banco, escudo/candado y ojo mediante vértices UI |
| `Assets/_Project/Scripts/UI/GazeProgressVisual.cs` | Ocultación del texto redundante, sin cambiar el anillo |
| `Assets/_Project/Editor/PortalVisualPolishReview.cs` y meta | Aplicación con APIs del Editor, comprobaciones y capturas |

Los PNG originales de referencia y los componentes visuales anteriores se conservan. La decoración anterior se desactiva; no se borran recursos originales. No se añadieron texturas grandes, materiales complejos ni paquetes.

## Respaldo y estado del Editor

Antes de editar se copiaron Assets, Docs y ProjectSettings a:

`D:\UnityGames\SWIFT_Backups\PortalVisualPolish-20261103-224008`

Como la escena del Editor tenía cambios pendientes, se guardó además una copia con `SaveScene(..., saveAsCopy: true)` antes de intervenir. Está en `Assets/_Recovery/PortalVisualPolish_Before_20261104T034725888.unity` y en el respaldo externo como `EditorBefore.unity`. La copia tiene GUID propio; la escena principal mantiene el suyo. Para restaurar el contenido de `EditorBefore.unity` en BancoCorresponsal, conservar el `.meta` original de BancoCorresponsal, no sustituirlo por el de la copia de recuperación.

La escena principal se guardó después del afinado, incluyendo el estado previo del usuario conservado en esa copia. No se utilizó reset, clean ni checkout destructivo.

## Verificación final

| Comprobación | Resultado |
|---|---|
| Foco y cancelación sobre ambas tarjetas | PASS Play Mode |
| Barra parcial frente al dwell real | PASS: ATAQUE 49,17 %, DEFENSA 49,24 %, sin progreso en la tarjeta contraria |
| Dwell de 1,5 s y activación única | PASS |
| Soltar/repetir en PC y cambio de tarjeta | PASS |
| Prólogo y retorno al portal | PASS |
| Todos los TMP activos sin desbordes | PASS Editor y Play Mode |
| GameplayFlowProbe | PASS |
| BankOfficeProbe | PASS |
| PrototypeSmokeProbe | PASS |

Los resultados finales están en `Artifacts/PortalVisualPolish/*-result.txt`; las iteraciones intermedias se conservaron. La primera aplicación detectó que faltaba altura para el título y se corrigió antes de finalizar. Los gráficos vectoriales se comprobaron visualmente tras corregir su ruta de render; no se dio por suficiente una prueba funcional sin iconos visibles.

Galería: [capturas reales antes/después, foco y progreso](../../../Previews/PortalVisualPolish/20261103/README.md), con recibos UTC y hashes. Los timestamps del 4 de noviembre UTC corresponden a la noche del 3 de noviembre en Lima.

## Límites y revisión en HONOR

Desde la posición inicial real se distinguen los modos, colores e iconos; los textos secundarios ocupan menos ángulo que en la vista frontal cercana. Se mejoraron sus tamaños sin reducir el panel. La lectura binocular y comodidad siguen pendientes en HONOR, especialmente para la descripción y el módulo inferior desde lejos.

No se probó rendimiento Android ni se generó APK. Los contadores `UnityStats` de los recibos son agregados del Editor, no una medición aislada del coste UI ni una certificación de FPS en móvil. Los marcos transparentes omiten el relleno central para reducir overdraw; los fondos usan vértices y las dos pistas emplean máscaras UI estándar. Medir draw calls, overdraw, memoria y temperatura en el dispositivo sigue pendiente.
