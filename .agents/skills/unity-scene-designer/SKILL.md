---
name: unity-scene-designer
description: Diseñar, decorar y revisar la oficina 3D de SWIFT en Unity 6/URP para Android Google Cardboard, con capturas reales y correcciones iterativas mediante ramas de revisión.
---

# Unity scene designer

## Contexto obligatorio

Trabaja desde la raíz del checkout existente. Lee primero `Docs/Context/PROJECT_CONTEXT.md` y `Docs/Context/GAME_DESIGN.md`; después `README.md`, `Docs/Context/ASSET_CREDITS.md`, `Docs/Context/OFFICE_SCENE_REPORT.md` y `Docs/VisualReview/README.md`. Respeta instrucciones AGENTS.md aplicables y cambios locales del usuario.

Objetivo: `Assets/_Project/Scenes/BancoCorresponsal.unity`, Unity **6000.3.24f1**, **URP 17.3.0**, Google Cardboard/Android. No cambies el pipeline, XR, paquetes o versiones para resolver un problema visual. Conserva las cinco interacciones, cuatro destinos de teletransporte y separación entre hechos históricos y defensa simulada.

## Capacidades y límites

- Comprueba si realmente hay Unity instalado, activado y con dispositivo gráfico. La nube de este proyecto solo tiene flujo .NET; no puede producir ni certificar renders Unity. No instales Unity o MCP como efecto secundario de una petición de decoración.
- Sin Unity, prepara código/configuración de Editor, inventarios y propuestas verificables; registra las capturas como **pendientes**. Nunca inventes imágenes, reutilices una captura antigua como nueva ni confundas checks estáticos con validación visual.
- Localmente, utiliza Unity Editor y opcionalmente un MCP revisado. Lee `Docs/VisualReview/UNITY_MCP.md` antes de proponer integración. No instales integraciones comunitarias sin revisar versión fijada, dependencias transitivas, descargas, permisos, endpoints y telemetría. No actives servicios remotos ni expongas el Editor por Internet.

## Procedimiento de diseño

1. Lee `git status`, rama y diferencias. Para cambios visuales importantes crea o reutiliza una rama `review/<tema>` desde la base acordada. No hagas reset, no pises cambios locales, no crees worktrees salvo petición expresa. No hagas push, merge, publicación ni despliegue automáticamente.
2. Antes de modificar/reemplazar una escena principal, conserva un estado recuperable: commit local de los cambios autorizados y limpios, o copia fechada de la escena y `.meta` fuera del checkout. Registra dónde y cómo restaurarlo. Si duplicas la escena dentro de Assets usa `AssetDatabase.CopyAsset` para obtener un GUID distinto; nunca dupliques su `.meta` con el mismo GUID. Conserva también materiales/prefabs que vayas a modificar. No elimines backups existentes ni Legacy_Prototype_Preserved.
3. Identifica los modelos disponibles en `Assets/Models/`, los prefabs/materiales en `Assets/_Project/OfficeVisual/` y sus licencias. Prioriza Kenney Furniture, escritorio Quaternius, Computer/Stapler de Poly by Google, impresora de Bruno Oliveira y papeles de Jarlan Perez. Usa FBX/OBJ importados antes que añadir importadores GLB o descargar duplicados. No uses Office Pack sin licencia verificable. Mantén atribuciones y `.meta`.
4. Formula criterios observables a partir de la petición: objetos, ubicación, escala, materiales, contraste, circulación, encuadres y texto que debe leerse. Modifica BancoCorresponsal con APIs del Editor, Undo y PrefabUtility o herramientas MCP revisadas. No edites YAML complejo de escena a ciegas. No ejecutes `Build`, `RefineLayout` ni `StartPlayValidation` sin revisar sus efectos: pueden modificar escena y assets.
5. Mantén jerarquías por zonas: arquitectura, entrada, inspección, evidencias, seguridad y mobiliario secundario. Usa instancias de prefabs compartidos, nombres claros, escala física coherente y colliders apropiados. No desempaquetes todos los prefabs para decorar. Las cámaras de revisión son temporales y no deben llegar a la APK ni alterar el rig XR.
6. Para Cardboard, conserva el perfil URP móvil, materiales compartidos y geometría low poly. Prefiere iluminación horneada y pocas luces; limita sombras en tiempo real, distancia y casters. Evita nuevos SSAO, bloom, volumétricos, reflejos o transparencias extensas sin medición en dispositivo. No aumentes globalmente calidad, MSAA, resolución de sombras o render scale para mejorar una captura. Registra antes/después con Profiler y Frame Debugger; un PNG no mide FPS, memoria, temperatura ni comodidad VR.
7. Mantén monitores, expedientes y botones World Space legibles desde el puesto de interacción, con contraste suficiente, tamaño de fuente y distancia cómodos, sin glare, clipping ni z-fighting. No tapes retícula, accesos ni teletransportes. Comprueba también el estado de Play: algunos textos/UI solo aparecen al ejecutar. No inventes datos de banco reales.

## Captura y corrección

1. Con Unity disponible, espera compilación/importación, abre únicamente BancoCorresponsal, sal de Play y guarda el estado aprobado para captura. Genera las cuatro vistas desde `SWIFT > Revision visual > Camaras y capturas PNG`. Ajusta los encuadres iniciales si ya no muestran los objetivos. La herramienta está en `Assets/_Project/Editor/SceneReviewCapture.cs` y escribe en `Docs/Previews/<sesion>/`.
2. Genera nuevas capturas después de cada modificación visual. Conserva las anteriores. Revisa `capture.json`, cuatro PNG y `README.md`; comprueba fecha, hashes y versión, y vincula la sesión al commit de escena. No reutilices `LATEST.md` si corresponde a otra revisión. No presentes capturas parciales como revisión completa.
3. Inspecciona las cuatro imágenes reales: oficina general, escritorio, computadora/expedientes y distribución superior. Compara con los criterios del usuario y la revisión anterior. Distingue vista cenital con techo oculto, render de Editor, UI de Play y VR real. Señala oclusión, clipping, material rosa, texto ilegible o encuadre incorrecto; corrige y vuelve a capturar. No afirmes haber comparado imágenes que no pudiste abrir.
4. Registra en `Docs/VisualReview/CHANGELOG.md`: petición, rama/base y estado recuperable, assets afectados, cambios, sesión de capturas, comprobaciones ejecutadas, limitaciones y problemas pendientes. No marques la revisión aprobada hasta recibir ese comentario del usuario.
5. Prepara un PR borrador solo si el usuario autorizó subir/publicar. Incluye las cuatro imágenes por rutas del repositorio en la rama y referencia la sesión concreta, instrucciones de celular y limitaciones. Mantén borrador hasta revisión; no hagas merge ni publiques automáticamente. Para subir usa selección explícita de escena/prefabs/materiales, `.meta`, documentos y capturas; excluye Library, Temp, secretos, builds y logs.
6. Convierte los comentarios del celular en criterios concretos, aplica sobre la misma rama, guarda otra sesión y responde con qué cambió, evidencias y pendientes. Si no hay Unity disponible, deja explícita la captura pendiente y los pasos locales; no simules progreso visual.
