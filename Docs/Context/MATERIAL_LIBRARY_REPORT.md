# Biblioteca de materiales — SWIFT: Código de Alerta

2026-11-03. Unity 6000.3.24f1 / URP 17.3.0. Rama review/environment-immersive, base 1074fdd. Sin commit ni push.

## Resultado

Catálogo aislado en `Assets/_Project/MaterialLibrary/Showcase/MaterialLibrary_Studio.unity`. **12 materiales existentes reutilizados; ninguna nueva variante de las seis familias necesaria.** Se añade únicamente `Studio_NeutralGrey_Matte.mat`, un gris acromático de calibración para fondo y soportes del estudio (URP/Lit, smoothness 0,08, metallic 0, sin textura). No es una sustitución de materiales de oficina.

Los nuevos recursos están bajo `Assets/_Project/MaterialLibrary/`, con sus .meta. El índice JSON documenta rutas y referencias originales. Los 12 materiales siguen en `Assets/_Project/OfficeVisual/GraphicsPolishV2/Materials/`: **se referencian, no se copian ni se mueven**, para evitar duplicados y conservar GUID/referencias de la revisión anterior.

## Inspección y selección

Se inventariaron 431 materiales/subassets en OfficeVisual y Models, y 227 bindings renderer/material de modelos importados. Archivos locales: Artifacts/MaterialLibrary/existing-materials.tsv, model-material-bindings.tsv y equivalent-material-groups.txt. La comparación de equivalencia considera shader, propiedades, texturas/tiling, render queue y keywords; agrupa subassets importados equivalentes, sin borrarlos ni modificar modelos. No se asegura batching por compartir un nombre.

Se revisó el inventario de modelos y créditos: escritorio Quaternius, sillas/archivos/libros/marcos Kenney, equipos Google, impresora Bruno Oliveira y papeles Jarlan Perez. El estudio usa primitivas de Unity; no descarga ni extrae modelos. Office Pack sin licencia verificable queda fuera. No se modifican importadores, prefabs o artes.

La biblioteca V2 ya diferenciaba madera, plásticos, metal, textiles, papel y arquitectura. Por ello no se genera una tercera biblioteca de clones. Se eligen dos variantes por familia para comparar con claridad:

| Familia | Material reutilizado | Smoothness / Metallic | Aplicación recomendada |
|---|---|---|---|
| MADERA | `Wood_DesaturatedOak` | 0.280 / 0.00 | Tableros y estanterias |
| MADERA | `Wood_WalnutInset` | 0.230 / 0.00 | Cajones e interiores |
| PLASTICO | `Plastic_EquipmentMatte` | 0.180 / 0.00 | Monitor, PC e impresora |
| PLASTICO | `Plastic_Keycaps` | 0.120 / 0.00 | Teclados y controles fisicos |
| METAL PINTADO | `Metal_GraphitePaint` | 0.380 / 0.35 | Marcos, soportes y patas |
| METAL PINTADO | `Metal_ArchiveEnamel` | 0.320 / 0.15 | Archivadores y gabinetes |
| TELA | `Fabric_AcousticSlate` | 0.040 / 0.00 | Mamparas y separadores |
| TELA | `Fabric_SeatCharcoal` | 0.060 / 0.00 | Sillas y sofa |
| PAPEL | `Paper_Ivory` | 0.025 / 0.00 | Hojas y libretas |
| PAPEL | `Paper_StackEdges` | 0.025 / 0.00 | Pilas de documentos |
| ARQUITECTURA | `Architecture_Mineral` | 0.080 / 0.00 | Paredes y revestimientos |
| ARQUITECTURA | `Architecture_CarpetSlate` | 0.015 / 0.00 | Suelo de puestos |

## Otros recursos existentes que conviene conservar

No se duplican ni se muestran como materiales nuevos: `Metal_AluminiumTrim` para herrajes/remates; `Architecture_CeilingIvory` para techo; `Architecture_CeilingJoint` para juntas; `Architecture_ColumnMineral` para columnas; `Architecture_CarpetAisle` para circulación. El material Unlit `Exterior_WarmWindows` pertenece al exterior y no forma parte de estas muestras Lit.

Los materiales V1 y los importados permanecen para las escenas/prefabs que los usan. Una futura consolidación exige revisar referencias antes de migrarlos; no se eliminan como supuesto duplicado.

## Escena y capturas

