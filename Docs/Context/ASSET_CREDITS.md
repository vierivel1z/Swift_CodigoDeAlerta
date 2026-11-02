# Créditos de modelos — Oficina bancaria (2026-10-28)

## Artes suministrados por el equipo — integración 2026-11-02

- `Assets/_Project/Art/WallGraphics/ART_BankWallGraphic.png`: gráfica institucional ficticia proporcionada por el usuario, identificada en su solicitud como arte generado con IA. Original conservado; usado en un cuadro mate URP de geometría propia.
- `Assets/_Project/Art/Prologue/Reference/`: storyboard e imágenes individuales proporcionados por el usuario, identificados como generados con IA. Se usan los dos primeros individuales con recorte UV del relleno; el compuesto y la tercera ilustración se conservan como referencias. No se atribuye una licencia comercial externa no aportada.
- `Assets/_Project/Art/Integrated/Prologue_BancoReal.png`: render derivado de BancoCorresponsal en Unity, generado localmente para el tercer momento. Conserva las atribuciones y licencias de los modelos que aparecen, detalladas abajo. No es una fotografía ni evidencia de un banco real.

Informe y evidencia: [ART_INTEGRATION_REPORT.md](ART_INTEGRATION_REPORT.md).

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
