# Diagnóstico inicial — 2026-10-27

Se leyeron los seis documentos de Docs/Context, manifest/lock, ajustes de Player,
Graphics, Quality, XR y escenas de compilación. No se encontraron AGENTS.md ni
skills locales en el proyecto. No se modificaron escenas, paquetes ni ProjectSettings.

## Estado real
- Unity 6000.3.24f1, revisión 4e7b9b5b6244. Hay procesos Unity activos; no se lanzó otra instancia.
- URP 17.3.0 declarado, resuelto y presente en caché. GraphicsSettings usa PC_RPAsset;
  QualitySettings usa Mobile_RPAsset para Android y PC_RPAsset para PC. GUID verificados.
- Input System 1.20.0 y uGUI 2.0.0 declarados, resueltos y presentes en caché.
- Cardboard y XR Plug-in Management ausentes de manifest/lock. Los módulos vr/xr
  incorporados no acreditan integración Cardboard. No hay loader configurado observado.
- Solo SampleScene habilitada, con cámara, luz y volumen global. No hay scripts de juego;
  los dos C# son Readme/ReadmeEditor de plantilla. No hay sala bancaria.
- AndroidPlayer, SDK, NDK y OpenJDK presentes bajo
  D:/Program Files/Unity/Installations/6000.3.24f1/Editor/Data/PlaybackEngines.
  Esto no confirma rutas activas ni funcionamiento de la compilación.
- Player: mínimo API 25, target automático (0), ARM64 (2), IL2CPP Android (1),
  Input System nuevo (1). Identificador Android aún de plantilla.
- Git disponible; git status informa que no existe repositorio. No hay .gitignore.
- Logs contienen diagnósticos de shaders URP y Curl error 3 en AssetImportWorker0.
  Contrastar con Console actual antes de tratarlos como errores vigentes. No se observó
  error CS en los logs consultados. No se ejecutó compilación C# ni APK.

## Compatibilidad y fuentes oficiales consultadas
[Quickstart Cardboard](https://developers.google.com/cardboard/develop/unity/quickstart)
requiere Unity 6000.0.23f1 o posterior; el editor local cumple ese mínimo. No certifica
la combinación exacta con URP ni el teléfono. La guía pide mínimo API 26 y target 35
 o posterior: revisar mínimo local 25 y SDK seleccionado por el target automático.
Revisar orientación, Activity, frame pacing y Gradle con la versión concreta del plugin.
No se instaló ningún paquete.

Otras referencias consultadas:
- [Repositorio oficial](https://github.com/googlevr/cardboard-xr-plugin).
- [Manual Android Unity 6.3](https://docs.unity3d.com/6000.3/Documentation/Manual/android.html).

## Seguimiento y arquitectura
K02 tiene inspección estática realizada y validación manual pendiente. Ninguna tarea
completa reúne evidencia suficiente para HECHO. K01 depende de las pestañas docentes;
K03 carece de repositorio; K04 de integración y APK; K05–K19 no están implementadas;
K20 tampoco tiene evidencia de entrega. No se inventaron requisitos particulares.
Conservar la propuesta de TECHNICAL_PLAN: empezar Assets/_Project/Scenes y Scripts
(Core, Data, Interaction) al implementar; crear otras carpetas cuando tengan contenido.
No generar todos los managers anticipadamente.

## Repetir comprobación
Desde PowerShell en la raíz:

```powershell
./Tools/Test-ProjectConfiguration.ps1 -EditorPath 'D:/Program Files/Unity/Installations/6000.3.24f1/Editor/Unity.exe'
```

Solo lee archivos. Sin excepciones confirma coherencia directa manifest/lock, no build.

## Validación manual de K02
1. Abrir con 6000.3.24f1, esperar importación y revisar Console; registrar errores actuales.
2. Comprobar Android disponible en Build Profiles y rutas SDK/NDK/JDK en External Tools.
3. Confirmar URP y perfiles Mobile/PC; adjuntar evidencia en KANBAN.
4. Mantener prueba Android/VR pendiente hasta instalar y ejecutar APK en teléfono real.

Siguiente: cerrar validación manual de K02. K03 puede avanzar independientemente con
.gitignore e inicialización local; acordar remoto/equipo antes de publicar. Después K04.
