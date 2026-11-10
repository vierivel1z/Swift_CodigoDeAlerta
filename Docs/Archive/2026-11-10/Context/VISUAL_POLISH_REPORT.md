> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Pulido art?stico y correcci?n ? BancoCorresponsal

Fecha: 2026-11-03, America/Lima. Unity 6000.3.24f1, URP 17.3.0, escena BancoCorresponsal. Rama `review/environment-immersive`, base 1074fdd. Sin commit ni push/merge. Se conserva la oficina de **20 ? 18 m**, sin nueva ampliaci?n ni escalado del conjunto.

## Inspecci?n inicial y criterios

Se revisaron las tres capturas enviadas en la conversaci?n, las capturas del estado ampliado y las referencias `Assets/_Project/Art/References/Dise?oVisual_Completo.png` y `Dise?oVisual_PC.png`. Las tres recibidas muestran: silla con asiento hacia afuera, planta en recorrido y bloque de cierre sin apoyo con r?tulo visto por detr?s; supervisi?n vac?a con divisiones sin justificar. Se generaron adem?s nueve capturas nuevas ANTES desde c?maras controladas. No se presentan im?genes anteriores como nuevas.

| Prioridad | Problema observado | Correcci?n |
|---|---|---|
| CR?TICO para orientaci?n | Sillas secundarias y principal con respaldo hacia el monitor | Se inspeccion? la malla chairDesk: centro de v?rtices altos en +Z. Frente real = ?Z. Secundarias apuntan a su propio escritorio; principal cambia solo su modelo visual, sin girar puntos de sentado/de pie |
| CR?TICO para comprensi?n | CERRAR TURNO como bloque sin contexto y cartel visto por detr?s | Acci?n End original integrada en monitor de un escritorio de supervisi?n, con carcasa, teclado y silla; r?tulo frontal legible; regla original intacta |
| IMPORTANTE | Monitores mostrando dorso y teclados de canto | Monitores y teclados secundarios orientados al asiento; apoyo sobre mesa comprobado |
| IMPORTANTE | Planta aislada en circulaci?n; espera dispersa | Macetas y sof?/mesa/libros agrupados en recepci?n y per?metro; macetas apoyadas |
| IMPORTANTE | Archivadores orientados al lado opuesto | Puertas giradas hacia acceso documental; referencias de Registry intactas |
| IMPORTANTE | Teclado/libreta detr?s del panel principal | Superficie de trabajo delante del monitor, teclado, rat?n y libreta visibles y apoyados; panel legible y KeepPanelInFrontOfComputer preservados |
| IMPORTANTE | Materiales oscuros planos y uso poco selectivo de turquesa | Familias compartidas de roble, inserto, tela, metal pintado, pl?stico, papel, mineral y vegetaci?n; cuatro texturas procedurales peque?as |
| IMPORTANTE | Arquitectura sin remates y techo continuo | Z?calos/cornisa, perfiles de mamparas y ret?cula modular, combinados por material en pocas mallas |
| IMPORTANTE | Pads como rect?ngulos verdes sobresalientes | Aros discretos, r?tulos breves y foco ligado al gestor existente; mismos colliders, destinos y activaci?n |
| OPCIONAL | Detalle/realismo distinto del arte de referencia | Mantener low-poly m?vil; mayor variedad de accesorios y acabado fino sujeto a revisi?n, no bloqueo |

## Cambios implementados

### Sillas y apoyos

Las cinco sillas iniciales se revisan individualmente. La geometr?a del respaldo, no el nombre del prefab, determina el frente. En el jugador se conserva el objeto Analyst chair y sus transformaciones de Seated eye target, Safe standing floor y controles; gira solamente el modelo chairDesk alrededor de su apoyo. No se anima la orientaci?n de cabeza. Las cuatro sillas secundarias miran su mesa y conservan separaci?n del tablero y acceso lateral/posterior. La silla adicional de supervisi?n reutiliza el mismo prefab verificado, con acceso lateral por el vano.

La auditor?a de sillas comprueba frente mediante producto escalar (>0,95), altura f?sica de 0,9?1,25 m, ruedas al suelo y ausencia de intersecci?n con mesa. Detalle num?rico: `Artifacts/VisualPolish/chair-audit.txt`.

Los teclados, ratones, papeles y l?mparas secundarios se apoyan en la altura real del tablero. Los libros de la estanter?a nueva se apoyan en un plano horizontal de la malla encontrado por sus tri?ngulos, Y?0,7148 m, no en una altura inventada.

### Organizaci?n por zonas

