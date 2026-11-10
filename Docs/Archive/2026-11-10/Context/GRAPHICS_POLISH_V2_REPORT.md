> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Graphics Polish V2 â€” revisiÃ³n aislada de BancoCorresponsal

Fecha: 2026-11-03. Unity 6000.3.24f1, URP 17.3.0. Rama `review/environment-immersive`, base `1074fdd313eac8220451f11559e6076ba4fb4e2c`. Sin commit, push o merge.

## Estado e integraciÃ³n

**El pulido se implementÃ³ en `Assets/_Project/Scenes/Review/BancoCorresponsal_GraphicsV2.unity`. BancoCorresponsal principal permanece intacta.**

La inspecciÃ³n encontrÃ³ varios procesos Codex y un Editor del proyecto. Esto no prueba que todos estÃ©n editando la escena. Se pidiÃ³ aclarar la concurrencia; al no estar confirmada la exclusividad durante la implementaciÃ³n, se utilizÃ³ la alternativa aislada solicitada por el usuario. La copia se creÃ³ con `AssetDatabase.CopyAsset`, con GUID propio. No se cambiÃ³ la lista de escenas de compilaciÃ³n: esta revisiÃ³n todavÃ­a no llega al juego compilado.

Respaldo inicial completo de Assets, Packages, ProjectSettings y Docs: `D:/UnityGames/SWIFT_Backups/GraphicsPolishV2-20261103-135942`. Incluye el estado local previo, sin descartar cambios. Los checkpoints de cada fase estÃ¡n bajo ese directorio; listado en `Artifacts/GraphicsPolishV2/checkpoints.txt`. No hay reset, clean ni checkout destructivo.

Para revisar: abrir la escena V2, usar Play y consultar la galerÃ­a. La integraciÃ³n estÃ¡ preparada en el menÃº **SWIFT > Graphics Polish V2 > Integrar revision validada (sin otra sesion editando)**. Usarlo Ãºnicamente una vez resuelta la concurrencia. Comprueba que la escena principal no cambiÃ³ desde la copia, exige resultados de pruebas y crea otro respaldo antes de guardar una copia sobre la principal mediante Unity. Si la principal cambiÃ³, exige reconciliaciÃ³n selectiva; no la sobrescribe. No se ejecutÃ³ ese menÃº en esta sesiÃ³n. DespuÃ©s de integrar hay que abrir BancoCorresponsal, verificar iluminaciÃ³n/referencias, repetir las sondas y generar la APK.

## 1. DiagnÃ³stico inicial y referencias

Se leyeron los informes de pulido, inventario/licencias, contexto, diseÃ±o y revisiÃ³n. Se abrieron las dos referencias aprobadas `DiseÃ±oVisual_Completo.png` y `DiseÃ±oVisual_PC.png`, las capturas finales anteriores y las nuevas capturas Before de esta sesiÃ³n.

El pulido anterior ya habÃ­a corregido orientaciÃ³n de sillas/monitores, apoyo del teclado/libreta, terminal de cierre, ocho destinos y presentaciÃ³n de navegaciÃ³n. No se reconstruyen esos sistemas. La oficina conserva 20 Ã— 18 m, cuatro cubÃ­culos secundarios y seis sillas.

Las diferencias perceptibles frente a las referencias eran madera anaranjada, archivos con el mismo acabado de madera, teclado/ratÃ³n secundarios que parecÃ­an metal, techo marrÃ³n oscuro de mÃ³dulos grandes, columnas sin separaciÃ³n de cuerpo/remates, luces de trabajo naranjas en todos los puestos y accesorios repetidos. Los libros de dos cubÃ­culos habÃ­an quedado desplazados hacia mobiliario de recepciÃ³n: se devolvieron a su propio tablero.

La referencia dirige la relaciÃ³n de madera cÃ¡lida, textiles azul gris, metal sobrio, luz administrativa neutra y acento del puesto. Se mantienen geometrÃ­a low-poly y monitores funcionales grandes. La revisiÃ³n sigue siendo mÃ¡s simple que el arte; **no se declara coincidencia visual ni aprobaciÃ³n artÃ­stica del usuario**.

## 2. Materiales

Biblioteca propia: `Assets/_Project/OfficeVisual/GraphicsPolishV2/Materials/`, 18 materiales compartidos. Overrides de renderers en la copia de escena. No se sobrescriben materiales, prefabs, modelos o importadores previos.

