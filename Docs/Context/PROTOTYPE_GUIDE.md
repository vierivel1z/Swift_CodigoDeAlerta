# Prototipo autónomo — guía de continuación

## Abrir y jugar
1. Esperar la resolución de paquetes y la compilación en Unity 6000.3.24f1.
2. Si no existe la escena, ejecutar **SWIFT > Crear escena BancoCorresponsal**.
   El generador conserva una escena existente y utiliza APIs de Unity, sin escribir YAML manual.
3. Abrir `Assets/_Project/Scenes/BancoCorresponsal.unity` y pulsar Play.
4. Botón derecho + mouse para mirar, WASD para desplazarse en PC; clic izquierdo selecciona
   el objeto bajo el cursor. Los portales están a izquierda/derecha del puesto.
5. Elegir Portal ATAQUE o Portal DEFENSA. Cambiar de portal reinicia el turno;
   se conservan los últimos resultados de cada modo durante esa ejecución.
6. Consultar archivador y alertas, registrar evidencia, decidir Aprobar/Retener/Devolver.
   La última decisión abre resultados automáticamente. No se puede cerrar un turno incompleto.
7. En defensa activar primero el panel de controles. Para comparar: aprobar todo en ataque
   deja USD 965,000 de pérdida ficticia; aprobar todo con controles en defensa deja USD 0.
   No son cifras históricas. La coincidencia de SIM-006 es una alerta legítima que requiere revisión.

## Componentes
- `Scripts/Banking/BankingDomain.cs`: TransactionData, TransactionManager, EvidenceManager,
  DefenseManager y GameSession; C# independiente de Unity y de los dispositivos de entrada.
- `Scripts/Core`: ScenarioData (ScriptableObject), GameManager (estado y eventos).
- `Scripts/Interaction`: acciones reutilizables, raycast, clic PC y mirada mantenida Android
  (1.5 s, una activación hasta retirar la mirada), adaptador mouse/WASD.
- `Scripts/VR`: pose de cabeza desde XR, ciclo de vida Cardboard y configuración del visor.
- `Scripts/UI`: Canvas World Space/TextMeshPro; botones con colliders para el mismo raycast.
  Se usa Liberation Sans SDF de los recursos esenciales TMP, incluidos en el proyecto.
- `Scripts/Audio`: tono de confirmación generado, sin archivos externos.
- `Scripts/Testing`: fábrica de oficina y sonda automatizada de Play Mode.
- `Editor`: generación de escenas, configuración Android/XR, validación y build APK.
- Prefabs, Models y Audio se reservan para hitos posteriores; no se descargaron modelos.

## XR y Android
Cardboard 1.36.0 está fijado al commit oficial
`7fb2c0e438d2ef686f51cd59b53da16cc07b1b68`. Su package.json declara Unity 6000.0.58f2;
6000.3.24f1 es posterior. Esto satisface el mínimo declarado, no certifica cada teléfono.
XR Management 4.5.4 declara compatibilidad con Unity 2022.3 o posterior.
Mantener Input System 1.20.0 y URP 17.3.0.

**SWIFT > Configurar Android Cardboard** usa las APIs del paquete resuelto para registrar
el loader `Google.XR.Cardboard.XRLoader` únicamente para Android. Conserva proveedores
existentes y el pipeline. Configura API mínimo 26, target 35, ARM64/IL2CPP, Activity,
GLES3, LandscapeLeft, Internet requerido y frame pacing desactivado.
Las plantillas Gradle parten de la instalación exacta del editor y añaden las dependencias
indicadas por la guía oficial. Verificarlas si se cambia la versión del plugin.
El identificador se reemplaza solo si todavía es el de plantilla.

`CardboardSmokeTest.unity` es una copia inicial de la oficina para pruebas VR.
El sample HelloCardboard sigue disponible en Package Manager > Google Cardboard > Samples;
no se ha declarado probado ni se ha importado como contenido del banco.
La APK usa exclusivamente BancoCorresponsal; SampleScene se conserva.
En el teléfono comprobar giroscopio, visor/QR, render estéreo, orientación y recenter;
las pruebas de Editor no certifican estos puntos.

