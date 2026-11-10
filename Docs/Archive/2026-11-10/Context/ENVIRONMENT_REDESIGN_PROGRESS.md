> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Rediseño inmersivo — registro por fases

2026-11-02. Rama local `review/environment-immersive`, base `1074fdd313eac8220451f11559e6076ba4fb4e2c`. Sin commit, push ni merge en esta tarea.

## Fase 0: inspección y protección

Se inspeccionó Git, escena guardada desde Unity, jerarquía, dependencias nativas, prefabs y modelos. Estado inicial: una escena, dirty=False, fuera de Play. Se conservan los cambios previos en ProjectSettings/ProjectSettings.asset, referencias artísticas y galerías locales. Las referencias PNG nuevas se importan normalmente con Unity, sin modificar sus bytes.

Respaldo completo de Assets, Packages, ProjectSettings y Docs antes de cambios: `D:/UnityGames/SWIFT_Backups/EnvironmentRedesign-20261102-161323`. Restauración selectiva desde esa carpeta, con el Editor fuera de Play y preservando previamente cualquier cambio posterior. No se ejecutan reset, clean ni regeneradores de oficina.

Inventario Editor y dependencias: `Artifacts/EnvironmentRedesign/hierarchy-before.txt`, `model-inventory.txt` y `dependencies-before.txt`. Recursos y licencias: ENVIRONMENT_ASSET_NEEDS.md y ASSET_CREDITS.md.

## Plan verificable

1. Capturar oficina/planta anterior realmente en Unity.
2. Ampliar arquitectura de 10 × 10 m a 16 × 14 m sin mover los destinos originales. Cuatro cubículos, dos filas laterales y corredor central; conservar objetos funcionales y vínculos.
3. Revestir las dos acciones originales con una única consola; un solo servicio de dwell de 1,5 s y señales de foco/estado sin luces dinámicas adicionales.
4. Ordenar el puesto principal: marco proporcional a UI, consulta secundaria, libreta TMP ampliable, silla y accesorios existentes.
5. Mover el origen del rig con fade al sentarse/levantarse; mantener HeadPoseAdapter, compensar offset real de cámara y bloquear pads sentado.
6. Consulta de formato exclusivamente de referencia/observación/alertas precargadas, sin nuevas reglas ni revelación de Recommended/SimulatedFraud.
7. Ajustar familias materiales y cobertura de luces baked, guardar checkpoint y hornear con geometría definitiva.
8. Probar consola/silla/raycast/offset/estado/textos, repetir pruebas existentes y producir galería real. Android, confort, rendimiento y aceptación estética separados.

## Implementación guardada

### Oficina y distribución

Arquitectura ampliada a **16 × 14 m**: X −8…8, Z −5…9, techo conservado a 3,4 m. Entrada institucional al sur, consola común de modos delante de la entrada, analista en el eje central, impresión/archivo al oeste y supervisión/seguridad al este. Cuatro cubículos secundarios (X ±5,55; Z 0,9 y 5,6) con mamparas, escritorios compartidos, sillas, monitores y pequeñas variaciones de libros/papeles/lámparas. Dos corredores laterales y un circuito posterior; no hay nuevas reglas bancarias en esos puestos.

Se conservan las referencias de los objetos funcionales. El escritorio, computadora, silla y accesorios del analista se adelantaron 0,7 m para que el destino original de inspección siga siendo un lugar de pie libre. El cuadro y los rótulos se realinearon con la arquitectura; los paneles de seguridad y archivo recibieron soportes físicos. Los modelos no se eliminaron; los dos atriles originales y accesorios redundantes se conservan desactivados.

### Portal y navegación

Una consola integra las dos acciones originales ATAQUE/DEFENSA. Mantiene GameManager.EnterScenario y el prólogo existente, sin duplicar temporizador. Dos tarjetas TMP con descripciones, estados de foco/selección y lámparas emisivas suaves: ámbar y turquesa. Estas lámparas no son luces dinámicas.

Destinos originales, conservados exactamente y contrastados con la copia anterior de escena: Entrada `(0,0,-3.3)`, Inspección `(0,0,-0.65)`, Evidencia `(-2.8,0,-0.1)` y Seguridad `(3.3,0,2)`. El **marcador** de Inspección se recolocó delante y a la izquierda de la consola, con etiqueta AL PUESTO, para que el panel no lo oculte. Su destino no cambió.

Dos destinos adicionales, usando el mismo BankAction.Teleport/retícula/dwell: Cubículos Oeste `(-3.35,0,6.45)` y Este `(3.35,0,6.45)`. Permiten explorar la parte posterior sin locomoción nueva. La red de seis destinos tiene trayectos de entrada y regreso comprobados por raycast, no necesariamente visibilidad directa entre cada pareja. Los originales siguen disponibles; el texto histórico de BankOfficeProbe menciona cuatro, aunque su bucle ejercita todos los pads encontrados. La revisión específica comprueba los seis.

### Silla y seguimiento

