# Dirección artística — comparación previa a implementación

## Confirmación posterior — 2026-11-03

El usuario confirmó expresamente la correspondencia: `DiseñoVisual_Completo.png` = REF_Office_OperationsRoom.png y `DiseñoVisual_PC.png` = REF_Analyst_Workstation.png. Se utilizan los archivos existentes sin renombrarlos ni duplicarlos. La falta de correspondencia documentada abajo describe la inspección anterior y ya está resuelta. La referencia queda aprobada; la implementación seguirá requiriendo comparación con capturas reales y validación en HONOR.

Nueva prioridad: ampliación arquitectónica medible, consola baja/inclinada y puesto más horizontal con consulta a la derecha. Ver EXPANDED_BANK_OFFICE_REPORT.md cuando finalicen las mediciones y pruebas.

Fecha: 2026-11-02. Rama de trabajo: `review/environment-immersive`. Unity 6000.3.24f1 / URP 17.3.0 / Google Cardboard.

## Estado de las referencias

El usuario aprobó `REF_Office_OperationsRoom.png` y `REF_Analyst_Workstation.png`. **No se localizaron archivos con esos nombres** en Assets, Docs, Artifacts, External Assets, Descargas, Escritorio, Imágenes o adjuntos locales examinados.

En `Assets/_Project/Art/References/` existen dos imágenes anteriores que se abrieron para inspección:

- `DiseñoVisual_Completo.png`: oficina con cubículos, consola de modos central y puesto en primer plano.
- `DiseñoVisual_PC.png`: acercamiento de puesto con dos monitores, libreta y tres decisiones integradas.

Su descripción es compatible con lo solicitado, pero **la correspondencia con los nombres aprobados queda por confirmar**. No se renombran originales ni se atribuye aprobación a un archivo distinto. La comparación siguiente es preliminar, basada en los recursos reales y esas candidatas; no certifica coincidencia con las referencias aprobadas.

## Recursos existentes y reutilizables

| Elemento | Recurso comprobado | Uso y ajuste posible |
|---|---|---|
| Escritorios | Quaternius_Desk.prefab y desk/deskCorner.fbx de Kenney | Reorganizar filas, separar puesto principal y ajustar superficies sin comprar otro pack |
| Sillas y cubículos | Kenney_OfficeChair.prefab; mamparas de EnvironmentRedesignV1 | Mantener asiento funcional y crear ritmo de módulos; variar orientación/accesorios |
| Monitores | Google_Computer.prefab; computerScreen.fbx y Kenney_ReferenceMonitor.prefab | Principal funcional y consulta secundaria; conservar datos TMP y raycasts |
| Impresora y archivo | Bruno_Printer.prefab; Kenney_Archive.prefab; bookcaseOpen/Closed.fbx | Agrupar impresión/archivo en una zona reconocible; estantería abierta disponible |
| Lámparas y plantas | Kenney_DeskLamp.prefab; lampSquareTable.fbx; Kenney_Plant.prefab y plantSmall*.fbx | Contrapunto cálido y separación visual sin luz dinámica por escritorio |
| Papel, libros y accesorios | Jarlan_Papers, Kenney_Books, Google_Stapler | Composición ordenada sobre mesa y estanterías; sin inventario nuevo |
| Libreta | Analyst review clipboard con TMP | Mantener lectura ampliada mediante PROCEDIMIENTO; mejorar encuadre y soporte |
| Ventanas/paneles | wallWindow.fbx y wallWindowSlide.fbx | Marcos existentes; composición arquitectónica con geometría simple |
| Materiales | Oak_Worksurface, Chair_Fabric, Equipment_Polymer, Painted_Metal, Paper_Ivory, Institutional_Navy, Signal_Amber/Teal | Repartir madera cálida, tela petróleo, plástico oscuro y papel claro; evitar colores aleatorios |

Licencias existentes: ASSET_CREDITS.md. No se necesita un modelo 3D nuevo para implementar la composición base. No se utiliza Office Pack de licencia pendiente.

## Diferencias visibles frente a las candidatas

La escena de la revisión anterior ya tiene cuatro cubículos y una consola, pero sus capturas muestran una lectura frontal y bastante simétrica, una consola alta y vertical, grandes paredes lisas y una separación limitada entre luz de trabajo y luz ambiental. La candidata de oficina usa una consola más baja e inclinada, profundidad entre planos, un fondo institucional protagonista y ritmos arquitectónicos más claros.

El puesto actual tiene dos superficies funcionales, pero el principal conserva una proporción cercana a 4:3 y la consulta está a la izquierda; la candidata de escritorio tiene un principal más horizontal y una segunda pantalla a la derecha. No se deben intercambiar o reducir monitores sin comprobar asiento, colliders, oclusión, alcance y tamaños de lectura. La libreta debe seguir siendo apoyo ampliable, sin convertirse en otra copia del expediente.