Fuentes consultadas:
- https://developers.google.com/cardboard/develop/unity/quickstart
- https://github.com/googlevr/cardboard-xr-plugin/tree/v1.36.0
- https://docs.unity3d.com/Packages/com.unity.xr.management@4.5/manual/index.html
- Código y documentación XML de la instalación local 6000.3.24f1 y paquetes resueltos.

## Verificación reproducible
```powershell
dotnet run --project Tools/DomainTests/DomainTests.csproj
./Tools/Test-ProjectConfiguration.ps1 -EditorPath 'D:/Program Files/Unity/Installations/6000.3.24f1/Editor/Unity.exe'
```
Las pruebas de dominio no requieren NuGet externo. Las evidencias de la sesión están
 en Artifacts (ignorado por Git); consultar SESSION_REPORT.md para resultados finales.
No ejecutar otra instancia batch sobre un proyecto abierto. Se utilizó ValidationProject
(copia aislada de Assets/Packages/ProjectSettings) para validar sin cerrar el editor del usuario.
Los métodos batch son `Swift.EditorTools.PrototypeBuilder.ValidateBatch`, `StartPlaySmoke`
y `BuildAndroid`. StartPlaySmoke debe ejecutarse sin `-quit`, pues sale al terminar la sonda.

## Git
Repositorio local inicializado. El 2026-10-28 el usuario autorizó el commit inicial y publicación privada; consultar README.md y el resumen de publicación actual. Library, Temp, Logs,
UserSettings, Artifacts, ValidationProject y bin/obj están ignorados. Assets y .meta,
Packages, ProjectSettings, Docs y Tools se conservan. La excepción del .csproj de
DomainTests permite versionar las pruebas aunque los proyectos generados se ignoren.
Por diferencia de propietario entre sandbox y usuario fue necesario añadir solo la ruta
exacta del proyecto a `safe.directory` del Git del usuario; no se confió en rutas globales (*).

## Créditos y licencias
Equipo 05 — A Toda Olla. Geometría con primitivas y tono generado para este prototipo.
Código asistido por IA: OpenAI Codex. Fuente Liberation Sans: SIL Open Font License 1.1, incluida en Assets/TextMesh Pro/Fonts. Cardboard oficial: Google LLC, Apache-2.0; LICENSE.md
incluido en el paquete. Unity, URP, Input System, uGUI/TMP y XR Management mantienen sus
licencias de paquete (consultar LICENSE en Package Manager). Sin arte ni música de Papers, Please.

## Validación humana pendiente
- Legibilidad de documentos, tamaño de botones, colisiones visuales y comodidad en visor.
- Interacción por clic, dwell y ambos puntos de teletransporte en el dispositivo real.
- Audio, temperatura/FPS, pausa/reanudación, QR, permisos y recenter.
- Pestañas particulares del escenario 5 y criterios del profesor todavía no compartidos.
- GitHub, commits de cada integrante y entregables académicos.


Las dependencias Gradle también se contrastaron con sdk/build.gradle de Cardboard v1.36.0: https://github.com/googlevr/cardboard/blob/v1.36.0/sdk/build.gradle . Se incluye org.jspecify:jspecify:1.0.0 además de las cuatro de la guía. El objetivo de 60 FPS en Android es una solicitud al motor; no una medición de rendimiento.

Assets/_Project/Resources/ThirdPartyNotices.txt incluye las licencias completas de Cardboard, Liberation Sans y los paquetes Unity utilizados. Al estar en Resources, acompaña la APK.

APK de desarrollo generada: Artifacts/SWIFT-prototype.apk (66.5 MB). Firma v2 verificada; API 26/35, ARM64. No probada en teléfono. Ver SESSION_REPORT.md para hash y evidencias.

La oficina importada actual tiene cuatro teletransportes y se valida con BankOfficeBuilder.Validate/StartPlayValidation; consultar OFFICE_SCENE_REPORT.md. La APK previa corresponde al prototipo anterior.