AnalystSeat distingue de pie, sentado y transición. Se inicia únicamente al seleccionar SENTARSE. Comprueba ocupación antes de oscurecer y otra vez con la pantalla opaca. Fundido de 0,16 s, cambio instantáneo del **origen del rig**, recuperación de 0,20 s; no hay descenso animado de la cámara ni rotación impuesta.

El objetivo virtual sentado está a `(0,1.25,0.05)`. Se resta el offset real entre cámara y origen en el momento del salto; no se fuerza camera.localPosition ni se desactiva HeadPoseAdapter. La altura virtual del puesto no se interpreta como estatura física. Al levantarse se recupera la altura observada al entrar, con destino de pie lateral `(1.6,0,-0.65)`. La fórmula se comprobó también con un offset sintético distinto de 1,6 m, separado de cualquier prueba XR real.

LEVANTARSE está visible y seleccionable desde el asiento. Se conservan el mismo servicio de mirada de 1,5 s y clic PC; solicitudes duplicadas o durante transición se rechazan. Los seis colliders de pads y la locomoción WASD se bloquean sentado/transicionando; al salir se restauran sus estados previos. La rotación de cabeza permanece independiente.

### Puesto, monitores y libreta

El Canvas bancario mantiene 1100 × 800 y escala 0,0015: **1,65 × 1,20 m**. No se redujo para encajarlo en un monitor pequeño: se construyó un marco físico acorde. Se conserva íntegramente UIManager.KeepPanelInFrontOfComputer. El expediente presenta campos alineados, monto grande y columna separada de observaciones; iconos geométricos de aprobar/pausa/devolver acompañan los textos originales exactos. Registro, alertas, procedimientos, resultados y cálculos se conservan.

La pantalla secundaria usa TMP editable y separa REFERENCIA, INFORMACIÓN OBSERVADA y ALERTA EXISTENTE. Proyecta los datos/indicadores precargados; no valida cuentas reales, no crea reglas y no consulta Recommended ni SimulatedFraud. Se eliminan de la presentación las instrucciones directas de decisión que antes acompañaban algunas observaciones, también en el feedback de consulta. No se cambia el contenido de dominio, la evidencia registrada ni la corrección de decisiones. Los criterios generales siguen disponibles en el procedimiento.

Clipboard de geometría propia, papel/clip y cuatro pasos TMP, colocado horizontalmente fuera del teclado. La mirada abre la página existente PROCEDIMIENTO ampliada en el monitor, sin mover la cámara, grabar notas ni crear inventario. Papeles reordenados y teclado/mouse redundantes desactivados; lámpara sin luz dinámica propia.

Dimensiones calculadas, no aprobación ergonómica: monitor principal a unos 1,41 m del ojo sentado, alrededor de 61° × 46°; de pie desde inspección, unos 2,11 m y 43° × 32°. La consulta lateral requiere girar la cabeza hacia la izquierda. Lectura binocular, tamaño angular percibido y comodidad física siguen pendientes.

### Materiales e iluminación

Familias compartidas de roble, polímero, metal pintado, tela, papel y señales. La computadora conserva su atlas original. Nuevos materiales URP Lit económicos, sin transparencias de cristal ni efectos de postprocesado nuevos. Cuatro cubículos comparten prefabs/mallas/materiales; no se desempaquetaron los originales.

Iluminación rehorneada con geometría final. Se recolocaron las seis luces de techo, se mantuvo el relleno del analista y se añadieron dos luces **baked** traseras. Resultado: tres lightmaps con texturas de color/dirección, nueve luces baked activas y una direccional realtime ya existente. Sin una luz dinámica por escritorio, cambios de URP Mobile, XR, calidad global o dependencias.

Inventario de Editor: 177 MeshRenderer activos, **26.004 triángulos** sumando submallas y 39 materiales distintos visibles en mallas. Es un inventario de geometría; **no** mide draw calls, memoria GPU, FPS o coste en Android. Frame Debugger/Profiler móvil y comparación térmica siguen pendientes.

## Archivos afectados

- Escena: `Assets/_Project/Scenes/BancoCorresponsal.unity`, conservando GUID y referencias funcionales; LightingData y lightmaps de su subcarpeta. Dos pares nuevos Lightmap-1/2 con `.meta`.
- Materiales y dos prefabs nuevos (monitor/lámpara normalizados): `Assets/_Project/OfficeVisual/EnvironmentRedesign/`. Los quince prefabs originales de OfficeVisual se mantienen idénticos al respaldo.
- Nuevos componentes: `Scripts/VR/AnalystSeat.cs`, `Scripts/UI/ModeStationVisual.cs`, `Scripts/UI/FormatReferencePanel.cs` y sus `.meta`.
- Modificados: `Scripts/Interaction/BankInteractable.cs` (acciones añadidas al final del enum), `EditorInputAdapter.cs` (bounds de depuración y bloqueo sentado), `Scripts/UI/BankDocumentUI.cs` (presentación TMP).
- Herramienta opt-in Editor: `Assets/_Project/Editor/EnvironmentRedesign.cs`. No se ejecuta al abrir la escena salvo que exista un comando explícito en Artifacts/EnvironmentRedesign/command.txt. No usa red ni generadores anteriores.
- Validadores Editor: `BankNavigationValidation.cs` nuevo y ajuste de BankOfficeBuilder.Validate, OperationsLightingReview.Validate y DocumentUIReview. Ahora comprueban por nombre, coordenadas y referencia los cuatro destinos originales, permitiendo los dos auxiliares; no se sustituyen las pruebas por un simple mínimo de cantidad. Los generadores no se ejecutaron.
- Documentación: este informe, ENVIRONMENT_ASSET_NEEDS.md, KANBAN, CHANGELOG y galería.
- Los `.meta` de las dos referencias PNG recién importadas se conservan; los bytes de arte originales no se cambian.

