# Recursos del rediseño de la oficina

Fecha: 2026-11-02. Unity 6000.3.24f1 / URP 17.3.0. Inventario directo de Assets y del Editor; no se compraron ni descargaron recursos.

## Existe y se reutiliza

| Recurso | Ruta existente | Uso |
|---|---|---|
| Escritorio Quaternius | Assets/_Project/OfficeVisual/Prefabs/Quaternius_Desk.prefab | Analista y cuatro puestos secundarios; CC0 |
| Computadora Google | Assets/_Project/OfficeVisual/Prefabs/Google_Computer.prefab | Puesto funcional, con textura conservada; CC BY 3.0 |
| Silla, teclado, mouse, archivo, libros, plantas | Assets/_Project/OfficeVisual/Prefabs/Kenney_*.prefab | Puestos, archivo y ambiente; CC0 |
| Monitor independiente | Assets/Models/KenneyFurniture/Models/FBX format/computerScreen.fbx | Monitores visuales secundarios; CC0 |
| Lámpara de mesa | Assets/Models/KenneyFurniture/Models/FBX format/lampSquareTable.fbx | Accesorio sin luz dinámica por escritorio; CC0 |
| Impresora Bruno Oliveira | Assets/_Project/OfficeVisual/Prefabs/Bruno_Printer.prefab | Evidencia existente; CC BY 3.0 |
| Papel Jarlan Perez y grapadora Google | Assets/_Project/OfficeVisual/Prefabs/Jarlan_Papers.prefab; Google_Stapler.prefab | Documentos ordenados; CC BY 3.0 |
| Propuesta C | Assets/_Project/Styles/OperationsCenter_Original.png | Referencia de composición; UI reconstruida mediante componentes |
| Referencias del usuario | Assets/_Project/Art/References/DiseñoVisual_PC.png; DiseñoVisual_Completo.png | Dirección visual, no fondo funcional |
| Cuadro y prólogo | Assets/_Project/Art/WallGraphics y Prologue | Se conservan los originales y su atribución a IA |

Las licencias originales y sus fuentes están en ASSET_CREDITS.md. No usar Office Pack sin licencia verificable.

## Puede construirse en Unity

| Nombre / tipo | Dimensiones aproximadas | Superficies / uso | Prioridad / destino |
|---|---|---|---|
| Mampara acústica, geometría Unity | 2,7 × 1,56 × 0,07 m | Tela petróleo y metal pintado, compartidos | Alta; escena, EnvironmentRedesignV1/02_SecondaryCubicles |
| Consola de entrada, geometría + UI/TMP | 2,85 × 2,1 × 0,65 m | Polímero marino, ámbar/turquesa; dos acciones originales | Alta; escena, 01_ModePortalConsole |
| Monitor principal y consulta, geometría + UI/TMP | 1,77 × 1,32 m y 1,14 × 0,99 m | Polímero mate y textos dinámicos | Alta; escena, 03_AnalystWorkstation |
| Clipboard, geometría + TMP | 0,35 × 0,48 m | Papel marfil y clip metálico; abre procedimiento ampliado | Alta; escena, Analyst review clipboard |
| Iconos y estados | Elementos UI | Trazos de check/pausa/devolución; sin nuevas fuentes | Alta; BankDocumentUI |
| Guías de circulación | Tiras de geometría | Señalización discreta, sin collider | Media; escena |

Son piezas propias creadas para el proyecto, sin licencia externa adicional. Materiales/prefabs nuevos bajo `Assets/_Project/OfficeVisual/EnvironmentRedesign/`. No se sobrescriben los prefabs originales.

## Requiere un modelo 3D adicional

**Ninguno imprescindible.** Los modelos existentes cubren el alcance actual. No hace falta entregar nuevos FBX/OBJ para terminar esta distribución.

## Textura o arte 2D / opcional

| Nombre | Tipo / dimensiones | Material y uso | Prioridad, licencia y entrega |
|---|---|---|---|
| Tejido acústico discreto | PNG tileable, 256–512 px, escala física 0,5 m | Sustituir color liso de mamparas si se aprueba; sin detalle de alto contraste | OPCIONAL. Crear con IA o propio, derechos de uso documentados. `Assets/_Project/Art/Environment/AcousticFabric_512.png` |
| Madera de escritorio estilizada | PNG tileable, 512 px, escala 1 m | Variación suave de roble; no marcas ni texto | OPCIONAL. IA/propio o CC0 verificable. `Assets/_Project/Art/Environment/Oak_512.png` |

No generar interfaces con números ficticios ni pantallas completas. Mantener datos editables mediante TMP. Antes de incorporar cualquier descarga, registrar autor, URL, licencia y modificaciones.

## Audio pendiente, independiente del mobiliario

Ya existe la biblioteca de cues y tonos TEMP. Faltan los ocho SFX finales, ambiente de oficina en bucle y música opcional descritos en UX_GAMEPLAY_PROGRESS.md. Entregar clips propios o con licencia compatible y fuente; no se requieren para compilar la escena ni se descargan en esta tarea.


## Ampliaci?n real ? 2026-11-03

No se requiere ning?n modelo nuevo para la oficina de 20 ? 18 m. Se reutilizan los cuatro cub?culos y muebles existentes. Ventanas nocturnas opacas, siluetas urbanas, columnas, mamparas de supervisi?n y carcasa inclinada son geometr?a propia; no utilizan reflejos, transparencias extensas ni nuevas dependencias. Cinco materiales compartidos nuevos en `Assets/_Project/OfficeVisual/ExpandedBankOffice/Materials/`.

Se actualizan las dimensiones orientativas de la tabla anterior: marco del monitor principal 2,22 ? 1,32 m, UI ?til 2,10 ? 1,20 m. La consola tiene un frente de 2,85 ? 1,22 m inclinado 25?; no es ya un panel vertical de 2,1 m de altura.

| Nombre | Tipo | Uso | Prioridad | Formato y dimensiones | Ruta propuesta | Alternativa actual |
|---|---|---|---|---|---|---|
| AcousticFabric_512 | Textura repetible | Tela discreta de mamparas | Opcional | PNG, 512 ? 512, sin texto | Assets/_Project/Art/Environment/AcousticFabric_512.png | Chair_Fabric compartido |
| Oak_512 | Textura repetible | Variaci?n suave de madera | Opcional | PNG, 512 ? 512 | Assets/_Project/Art/Environment/Oak_512.png | Oak_Worksurface compartido |
| NightSkyline_1024 | Arte 2D | Mayor variedad en ventanas | Opcional, despu?s de medir Android | PNG opaco, 1024 ? 512 | Assets/_Project/Art/Environment/NightSkyline_1024.png | Siluetas geom?tricas actuales y Night_Window |

Generar material propio/IA o utilizar licencia compatible documentada. No se descargaron recursos. El audio definitivo sigue siendo una tarea independiente; los tonos temporales no impiden evaluar la arquitectura.