- Recepci?n/modos: se conserva consola inclinada y sus dos acciones; espera y plantas agrupadas al per?metro, sin objetos decorativos en medio del paso.
- Analistas: cuatro cub?culos existentes, orientaci?n coherente de silla/monitor/teclado, accesorios ordenados y mamparas diferenciadas del tapizado.
- Puesto del jugador: dos monitores funcionales, superficie de apoyo delante de la pantalla, teclado/rat?n originales de Google, libreta y l?mpara existentes. Se conserva tama?o 1400 ? 800 y escala f?sica, sin reemplazar TMP por im?genes.
- Documentaci?n: impresora y evidencia existentes, puertas de archivo hacia el acceso, estanter?a abierta de procedimientos y libros.
- Supervisi?n: escritorio Kenney existente, monitor con acci?n End original, teclado, silla, documentaci?n y cierre de operaciones. No es una nueva mec?nica ni modifica la finalizaci?n autom?tica al completar expedientes.
- Seguridad: paneles/controles existentes conservados. Se mantiene separaci?n respecto al cierre y el lenguaje de advertencia no implica fraude confirmado.

Los aros de navegaci?n reutilizan la selecci?n de 1,5 s de VRInteractionManager. ArchitecturalFocusVisual solo lee foco/latch; no activa nada, no tiene temporizador y usa MaterialPropertyBlock. Oculta el aro cuando su collider se deshabilita estando sentado. Los ocho destinos/colliders originales de este estado se conservan; se apaga ?nicamente el Renderer de los antiguos bloques y sus placas redundantes.

### Materiales y acabados

Nuevos materiales compartidos bajo `Assets/_Project/OfficeVisual/VisualPolish/Materials/`. Overrides de escena; no se sobrescriben prefabs/materiales fuente descargados.

| Familia | Tratamiento |
|---|---|
| Roble | Grano procedural tenue, albedo c?lido, smoothness 0,24; inserto m?s oscuro 0,18 |
| Tela ac?stica | Tejido sutil, azul gris?ceo, smoothness 0,04 |
| Tapizado | Azul oscuro separado de mamparas, smoothness 0,08 |
| Metal pintado | Gris azulado, metallic 0,2 / smoothness 0,32 |
| Detalles champagne | Metallic 0,45 / smoothness 0,35, uso reducido |
| Pol?mero | Marino mate, smoothness 0,2 |
| Papel y mineral | Marfil/neutro, muy poca especularidad |
| Moqueta | Textura de baldosa discreta, roughness alta; sin geometr?a por baldosa |
| Pantallas auxiliares | Composici?n abstracta tenue, sin cuentas, cifras ni resultados inventados; decorativas |
| Vegetaci?n/maceta | Oliva apagado, tierra y maceta mineral; se evita turquesa en hojas |

Tres texturas de 256? para madera, tejido y moqueta; una de 128? para pantallas auxiliares. Mipmaps, filtrado trilineal, compresi?n y aniso 2 en las texturas superficiales. Son patrones procedurales propios, no recursos descargados ni modificaciones de los PNG originales del equipo. No se a?adieron normal maps a todos los materiales ni brillo global.

Los remates y perfiles repetidos se combinan por familia en mallas compartidas. Ventanas permanecen opacas, con luces urbanas distantes agrupadas en una malla; no hay reflejos realtime ni transparencias extensas.

### Iluminaci?n

Se conserva la iluminaci?n de techo baked y la ?nica direccional realtime existente. Acento del puesto ajustado a la l?mpara; cuatro ?reas baked en los escritorios y una en supervisi?n, sin luces din?micas por cub?culo. Total final verificado por recibo: 17 baked, 1 realtime, 81 sondas. Una sonda de reflexi?n **baked de 128?**, proyectada a la habitaci?n, aporta lectura al metal/pol?mero; no se actualiza en tiempo real. Se renueva el horneado despu?s del refinamiento.

La lectura sigue siendo estilizada y m?s simple que los artes. No se afirma reproducci?n exacta de objetos, sombras o detalle cinematogr?fico de las referencias. La mejora debe evaluarse en las parejas antes/despu?s; aceptaci?n art?stica del usuario pendiente.

## Modelos y licencias

[AVAILABLE_3D_ASSETS.md](../../../Context/AVAILABLE_3D_ASSETS.md) documenta los siete ZIP, 145 fuentes FBX/OBJ de Models, cuatro OBJ del ejemplo Cardboard, autores, origen, licencias, condiciones, candidatos y estado de importaci?n. Fuentes de licencia actuales enlazadas all? y en ASSET_CREDITS.

Se aprovechan por primera vez en esta oficina `desk.fbx` y `bookcaseOpen.fbx`, ya importados del Furniture Kit CC0. Se reutilizan chairDesk, computerScreen, computerKeyboard, books, plantas, sof? y mesas acreditadas. Las mallas GI/UV auxiliares son derivados locales dentro de VisualPolish; los modelos/importadores fuente quedan intactos.

