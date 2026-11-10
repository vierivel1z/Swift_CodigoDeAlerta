> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Abrir la versión publicada

Rama preparada: `feature/vr-visual-improvements`, desde el commit de trabajo existente, sin merge con main. Consultar el hash publicado en GitHub o mediante `git rev-parse HEAD`; este documento no intenta incluir el hash del propio commit que lo contiene.

```sh
git clone --branch feature/vr-visual-improvements https://github.com/vierivel1z/Swift_CodigoDeAlerta.git
```

1. En Unity Hub, añadir la carpeta clonada y abrir con **Unity 6000.3.24f1**. Para compilar Android, instalar desde Hub los módulos Android Build Support/SDK/NDK/OpenJDK de esa versión.
2. Esperar la resolución de `Packages/manifest.json` y `packages-lock.json`. Se preservan URP **17.3.0** y Cardboard, incluida la revisión resuelta en lock. La resolución de paquetes necesita red y Git disponible; no se versiona Library/PackageCache.
3. Abrir `Assets/_Project/Scenes/BancoCorresponsal.unity`. Los modelos de Assets/Models, materiales, prefabs, LightingData y texturas de lightmap están versionados con sus `.meta`. No ejecutar regeneradores de oficina para abrirla ni volver a hornear por defecto.
4. Ejecutar Play: elegir un modo o abrir el tutorial opcional del monitor. Los documentos se presentan con TMP/UI en runtime; una vista de Edición muestra el monitor serializado anterior al montaje runtime. El cuadro y las referencias del prólogo sí están guardados en la escena.
5. La biblioteca `Assets/_Project/Resources/Audio/SWIFT_AudioCues.asset` usa tonos sintéticos TEMP para cues puntuales. No requiere sonidos externos para abrir el proyecto; música y ambiente están preparados y silenciosos hasta asignar clips finales.
6. BancoCorresponsal debe ser la primera escena habilitada de Build Settings. HelloCardboard y SampleScene permanecen disponibles como pruebas deshabilitadas.

La versión se compiló y ejecutó en el Editor local antes de publicar; no se probó una importación desde cero en otro equipo. Para Android, generar una APK nueva y seguir `HONOR_TEST_CHECKLIST.md`. Las APK previas y evidencias pesadas de sesión no forman parte del repositorio.

## Qué queda fuera

Library, Temp, Obj, Logs, UserSettings, Artifacts, APK/AAB, ZIP originales y copias de recuperación se conservan localmente o son regenerables; no se incluyen en este commit. Las primeras iteraciones de capturas se excluyen; se publican galerías finales seleccionadas.

La diferencia local de `ProjectSettings/ProjectSettings.asset` que solo limpia preloadedAssets tras compilación se mantiene sin versionar en este commit: no cambia API/arquitectura/URP/Cardboard del Player. XR Management incorpora los settings activos al preprocesar el build y los limpia después; su configuración y assets se conservan versionados. Se publica el cambio necesario de EditorBuildSettings para cargar el banco.

Los informes históricos pueden mencionar rutas de respaldo o logs locales excluidos. Las galerías seleccionadas y sus manifiestos sí están disponibles en el repositorio; ningún PASS histórico se atribuye a una prueba de dispositivo.

Los dos PNG nuevos de `Assets/_Project/Art/References/` quedan locales: aparecieron durante el cierre, no tienen todavía `.meta` ni referencias verificadas en la escena. Se preservan para revisión posterior, sin incluirlos como integración terminada. También quedan fuera el plan local de instalación MCP, herramientas de extracción no necesarias y galerías históricas no seleccionadas.