## Pruebas y resultados

| Comprobación ejecutada | Resultado / alcance |
|---|---|
| DomainTests | **PASS**, 27 checks; dominio, evidencia, decisiones, pérdida y resumen |
| GameplayFlowProbe | **PASS**, prólogo omitido/12 s, ambos modos, comparación y secuencia 6/6 |
| BankOfficeProbe | **PASS**, cinco funciones, nueve roles raycast, decisiones y pads disponibles |
| PrototypeSmokeProbe | **PASS**, fuentes TMP, decisiones/resultados y dwell/latch/rearme |
| EnvironmentRedesign review | **PASS**, mirada real simulada en consola/silla, asiento ocupado rechazado, salto bajo negro opaco, seguimiento habilitado, offset no estándar, seis destinos con espacio corporal/rutas de regreso, tres botones desde asiento, consulta de seis registros sin desbordes y libreta ampliada sin avanzar expediente |
| Guardado / reapertura de datos / render | **PASS**, dependencias nativas existentes, scripts presentes, una cámara persistente, lightmaps color/dirección recuperados y materiales compatibles |
| Android/HONOR, confort, sensores, estéreo | **NO PROBADO** |
| FPS, memoria, temperatura y Frame Debugger Android | **NO PROBADO** |
| Aceptación estética por el usuario | **PENDIENTE** |

Los recibos finales con UTC están en la galería y `Artifacts/EnvironmentRedesign/`. Las pruebas de Editor no certifican seguimiento físico ni confort. No hubo prueba física de clic PC ni una APK nueva durante esta tarea; se conserva la ruta de entrada por clic existente.

Incidencias de coordinación conservadas en logs: capturas/pruebas inicialmente rechazadas por estado dirty tras el montaje/importación; guardado explícito con checkpoint posterior. Un comando de refinamiento se envió antes de que Unity terminara la recarga y se repitió tras compilar. No se descartó una escena modificada. El pedestal tapaba el estado de la consola: se bajó y volvió a capturar. Se corrigió el encuadre de la libreta y la separación intermediario/fecha. La primera expectativa de coordenadas del nuevo validador de Evidencia era incorrecta: se corrigió contrastando escena y respaldo, sin mover ese destino. Los recibos finales PASS no significan que la Console histórica esté vacía.

## Capturas reales y protección

Galería: `Docs/Previews/EnvironmentRedesign/20261102T212047822Z/README.md`, con antes/después comparables, consola, cubículos, puesto de pie/sentado, monitores, clipboard, luz/materiales, planta y marcador LEVANTARSE. Las primeras iteraciones se conservan. Capturas de Editor/Play a 1600 × 1000; nunca imágenes IA presentadas como Unity.

Respaldo inicial completo indicado arriba y checkpoints posteriores en esa misma carpeta externa. Comprobación de integridad de 67 archivos protegidos: paquetes, URP, XR, prefabs originales, dominio, GameManager, escenario, prólogo, pose/lifecycle, ajustes existentes y PNG originales idénticos al respaldo. Recibo `Artifacts/EnvironmentRedesign/preservation-check.json`. El cambio previo de preloadedAssets y otros archivos locales se conserva. No hubo commit/push/merge; main permanece intacta.

## Recursos necesarios y próximos pasos

No falta ningún modelo 3D imprescindible. Solo son opcionales una textura discreta de tejido y otra de roble; especificaciones/rutas/licencias en ENVIRONMENT_ASSET_NEEDS.md. Continúan pendientes los audios finales ya solicitados; los tonos TEMP no se presentan como diseño sonoro terminado.

1. Revisar la galería y aprobar/corregir distribución y proporciones.
2. Generar APK nueva sin sobrescribir la anterior.
3. En HONOR: portal → prólogo/omisión → AL PUESTO → SENTARSE → tres decisiones/consulta/libreta → LEVANTARSE → seis destinos y vuelta a entrada. Probar ambos modos, sensores, recenter, pausa/reanudación y distintas poses al sentarse.
4. Medir lectura de ambas pantallas en ambos ojos, tamaño del marcador, fundido binocular, comodidad del giro lateral y estabilidad del horizonte; ajustar con evidencia del dispositivo.
5. Perfilar Mobile URP: frame time, batches, memoria/lightmaps y temperatura; reducir coste donde lo indiquen las mediciones. Sustituir tonos TEMP por recursos finales licenciados.