Office Pack contiene 125 archivos de malla. Cinco son id?nticos por SHA-256 a Computer, Desk, Printer, Paper y Stapler con procedencia individual documentada; siguen disponibles desde sus carpetas originales. Otros 20 emparejamientos son auxiliares: no autorizan las mallas con las que coinciden. **120 mallas siguen pendientes de autor/fuente/licencia verificables**. No se incorporan al contenido distribuible ni se presupone permiso por el fin acad?mico.

## Calidad URP y l?mites de comparaci?n

Se revisan perfiles existentes: PC renderScale 1 / MSAA 1 y Mobile renderScale 0,8 / MSAA 1; Android selecciona Mobile, mientras el Editor estaba en PC. Parte del dentado procede de esta ausencia de MSAA y la resoluci?n reducida, pero no explica las sillas/objetos/materiales mal compuestos.

No se modifican los assets URP, calidad global, XR ni paquetes. La comparaci?n controlada se hace temporalmente en Play usando los dos assets existentes, con la misma c?mara/contenido y restauraci?n del perfil. PNG monosc?pico no demuestra coste GPU, FPS o confort de Cardboard. Contadores del Editor, si est?n disponibles, incluyen su renderizado y no equivalen a draw calls aislados de Android. No se confunden con un benchmark.

Pendiente en HONOR: medir frame time, GPU/CPU, batches/draw calls, memoria de lightmaps/reflexi?n y temperatura. Solo despu?s valorar MSAA 2? frente a renderScale actual y conservar la alternativa ligera. No se sube calidad a ciegas ni se a?aden bloom, SSAO o reflejos din?micos.

## Pruebas y capturas

Galer?a: `Docs/Previews/VisualPolishReview/20261103-BankPolish/`. Before y After conservan ocho c?maras equivalentes; la vista 07 se corrigi? por oclusi?n y no es una pareja comparable; Placement y las iteraciones intermedias se conservan. Play contiene cierre, puesto de pie/sentado y libreta/teclado. Profiles compara la misma c?mara con PC y Mobile. Todos los renders son Unity real, no IA; las plantas ocultan temporalmente el techo y lo restauran.

DomainTests: 27 PASS. La revisi?n espec?fica incluye seis sillas, apoyo, cierre por mirada una vez, rechazo de turno incompleto, raycasts desde asiento, TMP y sentado/de pie. Las sondas existentes comprueban el pr?logo omitido/autom?tico, ambos modos, decisiones, cinco interacciones, ocho destinos, retorno y cuerpo libre. Consultar resultados FINALES en README/recibos, no inferir PASS de pruebas parciales.

Incidencias de herramientas durante la sesi?n, conservadas en logs: correcci?n de API Unity 6 Area?Rectangle y uso de enabled/activeInHierarchy para MeshRenderer; espera de IEnumerator de Editor corregida a frames/tiempo real; comparaci?n inicial por ?ndices de Quality fall? y se sustituy? por assets expl?citos; escritura de command.txt sufri? una lectura concurrente, corregida mediante entrega at?mica y reintento de lectura. Ninguna de estas incidencias cambia una regla bancaria. Los resultados fallidos originales se conservan, no se presentan como pruebas finales superadas.

## Protecci?n y archivos afectados

Respaldo inicial completo: `D:/UnityGames/SWIFT_Backups/VisualPolish-20261103-130232`. Checkpoints de escena/.meta/lightmaps tras cada fase, incluidos guardados expl?citos del estado en memoria. Restaurar selectivamente con Unity cerrado, conservando posteriores cambios ajenos; no reset/clean ni checkout destructivo.

Archivos de implementaci?n: escena BancoCorresponsal e iluminaci?n asociada; nuevo Editor/VisualPolishReview.cs y runtime UI/ArchitecturalFocusVisual.cs; materiales, cuatro texturas y mallas derivadas propias en OfficeVisual/VisualPolish. Documentaci?n: este informe, AVAILABLE_3D_ASSETS, ASSET_CREDITS, KANBAN, HONOR_TEST_CHECKLIST y VisualReview/CHANGELOG, m?s galer?a y recibos.

Se conserva el historial y el conjunto local previo. La comparaci?n de hashes inicial/final y los metadatos/GUID se documentan al cierre. No se marca aceptaci?n visual ni Android como completados.

## Pendientes y recursos del usuario