Estos juicios se basan en las imágenes anteriores abiertas y en la galería real de EnvironmentRedesign, no en una nueva captura ni en archivos canónicos aún ausentes. Aprobar una referencia artística no significa aprobar la escena implementada.

## Qué puede construirse sin assets nuevos

- Reorganizar cubículos/pasillos y controlar las capas de primer plano, consola y fondo.
- Crear una carcasa de consola baja e inclinada con geometría Unity; conservar las dos acciones, el único dwell y el prólogo.
- Diseñar marcos, zócalos, montantes, falso techo modular y división de supervisión mediante geometría simple compartida.
- Reencuadrar el puesto con los dos monitores y clipboard disponibles; reorganizar la interfaz nativa y estados sin imágenes con datos falsos.
- Distribuir materiales existentes por familias físicas y ajustar luces baked por zonas, manteniendo el perfil Mobile.
- Generar iconos geométricos/UI, separadores y rótulos editables mediante TMP.

## Arte nuevo: únicamente si se conserva ese detalle

| Detalle | Recurso faltante / producción | Alternativa Android |
|---|---|---|
| Ciudad nocturna tras ventanas | No hay un panorama urbano dedicado verificado. PNG de 1024–2048 px, propio o con licencia documentada | Fondo opaco de edificios low-poly o tarjetas simples con pocas ventanas; sin escena urbana completa |
| Mapa institucional grande | El cuadro existente es otra composición; no se ha verificado un mapa mundial equivalente | Reutilizar el cuadro del banco o un esquema de rutas ficticias mediante UI; mapa nuevo solo si se pide expresamente |
| Veta de madera y tejido perceptible cerca | Materiales actuales principalmente de color liso; texturas dedicadas opcionales de 256–512 px | Mantener color/roughness y variaciones por panel; no texturas PBR enormes |
| Pequeños archivadores, taza y portalápices de la candidata | No se certifica que exista cada accesorio exacto | Construir formas simples o reutilizar libros/papeles; no son requisitos para la composición |

No se solicita comprar ni descargar recursos todavía. Rutas propuestas para arte nuevo opcional: `Assets/_Project/Art/Environment/`. Para referencias canónicas: `Assets/_Project/Art/References/REF_Office_OperationsRoom.png` y `REF_Analyst_Workstation.png`.

## Simplificaciones de render

| Efecto de referencia | Implementación económica propuesta |
|---|---|
| Cristal con múltiples reflejos | Marcos y panel opaco/ligeramente tintado; transparencia solo si la prueba en dispositivo la justifica |
| Luz cálida bajo consola, lámparas y techo | Emisión contenida y luz horneada; conservar señales de estado con MaterialPropertyBlock |
| Reflejos suaves de suelo | Material mate/satinado compartido; sin SSR ni reflection probes en tiempo real |
| Resplandor, haze y profundidad fotográfica | Contraste de materiales y luces baked; no agregar bloom, volumétricos, DOF o motion blur para imitar una ilustración |
| Muchos puestos detallados | Reutilizar cuatro módulos y accesorios limitados; ampliar cantidad únicamente tras perfilado Android |

## Etapas y aceptación

1. Confirmar identidad/ruta de las dos referencias; registrar hashes y encuadres de comparación. Conservar escena, `.meta`, materiales y recursos en respaldo externo antes de editar.
2. Capturar línea base nueva desde los encuadres equivalentes: general de oficina y acercamiento de puesto, más escritorio/planta de control. Distinguir encuadre cinematográfico de vista real sentado.
3. Ajustar distribución y volumen de consola. Guardar checkpoint y verificar entrada, seis destinos, retorno y dos modos.
4. Ajustar puesto/monitores/UI. Comprobar sentado/de pie, botones, consulta, libreta y marcador LEVANTARSE; mantener KeepPanelInFrontOfComputer.
5. Ajustar materiales e iluminación, hornear y capturar otra sesión con las mismas cámaras. Abrir las imágenes y comparar composición, foco, materiales y lectura; documentar diferencias pendientes.
6. Repetir pruebas pertinentes y registrar PASS/FAIL/NO PROBADO. Solo HONOR puede validar confort, lectura binocular, tracking, frame time y temperatura.

No se ha aplicado una nueva modificación de escena o materiales en esta fase de comparación. No hay nuevas pruebas ni capturas atribuidas a esta solicitud; las de la sesión anterior siguen siendo evidencia histórica de aquella versión.

Respaldo adicional: `D:/UnityGames/SWIFT_Backups/ApprovedReferences-20261102T220031Z`, con escena, `.meta`, iluminación y recursos de entorno. El checkpoint completo anterior `EnvironmentRedesign-20261102-161323/CheckpointFinal-20261102T215544Z` conserva la misma escena, contrastada por SHA256. Recibo: `Artifacts/ApprovedReferencesReview/inspection.json`.
