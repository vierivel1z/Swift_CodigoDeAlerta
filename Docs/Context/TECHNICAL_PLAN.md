# Arquitectura técnica y convenciones

## Estructura recomendada dentro de `Assets/`
```
Assets/
  _Project/
    Scenes/
    Scripts/
      Core/
      Data/
      Interaction/
      Gameplay/
      UI/
    Prefabs/
    Materials/
    Models/
    Audio/
    UI/
    ResourcesCredits/
```
No mover ni modificar archivos internos del plugin Cardboard. Mantener assets de terceros en carpetas separadas si es práctico.

## Clases y responsabilidades
- `GameManager`: control de estado `Menu`, `Attack`, `Defense`, `Results`; almacena evidencias y termina escenarios.
- `ScenarioData` (`ScriptableObject` o serializable): configuración del escenario, modos, órdenes ficticias, datos y resultados históricos. Elegir una opción y documentarla.
- `TransactionData`: datos inventados de cada orden y anomalías visibles.
- `TransactionManager`: entrega y evalúa órdenes en la simulación.
- `VRGazeInteractor`: raycast desde cámara y temporizador de enfoque sobre objetos interactivos.
- `EditorInputAdapter`: modo de test para mouse/teclado, desacoplado de Cardboard.
- `GazeTeleport`: teletransporte a puntos fijos.
- `TransactionPanelUI`: visualización de expedientes en `World Space`.
- `ResultsPanelUI`: resumen comparativo.
- `AudioManager`: pistas y eventos sonoros ligeros.

### API de arquitectura obligatoria
Mantener **exactamente** estos nombres (firmas a acordar según la práctica, sin inventar compatibilidad):
- `RegisterEvidence`
- `ApplyDefense`
- `EndScenario`

## Decisiones técnicas tempranas
1. Inspeccionar el proyecto existente y su render pipeline; **no migrar Built-in↔URP automáticamente**.
2. Verificar Android Build Support (SDK, NDK y OpenJDK) en Unity Hub.
3. Seguir la guía oficial del plugin Cardboard: https://developers.google.com/cardboard/develop/unity/quickstart
4. Probar primero el ejemplo `HelloCardboard` en Android.
5. Usar `Canvas` en `World Space` y botones suficientemente grandes; revisar legibilidad en visor real.
6. Probar FPS/temperatura en el teléfono destino, sin suponer una cifra de rendimiento sin medición.
7. Mantener escenas y scripts propios versionados; incluir `.gitignore` de Unity, archivos `.meta`, `Packages/manifest.json` y `ProjectSettings/`; ignorar `Library/`, `Temp/`, `Obj/`, `Build/`.

## Reglas para el agente del IDE
- Antes de editar, leer `PROJECT_CONTEXT.md`, `GAME_DESIGN.md`, `TECHNICAL_PLAN.md` y `KANBAN.md`.
- No cambiar requisitos del docente, ni renombrar las APIs obligatorias.
- No sobrescribir escenas, prefabs ni configuración de paquetes sin explicar riesgos.
- Hacer cambios pequeños, comprobables y con nombres de archivos explícitos.
- No inventar archivos que ya existan; inspeccionar el árbol del proyecto.
- No afirmar que algo fue probado en Android si no hay evidencia de una prueba real.
- Registrar avances, pruebas y bloqueos en `KANBAN.md`.
- Comentar qué operaciones requieren cambios manuales en el Editor.
- No programar malware real, ni solicitar secretos o datos bancarios auténticos.