- **Implementado:** correcciones de orientaci?n, apoyos, cierre contextual, distribuci?n, materiales/arquitectura, aros y nuevo bake; validaci?n final seg?n recibos.
- **Pendiente de dispositivo:** APK nueva, confort de pantalla ancha/giro, estereoscop?a/fade/pose, lectura de aros y perfiles de rendimiento.
- **Bloqueado por licencia:** 120 mallas de Office Pack; aportar p?ginas individuales o autorizaci?n del titular si se quieren usar.
- **Bloqueado por archivos:** ninguno para esta fase. Las tres capturas s? llegaron y fueron revisadas.
- **Recursos realmente necesarios:** no hay modelos o texturas imprescindibles nuevos. Audio final sigue pendiente de la tarea correspondiente.
- **Opcional:** l?mpara articulada y accesorios institucionales m?s espec?ficos con procedencia verificable, despu?s de aceptar esta comparaci?n y medir Android.

## Cierre verificado de la sesi?n

- PASS bake final: 2026-11-03T18:38:17Z, con reflexi?n baked vinculada en ReflectionProbe-0.exr.
- PASS flujo 18:40:38Z, oficina 18:40:50Z, smoke 18:41:01Z y entorno 18:41:17Z. Pr?logo omitido y autom?tico de 12 s, ambos modos, resultados, decisiones 6/6 y nueve roles de interacci?n/raycast. Se conservan los ocho destinos, retorno y clearance de cuerpo.
- Despu?s se retiraron solo dos placas UI redundantes (Cubicle route 0/1), sin collider ni GI. Se guard? checkpoint y se repiti? revisi?n espec?fica: PASS 18:43:59Z; seis sillas, cierre por mirada una vez, turno incompleto bloqueado, seis raycasts sentados, TMP, proveedor de pose y levantarse/fade.
- DomainTests: 27 PASS en esta sesi?n. No se realizaron pruebas Android ni se gener? APK.
- Configuraci?n, paquetes, XR, 326 archivos de Models, n?cleo de banco/pr?logo/VR/interacci?n, prefabs y artes originales: **488 archivos protegidos id?nticos** al respaldo inicial. GUID de escena conservado. Detalle: preservation-check.json.

| Inventario (no benchmark GPU) | Inicial | Final |
|---|---:|---:|
| Planta | 20 ? 18 m | 20 ? 18 m |
| Cub?culos secundarios | 4 | 4 |
| Sillas revisadas | 5 | 6 |
| Teletransportes | 8 | 8 |
| MeshRenderer activos/habilitados | 270 | 284 |
| Materiales de malla distintos | 45 | 49 |
| Tri?ngulos, todas las mallas activas incluidas ocultas | 27.572 | 31.360 |
| Tri?ngulos con Renderer habilitado, final | ? | 31.228 |
| Luces baked / realtime | 12 / 1 | 17 / 1 |
| Sondas de luz | 75 | 81 |
| Reflexi?n baked | 0 | 1 ? 128? |
| Atlas de lightmap | 1 | 1, direccional 1024? |

El archivo inicial de escena coincide por SHA-256 con el manifest de ExpandedBankOffice: esas m?tricas de partida pertenecen exactamente al estado respaldado, no a una escena distinta. Geometr?a final: +3.788 tri?ngulos activos (+13,7%) y +14 MeshRenderer habilitados. Mayor inventario no certifica FPS; las mallas combinadas de remates evitan un renderer por perfil.

Comparaci?n controlada final en Editor, instant?neas globales (incluyen renderizado del Editor): PC 351 draw calls / 323 batches / 36.280 tri?ngulos; Mobile 185 / 171 / 19.620. Son contadores de frames diferentes, no draw calls aislados de la c?mara temporal ni medici?n de GPU Android, y no justifican afirmar una mejora porcentual de rendimiento. Se adjuntan assets activos, renderScale/MSAA/sombras y PNG de la misma c?mara. Frame Debugger y Profiler de Android siguen **NO EJECUTADOS**.

El log conserva fallos intermedios de herramientas y avisos del Editor/SDK; no se borra el historial de Console para aparentar una sesi?n limpia. Las compilaciones finales y revisiones de Play pasan. El rechazo del guard por escena sucia se resolvi? guardando la memoria con otra copia externa, nunca descartando ediciones.

?ltima repetici?n de captura/revisi?n espec?fica: PASS 2026-11-03T18:51:32Z, seguida de guardado final PASS 18:51:33Z. Se corrigi? el capturador para ocultar/restaurar la ret?cula del observador solamente en c?maras externas de detalle; evita el r?tulo de rearme visto de canto. En capturas desde el ojo se conserva. No cambi? ning?n componente de interacci?n/VR por este ajuste.

Checkpoint final completo: `D:\UnityGames\SWIFT_Backups\VisualPolish-20261103-130232\FinalCheckpoint-20261103T185430Z` (Assets, Packages, ProjectSettings, Docs y recibos de esta sesi?n). 488 archivos protegidos revalidados al cierre. Lista exacta del delta respecto al inicio: Artifacts/VisualPolish/task-file-delta.json.
