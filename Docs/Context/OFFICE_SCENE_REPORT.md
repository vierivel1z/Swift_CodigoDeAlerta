> **Documento histórico (28/10/2026).** Se conserva esta ruta por compatibilidad. Para el estado, pruebas y APK vigentes, consulta [el informe actual](IMMERSIVE_AUTONOMOUS_PROGRESS.md).

# Oficina bancaria con modelos importados — 2026-10-28

## Construcción
Se inspeccionaron siete ZIP de External Assets (nombre real con espacio). Se importaron
seis paquetes verificables en Assets/Models: 141 FBX, cuatro OBJ con sus cuatro MTL y
una textura PNG, más licencias/referencias. Kenney proporciona variantes GLB, pero no
hay importador glTF declarado y se usó FBX nativo, sin instalar paquetes.
Office Pack-zip.zip quedó intacto y fuera de Assets por falta de autoría/licencia global.
Créditos completos: ASSET_CREDITS.md y Resources/OfficeAssetCredits.txt.

BankOfficeBuilder crea quince prefabs normalizados y materiales URP propios sin
modificar las mallas originales. Importa sin animación ni MeshCollider automático;
se añaden BoxCollider para selección. Conserva las relaciones OBJ/MTL/texturas y
compensa los materiales con textura y diffuse negro para mostrar el atlas correctamente.

BancoCorresponsal se actualizó mediante APIs de Unity, conservando el GUID de escena.
Antes se creó una copia en Scenes/Backups. La geometría original también permanece
inactiva en 00_Systems/Legacy_Prototype_Preserved. Sistemas, cámara, XR y lógica
bancaria se reutilizan. La cámara comienza a 1.6 m en entrada. Un Canvas World Space
compacto anclado a la computadora se guarda con sus referencias TMP en la escena.

## Distribución
- A: puerta/marco Kenney, hoja provisional, cartel lateral, portales de modo y punto inicial.
- B: escritorio Quaternius, computadora Google, teclado, mouse, silla, papeles y grapadora.
- C: impresora Bruno Oliveira, dos archivadores Kenney y mesa con papeles ficticios.
- D: paneles de alertas/defensa separados y terminal de cierre.
- Sofá, plantas, libros, mesa de visitas, papelera y reloj completan la oficina.
- Cuatro destinos de teletransporte; VR no tiene locomoción continua. Mouse/WASD se
  mantienen para depuración PC. No se añade seguimiento de manos.

## Evidencia verificada
Unity 6000.3.24f1 y URP 17.3.0. Compilación y generación/reapertura de escena: PASS.
Play Mode: una cámara, un Canvas bancario serializado, computadora, impresora/evidencia,
registro, alertas, defensa, decisión registrada y cuatro destinos: PASS.
Capturas revisadas y disposición corregida (cartel de entrada, escritorio, paneles y puerta).
Aproximadamente 10.6 mil triángulos de malla activos, una luz direccional sin sombras;
esto no es una medición de FPS ni temperatura en Android.
Materiales de MeshRenderer compatibles con URP y colliders de acciones comprobados;
las fuentes/UI mantienen sus shaders TMP/UI. Los materiales nativos originales se
conservan como datos de importación, pero los prefabs de escena utilizan las copias URP.

Logs, inventario ZIP, hashes y capturas: Artifacts/Office20261028 (no se publican como cachés).
Se comprobaron hashes de los ZIP originales y de Packages/ProjectSettings: sin cambios
por esta tarea. La versión/configuración XR que el usuario tenía se respetó.

## Pendiente
- Probar en el visor real: tamaños, distancias, QR, orientación, selección, audio y rendimiento.
- Compilar una APK nueva si se quiere probar esta oficina; la APK de la sesión anterior
  contiene el prototipo previo y no se actualizó en esta tarea visual.
- Reloj, estructura, hoja de puerta y paneles siguen siendo provisionales.
- Las pestañas particulares del profesor continúan pendientes (K01).

Para abrir: Assets/_Project/Scenes/BancoCorresponsal.unity y Play. La herramienta de
Editor no reconstruye una oficina ya marcada BankOfficeVisualV1: conserva tus ediciones.
Los métodos de prueba/refinamiento se utilizan solo en la copia aislada, no durante Play normal.