| Familia | Acabado y uso |
|---|---|
| Madera | Roble marrÃ³n desaturado y nogal de inserto; smoothness 0,28/0,23; grano pequeÃ±o existente |
| Metal | Grafito pintado 0,38/metallic 0,35; esmalte de archivo 0,32/0,15; aluminio de remate 0,38/0,65 |
| PlÃ¡stico | Carcasas oscuras mates, keycaps separados; smoothness 0,18/0,12 |
| Tela | Mamparas pizarra y asiento carbÃ³n, tejido suave, smoothness 0,04/0,06 |
| Papel | Marfil y canto de pila, smoothness 0,025; se conserva el detalle impreso de los modelos |
| Arquitectura | Mineral para pared/columna, techo marfil, juntas de menor contraste y dos tonos de moqueta para circulaciÃ³n |
| Exterior | Ventanas cÃ¡lidas opacas y discretas, sin emisiÃ³n que requiera luz dinÃ¡mica |

Se reutilizan las texturas procedurales anteriores de roble, tejido y moqueta de 256Â²; no se generan mapas de alta resoluciÃ³n ni se modifican los originales. ContinÃºan mipmaps, trilineal, aniso 2 y compresiÃ³n. Las texturas originales de computadora/teclado Google y los artes proporcionados por el equipo se conservan.

No se separa artificialmente cada cara en un material distinto: algunas patas integradas en la malla del escritorio siguen compartiendo madera. Un cambio de topologÃ­a para convertirlas en metal queda como refinamiento opcional, no se presenta como implementado.

## 3. Arquitectura

- Se conservan zÃ³calos, cornisa y canales de mampara del pulido previo.
- RetÃ­cula de techo de 1,2 m con perfiles finos y contraste reducido; la retÃ­cula previa queda conservada con renderer deshabilitado.
- AlfÃ©izares/revelados de ventanas con aluminio, combinados en una malla.
- Cuerpos de columnas minerales con basamento y collar superior de metal; remates agrupados por material.
- Marco de entrada con acabado metÃ¡lico; no se modifica el vano ni su collider.
- Plintos de mamparas para un encuentro mÃ¡s limpio con el suelo.
- La moqueta diferencia recorrido y puestos con dos tonos controlados; no se desplazan cintas de circulaciÃ³n ni destinos.

Los elementos nuevos de remate no aÃ±aden colliders ni reducen accesos. Mallas combinadas y UV2 generadas mediante Unity. No se rehace la oficina ni se aÃ±aden detalles fuera de las vistas Ãºtiles.

## 4. Luz

Se reutilizan las 17 luces baked y la Ãºnica direccional realtime existente. Puesto principal cÃ¡lido menos saturado; cuatro Ã¡reas de cubÃ­culos y supervisiÃ³n pasan a neutro suave. El techo administrativo conserva lectura azul gris discreta. No hay luz dinÃ¡mica por escritorio, bloom, SSAO, volumÃ©tricos o reflejos realtime nuevos.

Se vuelve a hornear despuÃ©s de los remates finales. La copia tiene su propia carpeta de LightingData/lightmaps/reflexiÃ³n; no se escribe la iluminaciÃ³n de BancoCorresponsal original. Se mantienen 81 sondas, una reflexiÃ³n baked de 128Â² y un atlas direccional de 1024Â². Fechas y hashes finales constan en la galerÃ­a y recibos.

## 5. CubÃ­culos y exterior

Se conserva silla frente al monitor, pantalla correctamente orientada, dimensiones y posiciÃ³n de cada mesa. Dos pilas de papel cambian discretamente su Ã¡ngulo; libros previos de los cubÃ­culos 2/4 se apoyan en sus tableros y el cubÃ­culo 3 recibe un grupo de libros existente. El cubÃ­culo 1 sigue mÃ¡s despejado. No se aÃ±aden interfaces ni lÃ³gica a puestos secundarios.

Fuentes: papeles de Jarlan Perez, libros Kenney CC0, ya acreditados en ASSET_CREDITS. No se utiliza Office Pack de licencia pendiente ni se descarga contenido.

Se conservan siluetas y paneles nocturnos opacos existentes. Las luces urbanas se redistribuyen dentro de las siluetas reales, en una malla compartida; se conserva el renderer previo apagado. No hay ciudad 3D adicional, vidrio transparente extenso ni reflejo costoso.

## 6. Capturas reales

[GalerÃ­a Before / After](../../../Previews/GraphicsPolishV2/README.md).

Carpeta real: `Docs/Previews/GraphicsPolishV2/`. Before y After usan las mismas posiciones, destinos, FOV 60 y resoluciÃ³n 1600 Ã— 900: entrada, general, cubÃ­culos, puesto, documentaciÃ³n, supervisiÃ³n y nocturna. La octava vista es planta ortogrÃ¡fica tÃ©cnica con techo oculto temporalmente y restaurado. Materials y Architecture conservan iteraciones intermedias. Play muestra mirada/cierre, puesto de pie y sentado; Profiles compara temporalmente los assets PC y Mobile existentes y restaura el perfil.

