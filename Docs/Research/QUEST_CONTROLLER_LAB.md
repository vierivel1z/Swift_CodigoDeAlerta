# Laboratorio aislado de dos mandos Quest

Plan técnico y estado real, 11/11/2026. **No se creó aún una escena Quest ni hay variante ejecutable o prueba de simulador**: una escena vacía sin proveedor, dos rayos y agarre comprobable aparentaría un prototipo que todavía no existe. El proyecto estable usa Cardboard, `VRInteractionManager`, una cámara con seguimiento de cabeza, selección por mirada de 1,5 s y colliders `BankInteractable`. `Packages/manifest.json` contiene Input System 1.20.0 y XR Management 4.5.4, pero **no** XR Interaction Toolkit, Unity OpenXR Plugin, Meta XR SDK ni un proveedor de manos. El modelo Quest del salón sigue sin especificarse. No se cambiaron paquetes, loaders, Build Profiles ni la escena principal. El gate P0 de uso físico en HONOR sigue pendiente; por el orden solicitado, no se instaló XRI en la entrega estable para adelantar P5.

## Elección de simulador

**Primer candidato para el laboratorio: Unity XR Interaction Simulator de XRI 3.3.2**, en un **proyecto/checkout aislado** creado a partir de una copia controlada. La [documentación oficial de XRI 3.3.2](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.3/manual/xr-interaction-simulator.html) muestra cabeza, mandos izquierdo/derecho, gatillo, grip, poses y el sample de acciones. Unity [lista versiones XRI 3.x para Unity 6](https://docs.unity3d.com/6000.0/Documentation/Manual/com.unity.xr.interaction.toolkit.html); esto **no certifica** aún la combinación exacta 6000.3.24f1 + XRI 3.3.2 + paquetes de este proyecto. Hay que instalarla y compilarla primero en el aislamiento. El simulador genera entradas; **no crea por sí mismo rayos, agarres ni una escena jugable**. Esos componentes deben añadirse al rig y al objeto.

**Alternativa posterior: Meta XR Simulator standalone**, si el visor y runtime elegidos son Quest/OpenXR. Según la [guía oficial de Meta actualizada en noviembre de 2026](https://developers.meta.com/vr/documentation/unity/unity-simulate-xrsim/), es un runtime OpenXR independiente; no requiere instalar el paquete antiguo `com.meta.xr.simulator` en Unity. Permite elegir fuente izquierda/derecha e inspeccionar asignaciones en `Input bindings`; sus teclas pueden cambiar. La versión actual no se ejecutó aquí y el proyecto Cardboard no está configurado como aplicación OpenXR compatible. El simulador tampoco equivale a probar el Android real. No activar Meta SDK ni cambiar proveedor global de la entrega principal para obtener una captura rápida.

| Criterio | Unity XRI Simulator | Meta XR Simulator standalone | Decisión actual |
| --- | --- | --- | --- |
| Dependencia | XRI + sample e Input Actions en laboratorio | Aplicación externa + proyecto OpenXR compatible | Preparar primero laboratorio XRI; comparar Meta tras conocer visor |
| Cabeza y mandos | Dispositivos simulados por Input System | Runtime OpenXR con perfil Quest | Ambos sirven para iterar, ninguno valida hardware |
| Rayos y agarre | Requieren `XRRayInteractor`, `XRGrabInteractable`, rig y bindings | Requieren interactores y lógica de app igualmente | No afirmar función hasta integrar y probar |
| Riesgo para Cardboard | Alto si se instala en proyecto principal sin aislamiento | Alto si se cambia runtime/loader principal | No tocar la entrega Cardboard |

### Controles documentados del sample XRI 3.3.2, **condicionales a importar ese sample**

Estos son los [controles publicados por Unity](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.3/manual/xr-interaction-simulator.html#sample-controls), **no teclas comprobadas en esta instalación**: `[` activa/cicla el dispositivo izquierdo; `]` el derecho; `Tab` alterna modo cabeza + mandos y modo de controlador/mano; `H` activa solo cabeza; `WASD` traslada el dispositivo activo; flechas lo rotan; ratón derecho + movimiento rota y rueda traslada; `T` conmuta gatillo, `G` grip; `Shift` dirige los atajos al izquierdo; `R` restablece la posición del dispositivo activo. El sample advierte que controles pulsados pueden mantenerse al cambiar de dispositivo: soltar explícitamente gatillo/grip y comprobar las acciones antes de atribuir un doble disparo al juego. Los rayos láser deben probarse en el rig, porque no son una tecla del simulador. En Meta XR Simulator, usar su panel `Input bindings` para leer la asignación de la versión instalada; no trasladar estas teclas de XRI a Meta.

## Arquitectura propuesta para un aislamiento real

1. Congelar inventario SHA de escena principal, manifest, ProjectSettings XR y prefabs. Crear checkout/proyecto de laboratorio fuera de la entrega Cardboard, con GUID de escena nuevo mediante `AssetDatabase.CopyAsset` **solo si** se necesita copiar la oficina; alternativa más ligera: escena `QuestControllerLab` vacía con un escritorio, portapapeles y teléfono referenciados desde prefabs existentes. No declarar que una copia por sí sola es un prototipo funcional.
2. Instalar XRI y OpenXR **solo en laboratorio** y elegir versión compatible en Package Manager de Unity 6000.3.24f1. Registrar versiones exactas y resultado de compilación. Mantener un único rig/cámara y un proveedor activo por perfil. Importar sample oficial de simulator y acciones; inspeccionar bindings efectivos, no confiar en nombres de tutoriales antiguos.
3. Agregar `XR Origin` con interactor de rayo izquierdo/derecho y selección por gatillo. Añadir `XR Direct Interactor` para agarre cercano. Si se conserva `BankInteractable` en el laboratorio, usar adaptador de interacción que llame `Activate()` **una vez por selección** y compruebe `CanActivate`, sin duplicar reglas ni crear otro dwell. Priorizar un sistema de selección por vez para que ambos rayos no emitan la misma decisión.
4. Usar un **proxy de portapapeles del laboratorio** con `Rigidbody` cinemático, collider propio y `XRGrabInteractable`. Reutilizar la misma fuente de `DeskSessionJournal`/`DeskInspectionView` para hojas y marcas; no duplicar el contenido bancario. Configurar retorno al soporte al soltar fuera de un área segura, sin perder página ni observaciones. No exigir agarre para terminar una ronda Cardboard.
5. Luego del portapapeles, evaluar el teléfono: grip solo sujeta; gatillo en objetivos separados Solicitar/Responder/Continuar/Registrar/Terminar; una acción por pulsación, registro único y cancelación al cambiar expediente. No equiparar agarrar con llamar.

La [guía oficial de XR Grab Interactable 3.3.2](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.3/manual/xr-grab-interactable.html) ofrece movimientos por velocidad, cinemático o Transform; para un portapapeles legible conviene empezar con cinemático, sin lanzamiento, una zona de retorno y un límite de distancia, y ajustar tras medir latencia y colisiones. Esta es una propuesta, no una configuración probada.

## Gates de verificación

| Gate | Prueba requerida | Estado |
| --- | --- | --- |
| L0 | Cardboard actual: mismos modos, dwell, decisión, evidencias, resultados y APK de referencia preservados | Editor PASS anterior; dispositivo HONOR pendiente |
| L1 | Laboratorio compila con versiones de XRI/OpenXR documentadas y sin cambios al manifest principal | NO PROBADO |
| L2 | Cabeza y dos rayos independientes; gatillo izquierda/derecha; grip; movimiento, rotación, liberación y recentrado | NO PROBADO |
| L3 | Portapapeles: agarre desde cada mando, acercar/rotar, cambiar hoja con el otro, soltar y volver al soporte sin perder datos | NO PROBADO |
| L4 | Una sola decisión y una sola evidencia por acción; mismos resultados del dominio .NET | NO PROBADO |
| L5 | Dispositivo Quest exacto: comodidad, lectura, tracking, rendimiento y regresión de dos mandos | NO PROBADO; hardware sin identificar |

**Reversión:** borrar o archivar solo el checkout/proyecto de laboratorio después de conservar sus recibos; el principal no necesita reset. Si el laboratorio comparte carpeta de assets, revertir sus cambios selectivamente contra inventario, nunca reemplazar la escena principal completa. No hacer merge automático. La variante Quest solo se considera entregable cuando L1–L5 pasan y el profesor puede seguir el mismo aprendizaje sin los mandos.