Luz clave y relleno blancos, ambiente gris neutro, sin color grading ni postprocesado. La reflexión de 128² se hornea sobre el estudio gris antes de incluir las muestras de color; no es realtime. El fondo no recibe/proyecta sombras que alteren la comparación. Las formas sí permiten apreciar cambios de luz y brillo. Dos direccionales se usan **solo en esta muestra**, no se añaden al banco.

Cada variante tiene placa plana, esfera, cubo girado y cilindro; todos usan el mismo material compartido. Las etiquetas TMP muestran nombre, smoothness (S) y metallic (M). Los números visibles redondean a dos decimales; el índice conserva precisión.

[Galería real](../Previews/MaterialLibrary/README.md): siete PNG nuevos 1600 × 900, vista general y seis familias. Son renders reales de Unity Editor DX11 con PC_RPAsset; no imágenes IA, ni capturas Android. InitialLighting conserva siete renders de la iteración inicial, con iluminación insuficiente; no son los finales. Recibos y hashes en la galería.

## Rendimiento y aplicación

- Los 12 materiales seleccionados usan **URP/Lit opaco** y smoothness ≤0,38; no hay vidrio, brillos espejo o shaders nuevos.
- Reutilizan tres patrones existentes de 256², con mipmaps, trilineal, aniso 2 y compresión. No se crea ninguna textura de superficie nueva ni mapas enormes. TMP reutiliza su fuente existente.
- No se añaden normal maps, transparencias amplias, emisión, postprocesado ni dependencias. La única textura nueva es el cubemap neutro 128² del estudio.
- No se modifican URP, calidad global, XR/Cardboard, paquetes ni Build Profiles. La muestra no se añade a compilación.
- Para aplicar en el futuro: usar sharedMaterial/overrides controlados, respetar submeshes y UV de cada modelo. No reemplazar a ciegas todos los materiales de impresora/teclado/documentos: se perderían detalles o partes diferenciadas.
- Moqueta conserva el tiling 20 × 18 previsto para la sala. La densidad de texel debe ajustarse al tamaño físico/UV al aplicarla en otro modelo; no se crea una variante por objeto para corregir escala.
- El cubemap y las dos luces del estudio son herramientas de comparación, no una configuración propuesta para cada escritorio Android.

Pendiente en HONOR: aliasing/moire del tejido y veta en movimiento, brillo percibido, compresión/tiling en superficies reales y coste GPU/CPU/memoria. No se afirma rendimiento Android por estas capturas.

## Validación y protección

PASS Unity: shaders compatibles, opacidad, brillo controlado, texturas de muestras ≤256², una cámara sin XR, cero colliders/juego y rótulos sin overflow; reflexión horneada y render requests URP completados. Resultado final: `PASS reopen 2026-11-03T19:36:49.5127810Z Unity=6000.3.24f1 EDITOR ONLY; ANDROID NOT TESTED`.

Se corrigieron dos incidencias del propio catálogo: altura inicial de rótulos y sombras del techo sobre las muestras. Los fallos iniciales se conservan en Artifacts/MaterialLibrary; no se atribuyen al banco ni se ocultan como pruebas pasadas.

**1058 archivos previos de Assets/Packages/ProjectSettings permanecen idénticos por SHA-256**, incluidas BancoCorresponsal, su iluminación, revisión V2, materiales/modelos previos, lógica, URP y Cardboard. Todos los recursos nuevos tienen .meta. No se ejecutaron sondas bancarias en esta sesión: el banco no se modificó. Tampoco se generó APK ni se probó Android.

Nuevo helper solo Editor: `Assets/_Project/Editor/MaterialLibraryReview.cs`, con comandos explícitos; no genera contenido automáticamente al importar. Escena, índice, material neutro y reflexión se guardan en MaterialLibrary. Copia preventiva de escena principal/meta y manifiesto inicial: `D:\UnityGames\SWIFT_Backups\MaterialLibrary-20261103-142424`. Evidencias en Artifacts/MaterialLibrary y Docs/Previews/MaterialLibrary.

El Editor queda mostrando el catálogo. Siguiente paso: revisar la galería y elegir materiales; cualquier aplicación a BancoCorresponsal corresponde a una tarea posterior.

## Entrega final

Se finaliza la preparaci?n por indicaci?n del usuario. No se ejecutan m?s verificaciones en Unity ni acciones con GitHub. Las comprobaciones anteriores se conservan como historial; la revisi?n visual y validaci?n posterior quedan a cargo del usuario. No se aplic? ning?n material a BancoCorresponsal.