Son renders reales de Unity Editor. Ninguna referencia IA se presenta como captura. La UI de Before/After es el estado guardado; la UI dinÃ¡mica y decisiones se comprueban en Play. Los render requests monoscÃ³picos no certifican Cardboard.

## 7. Pruebas y problemas

Resultados finales y fechas se incorporan al README y `validation-results.json`. Solo se considera PASS un recibo nuevo de esta revisiÃ³n.

- DomainTests: 27/27 PASS.
- GameplayFlowProbe: prÃ³logo omitido y fin temporizado de 12 s, ambos modos, comparaciÃ³n, ciclo de botones y Aprobar/Retener/Devolver.
- BankOfficeProbe: cÃ¡mara/canvas, cinco objetos principales, evidencia, registro, alertas, defensa, raycasts y decisiones.
- PrototypeSmokeProbe: ambos modos, seis decisiones, evidencia, consultas, defensa, resultados y mirada/latch.
- RevisiÃ³n de entorno: ocho destinos, circulaciÃ³n/retorno, selecciÃ³n de modo, libreta, sentado/de pie, fade y TMP.
- RevisiÃ³n V2 en Play: seis sillas, cierre por mirada una vez, rechazo de turno incompleto, raycasts de seis acciones desde asiento, textos sin overflow y proveedor de pose habilitado.
- ComparaciÃ³n de configuraciÃ³n protegida: poses/destinos/colliders de acciones bancarias frente a la escena principal de origen.
- Shaders visibles compatibles y ausencia de materiales rosas en las capturas inspeccionadas.

Incidencias de herramienta, corregidas sin cambiar reglas: la primera etapa de arquitectura suponÃ­a papel en todos los cubÃ­culos y se detuvo al encontrar uno con libros; se conservÃ³ el estado parcial, se hizo checkpoint y se continuÃ³ admitiendo accesorios opcionales. El guard de escena sucia rechazÃ³ un bake antes de su checkpoint explÃ­cito; se guardÃ³ de forma recuperable y se repitiÃ³. No se presentan esos intentos como pruebas superadas. No se detectaron fallos de lÃ³gica bancaria en las sondas finales.

## 8. Coste y calidad mÃ³vil

Inventario inicial â†’ final: **284 â†’ 288 MeshRenderer**, **49 â†’ 54 materiales visibles distintos**, **31.228 â†’ 32.718 triÃ¡ngulos** con renderer habilitado (+1.490; +4,77 %). Las mallas ocultas de respaldo no forman parte de ese Ãºltimo recuento. No equivale a coste GPU.

El aumento es acotado: remates agrupados y un grupo de libros. Reutilizar un material reduce variantes, pero no garantiza por sÃ­ solo una reducciÃ³n de draw calls. No se presume batching efectivo sin inspeccionar el frame.

URP Mobile permanece en renderScale **0,8**, MSAA **1** (sin antialiasing), sombras principales **1024**, distancia **50**, sombras adicionales desactivadas. PC: scale 1, MSAA 1 y sombra 2048. Sin cambio de configuraciÃ³n global. El dentado de lÃ­neas/TMP y juntas finas sigue siendo visible; debe medirse MSAA 2 frente al coste actual en HONOR antes de decidir.

`quality-audit.txt` registra lightmaps 1024Â², formatos efectivos importados, memoria reportada por el Editor y filtros/compresiÃ³n. La pareja color/direcciÃ³n reporta aproximadamente 5,34 MiB en este Editor; no es la memoria total del juego ni una mediciÃ³n de Android. `profile-comparison.txt` contiene draw calls, batches y triÃ¡ngulos globales del Editor; incluyen otras vistas y **no son un benchmark por cÃ¡mara ni comparaciÃ³n causal antes/despuÃ©s**.

Overdraw: se mantiene exterior opaco y no se agregan grandes transparencias ni overlays. La UI existente sigue requiriendo mediciÃ³n. **Frame Debugger, mapa real de overdraw, CPU/GPU frame time, FPS y temperatura en dispositivo quedan pendientes.** No se afirma optimizaciÃ³n Android.

## 9. Recursos adicionales

No hace falta un modelo o imagen externa para usar esta revisiÃ³n. Se mantiene el inventario de licencias existente.

Opcionales despuÃ©s de evaluar en telÃ©fono: una textura seamless de roble desaturado de 512Â², una tela acÃºstica de baja frecuencia y una panorÃ¡mica nocturna opaca simplificada si las siluetas resultan insuficientes. No se importaron ni se presentan como existentes. Audio final sigue pendiente en ART_AUDIO_NEEDS; esta tarea no aÃ±ade sonido.

