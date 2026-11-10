# Créditos de modelos — Oficina bancaria (2026-10-28)

## Reutilización en el rediseño inmersivo — 2026-11-02

`computerScreen.fbx` y `lampSquareTable.fbx` proceden del mismo Furniture Kit 2.0 de Kenney (CC0) ya incluido. Se crearon dos prefabs normalizados nuevos en OfficeVisual/EnvironmentRedesign; los originales no se sobrescriben. Cubículos reutilizan escritorio Quaternius y accesorios ya acreditados. Mamparas, consola, marcos, clipboard, señales e iconos son geometría/UI propia realizada con asistencia de Codex; las pantallas funcionales usan TMP y no imágenes IA con datos incrustados.

## Artes suministrados por el equipo — integración 2026-11-02

- `Assets/_Project/Art/WallGraphics/ART_BankWallGraphic.png`: gráfica institucional ficticia proporcionada por el usuario, identificada en su solicitud como arte generado con IA. Original conservado; usado en un cuadro mate URP de geometría propia.
- `Assets/_Project/Art/Prologue/Reference/`: storyboard e imágenes individuales proporcionados por el usuario, identificados como generados con IA. Se usan los dos primeros individuales con recorte UV del relleno; el compuesto y la tercera ilustración se conservan como referencias. No se atribuye una licencia comercial externa no aportada.
- `Assets/_Project/Art/Integrated/Prologue_BancoReal.png`: render derivado de BancoCorresponsal en Unity, generado localmente para el tercer momento. Conserva las atribuciones y licencias de los modelos que aparecen, detalladas abajo. No es una fotografía ni evidencia de un banco real.

Informe y evidencia: [ART_INTEGRATION_REPORT.md](../Archive/2026-11-10/Context/ART_INTEGRATION_REPORT.md).

## Modelos originales

ZIP originales conservados en `External Assets/` (nombre real con espacio). Inventario
completo y hashes en `Artifacts/Office20261028/`. Cada paquete compatible se extrajo en
su propia carpeta de `Assets/Models/`, conservando OBJ/MTL/texturas y nombres originales.

| Paquete | Autor | Fuente | Licencia verificada | Uso |
|---|---|---|---|---|
| Furniture Kit 2.0 | Kenney | https://kenney.nl/assets/furniture-kit | CC0 1.0; License.txt dentro del ZIP | Silla, archivadores, teclado, mouse, puerta, sofá, plantas y accesorios. Se importó la variante FBX. |
| Desk | Quaternius | https://poly.pizza/m/V86Go2rlnq | CC0 1.0, ficha de origen | Escritorio principal FBX. |
| Computer | Poly by Google | https://poly.pizza/m/2EHvZLax4Y3 | CC BY 3.0, enlace exacto comprobado en ficha/HTML | Computadora OBJ, MTL y Computer Texture.png, conservados juntos. |
| Office Printer / Copier | Bruno Oliveira | https://poly.pizza/m/bgNnmejxBa- | CC BY 3.0, enlace exacto comprobado en ficha/HTML | Impresora OBJ y materiales MTL. |
| Small Stack of Paper | Jarlan Perez | https://poly.pizza/m/aiBozYlPe-- | CC BY 3.0, enlace exacto comprobado en ficha/HTML | Pilas de papel OBJ/MTL; todas las órdenes del juego son ficticias. |
| Stapler | Poly by Google | https://poly.pizza/m/cSUMLFBEzi3 | CC BY 3.0, enlace exacto comprobado en ficha/HTML | Grapadora OBJ/MTL. |
| Office Pack-zip.zip | Autor/agregador no identificado | No incluido en ZIP; pendiente aportar página de descarga | No incluye licencia ni créditos: sin verificar | Inspeccionado, no importado ni utilizado. Contiene varios modelos duplicados y recursos con marcas; se prefieren los paquetes individuales verificables. |

Licencias: https://creativecommons.org/licenses/by/3.0/ y
https://creativecommons.org/publicdomain/zero/1.0/ . Las atribuciones CC BY incluyen
nombre, autor, enlace y licencia. Adaptaciones realizadas: escala/unidades, orientación,
materiales URP y colliders de selección; los archivos fuente extraídos se conservan.

Los GLB de Kenney no se importaron: el proyecto no declara glTFast/UnityGLTF y cada modelo
ya dispone de FBX nativo. No se instaló ningún paquete. Se omitieron vistas isométricas,
variantes STL/DAE/OBJ redundantes del kit, PSD y archivos de respaldo .bak.

Originales de escena preservados por copia de seguridad y jerarquía Legacy_Prototype_Preserved.
Reloj, hoja de puerta, estructura de sala y paneles planos son geometría provisional propia.
Los muebles no representan marcas bancarias reales. Créditos visibles dentro de la escena.
Código y montaje asistidos por OpenAI Codex.


## Pulido art?stico ? 2026-11-03

Inventario renovado: [AVAILABLE_3D_ASSETS.md](AVAILABLE_3D_ASSETS.md). Se a?aden usos de desk.fbx y bookcaseOpen.fbx ya importados de Kenney CC0, sin descargar modelos ni modificar sus fuentes. Geometr?a de remates/aros, patrones de roble/tejido/moqueta de 256? y pantalla auxiliar abstracta de 128? son creaci?n procedural propia asistida por Codex. No contienen datos bancarios ni reemplazan TMP. Cinco mallas de Office Pack coinciden por hash con los paquetes individuales acreditados; no se importan duplicados. Las restantes 120 mallas no verificadas siguen fuera de la escena/build.


## Graphics Polish V2 ? 2026-11-03

Se reutilizan ?nicamente modelos ya acreditados, incluido un grupo adicional Kenney_Books (Furniture Kit CC0) y papeles de Jarlan Perez. Los libros existentes de cub?culos se recolocan sobre sus tableros. Materiales compartidos, perfiles de techo/ventana/columna/mampara y ventanas urbanas agrupadas son derivados propios creados con Unity y asistencia de Codex en OfficeVisual/GraphicsPolishV2. Reutilizan los patrones procedurales anteriores sin modificar sus archivos ni las fuentes de terceros. No se descargan im?genes/modelos ni se usa Office Pack sin licencia. Revisi?n aislada y evidencia en GRAPHICS_POLISH_V2_REPORT.md.

### 2026-11-10 — Herramientas inmersivas

Teléfono de verificación: geometría procedural propia creada con primitivas Unity y combinada en tres meshes; reutiliza Plastic_EquipmentMatte, Plastic_Keycaps y Metal_AluminiumTrim de la biblioteca existente. Portapapeles: copia de revisión del modelo y materiales ya presentes, con sus créditos anteriores conservados. UI de inspección: Unity UI/TMP, fuentes y sprites institucionales existentes. No hay nuevos modelos, texturas ni audios descargados. El catálogo SIM-003 es contenido ficticio de prueba basado en la propuesta de Docs/Research, pendiente de aprobación editorial; no es una fuente histórica ni una grabación auténtica.