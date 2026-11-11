# Portapapeles físico interactivo — 11 de noviembre de 2026

## Resultado

La escena principal conserva el mismo objeto de madera, clip y papel. Al fijar la mirada sobre él durante el dwell existente de 1,5 s, el objeto completo se eleva primero por encima de la mesa y después se aproxima a una pose de lectura. El contenido dinámico de las tres hojas de `DeskTools` se presenta mediante TextMeshPro sobre el papel real. Los controles de hoja, marca manual, ampliación y devolución usan `BankInteractable` y el único contador de `VRInteractionManager`.

La vista ampliada anterior se conserva. `AMPLIAR` la abre por mirada durante la inspección; también puede dejarse como modo predeterminado desactivando `PhysicalClipboardEnabled` en `DeskTools_Trial.asset`. La variante física está activada en ese perfil. No se editaron la escena, el prefab físico, URP, Cardboard ni las reglas bancarias.

## Estados y resguardo de datos

`PhysicalClipboardPresentation` gestiona `Resting`, `Lifting`, `Inspecting` y `Returning`. Conserva la pose inicial del objeto y la restaura al dejarlo, al salir de la estación, al cambiar de expediente, al pausar y al desactivar el coordinador. El collider del tablero se desactiva mientras se lee para que no intercepte los raycasts de los botones del papel; vuelve a activarse en reposo. La página, las marcas y las fuentes consultadas siguen en `DeskInteractionCoordinator` y `DeskSessionJournal`.

## Verificación realizada

- **PASS Unity Editor 6000.3.24f1, Android como build target:** `physical-before`, `physical-after`, `objects`, `stationpc` y `regression`. Recibos y checks: `Artifacts/ImmersiveGameplay/*-result.txt` y `Artifacts/ImmersiveGameplay/physical-after-1854*/checks.txt`. Las sondas terminaron sin errores nuevos de Console.
- La sonda física comprobó selección real de 1,5 s, indicación `TOMAR PORTAPAPELES`, elevación previa al desplazamiento, pose final, TMP sobre el papel, raycast de hoja y marca, bloqueo de acciones bancarias durante la inspección, `AMPLIAR`, `DEJAR`, conservación de marca/expediente y restauración al salir y volver a la estación.
- Las capturas URP de Play Mode están en [galería antes/después](../Previews/PhysicalClipboard/README.md). El encuadre del escritorio antes/después utiliza el mismo ojo y objetivo. La toma de lectura muestra el cambio de presentación.
- Las sondas `objects` y `regression` reescribieron temporalmente su galería histórica. Se guardó esa ejecución en `Artifacts/PhysicalClipboard/RegressionCaptured` y se restituyeron los ocho archivos históricos que estaban intactos al inicio.

## Límites y siguiente comprobación

No se probó en un HONOR/Cardboard físico. La legibilidad binocular, la comodidad de la distancia de lectura, el tamaño angular de `AMPLIAR` y los botones, y la continuidad del movimiento requieren prueba en el visor. En el teléfono: entrar a la estación, mirar el tablero 1,5 s, leer las tres hojas, marcar una hoja, abrir `AMPLIAR`, volver, usar `DEJAR`, salir y reingresar, y comprobar que la marca persiste y el objeto vuelve a su lugar. Observar si el clip superior oculta alguna línea o si la lectura exige convergencia incómoda.

La restauración geométrica se comprobó en Editor; la ausencia de intersección perceptible con la mesa durante toda la animación se debe confirmar visualmente en el dispositivo. No se afirma seguimiento de manos: la selección sigue siendo por mirada.

## Archivos de esta variante

- `Assets/_Project/Scripts/DeskTools/PhysicalClipboardPresentation.cs` y `.meta`: movimiento, estados, etiqueta de foco y retorno.
- `DeskInspectionView.cs`: composición retrato sobre papel y control `AMPLIAR`.
- `DeskInteractionCoordinator.cs`, `DeskActionTarget.cs`, `DeskToolsProfile.cs`, `DeskTools_Trial.asset`: conexión de la variante, acceso a ampliación y bandera reversible.
- `ImmersivePlayProbe.cs`, `ImmersiveEditorBridge.cs`: sondas Editor y capturas.

No se hizo commit ni push.