Prompt opcional de madera: Â«Textura seamless de roble de oficina, marrÃ³n medio desaturado, veta larga tenue, iluminaciÃ³n completamente uniforme, sin sombras ni reflejos horneados, sin texto, sin marcas, variaciÃ³n de contraste baja, albedo para videojuego mÃ³vilÂ». ImportaciÃ³n prevista: `Assets/_Project/Art/Environment/Textures/T_Oak_Albedo_512.png`, conservando el material actual como alternativa.

Prompt opcional de ciudad: Â«Panorama horizontal de ciudad nocturna ficticia, edificios de oficinas lejanos en silueta azul gris, pocas ventanas cÃ¡lidas, horizonte bajo, sin calles ni personas, sin logotipos ni texto, perspectiva frontal, sin bloom ni reflejos, detalle de baja frecuencia para fondo opaco de videojuego low-polyÂ». Ruta prevista: `Assets/_Project/Art/Environment/City/T_NightCity_Opaque.png`; recorte por ventanas y compresiÃ³n se decidirÃ­an con pruebas de aliasing y memoria.

## 10. Pendientes y siguiente paso

1. Resolver concurrencia y revisar las parejas reales; integrar selectivamente si hubo cambios nuevos en la escena principal.
2. Verificar BancoCorresponsal tras integrar y generar APK nueva con logs y SHA-256, conservando la anterior.
3. En HONOR: confirmar primera escena, sensores/QR/recenter y pausa/reanudaciÃ³n; mirar y activar una vez a 1,5 s; recorrer ocho destinos; sentarse/levantarse y comprobar fade/altura.
4. Leer expedientes, registros, alertas, procedimiento y resultados con ambos ojos; aprobar/retener/devolver en ambos modos; asegurar que el monitor no queda bloqueado y que el segundo monitor se consulta cÃ³modamente.
5. Revisar moirÃ©/parpadeo de techo, tejido, teclado y textos, brillo de lÃ¡mpara, zonas demasiado oscuras y contraste nocturno.
6. Medir frame time, CPU/GPU, draw calls, overdraw, memoria y temperatura durante una sesiÃ³n sostenida. Solo entonces probar cambios controlados de MSAA/renderScale con respaldo.

La aprobaciÃ³n artÃ­stica y validaciÃ³n Android siguen pendientes. Ninguna APK se genera en esta sesiÃ³n de pulido.

## Cierre verificable

45 PNG nuevos, incluidas siete parejas comparables, planta tÃ©cnica, iteraciones y Play. Se revisaron directamente las siete parejas finales, planta y monitor centrado desde asiento. El recorte superior del encuadre inferior `Play/04-puesto-sentado.png` se documenta; `Play/06-monitor-centrado.png` muestra el monitor entero desde el mismo asiento. No se redujo ni desplazÃ³ el panel para capturar.

Ãšltimos recibos UTC: bake 19:10:01; After 19:11:27; GameplayFlowProbe 19:12:03; BankOfficeProbe 19:12:13; PrototypeSmokeProbe 19:12:22; entorno 19:13:12; Play V2 19:14:29; configuraciÃ³n protegida 19:14:30; sello de escena/dependencias 19:14:31. DomainTests 27/27 durante esta sesiÃ³n. Todos PASS; Android no ejecutado. Fechas completas en `validation-results.json`.

Se compararon los **995 archivos existentes de Assets/Packages/ProjectSettings** con el respaldo inicial: ninguno modificado ni eliminado. Esto incluye escena principal, iluminaciÃ³n previa, modelos, materiales previos, scripts bancarios/UI/VR, ajustes URP, XR y paquetes. Los nuevos archivos de implementaciÃ³n son `Editor/GraphicsPolishV2Review.cs` y su meta, la escena de Review y su iluminaciÃ³n propia, la biblioteca `OfficeVisual/GraphicsPolishV2` y sus metas. El helper es solo de Editor, con comandos explÃ­citos; no ejecuta etapas al importar ni aÃ±ade cÃ³digo al player.

DocumentaciÃ³n aÃ±adida: este informe, galerÃ­a/manifest/recibos. Se aÃ±aden entradas al Kanban, crÃ©ditos y changelog sin borrar historial. Evidencia local en `Artifacts/GraphicsPolishV2/`, incluidos resultados, auditorÃ­a de calidad, inventarios, comparaciÃ³n de colliders y hashes. La integraciÃ³n preparada comprueba tambiÃ©n que la revisiÃ³n y dependencias siguen coincidiendo con el sello validado; cualquier ediciÃ³n posterior requiere volver a validar.

Checkpoint final completo: `D:/UnityGames/SWIFT_Backups/GraphicsPolishV2-20261103-135942/FinalCheckpoint-20261103T141925`. Copia de Assets/Packages/ProjectSettings/Docs y evidencias V2, sin Library/Temp ni APK.
