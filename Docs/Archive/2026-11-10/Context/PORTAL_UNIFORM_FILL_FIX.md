> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Corrección de carga uniforme del portal

2026-11-03, Lima. Rama `review/portal-dwell-feedback`. Sin commit ni push.

## Fallo reproducido y causa

Se reprodujo en Unity con un ratón virtual del Input System y raycasts físicos reales. Antes de corregirlo, mantener el cursor sobre ATAQUE sin pulsar ya incrementaba `DwellProgress`, aunque la presentación ocultaba ese valor en modo PC. Además, el clic activaba inmediatamente la selección, sin esperar el dwell. La combinación permitía mostrar carga avanzada al volver a pulsar y ocultarla al comenzar el prólogo.

La prueba anterior a la corrección falló en `PC hover never accumulates invisible dwell`. Evidencia conservada: `Artifacts/PortalUniformFill/baseline-result.txt` y `baseline-checks.txt`.

## Comportamiento corregido

- **PC, tarjetas ATAQUE/DEFENSA:** mantener botón izquierdo durante 1,5 s. El hover no carga. Soltar antes de terminar o cambiar de tarjeta reinicia el contador. Volver a pulsar comienza desde cero.
- **Mirada/Cardboard:** se conserva la fijación de 1,5 s sin necesidad de pulsar. La simulación con G conserva la orientación de cámara existente.
- Retícula y barra leen el mismo contador de `VRInteractionManager`, también durante la pulsación en PC. No se añadió otro temporizador ni un suavizado que oculte el progreso real.
- Una activación al completar, seguida de la confirmación visual existente. Al iniciarse el prólogo se limpia el contador real, además de la barra, evitando progreso oculto durante ese estado.
- Perder el foco de la aplicación, pausarla o desactivar el interactor cancela la fijación.
- Los demás controles de PC conservan su selección por clic, incluidos clics sucesivos sobre un mismo botón bancario. No se cambiaron reglas bancarias, escenas, prefabs, URP ni Cardboard.

## Archivos

`Assets/_Project/Scripts/Interaction/VRInteractionManager.cs`: único contador, pulsación de las tarjetas en PC, cancelación y bloqueo durante el prólogo.

`Assets/_Project/Scripts/UI/ModeStationVisual.cs` y `GazeProgressVisual.cs`: mismo progreso real para ambas formas de entrada y texto «MANTÉN PULSADO» en PC.

`Assets/_Project/Editor/PortalUniformFillReview.cs` y su meta: regresión con estados de ratón enviados mediante Input System, cámara y colliders reales. `PortalOperationsReview.cs`: permite reutilizar su prueba de mirada sin sobrescribir capturas históricas.

Respaldo previo de los cuatro scripts existentes afectados y git status:
`D:\UnityGames\SWIFT_Backups\PortalUniformFill-20261103-210524`.

## Verificación final

| Prueba | Resultado |
|---|---|
| Hover sin pulsar | PASS: contador y barra a cero |
| Pulsación hasta aproximadamente 0,75 s | PASS: 49,204 %, exactamente igual en contador e Image |
| Incrementos durante carga | PASS: monotónicos, lineales respecto a deltaTime y visibles hasta completar |
| Soltar a mitad | PASS: imagen y contador vuelven a cero |
| Volver a pulsar | PASS: comienza desde cero |
| Cambiar ATAQUE → DEFENSA | PASS: sin transferencia |
| Completar 1,5 s y mantener pulsado | PASS: una activación y confirmación antes del prólogo |
| Prólogο / nuevo intento | PASS: sin carga oculta ni reanudación casi completa |
| Regresión de mirada de producción | PASS: ambas tarjetas, cancelación, latch, teletransportes y retorno |
| GameplayFlowProbe | PASS |
| BankOfficeProbe | PASS |
| PrototypeSmokeProbe | PASS |

Últimos resultados UTC: ratón `2026-11-04T02:12:20Z`, mirada `02:13:45Z`, flujo `02:14:08Z`, oficina `02:14:20Z`, smoke `02:14:33Z`. Corresponden a la noche del 3 de noviembre en Lima. Los logs de los intentos intermedios se conservan; el resultado final se encuentra en los archivos `*-result.txt`.

El prólogo puede interceptar el raycast con su propio collider, por lo que la prueba de regreso comprueba que un nuevo intento comienza con carga mínima, en lugar de suponer que el foco permaneció sobre la tarjeta durante el prólogo. La activación única con fijación continua se comprueba aparte.

Capturas nuevas y hashes: [UniformFix-20261103](../../../Previews/PortalOperationsReview/UniformFix-20261103/README.md). Se abrieron las cuatro imágenes reales para comprobar 0 %, mitad, cancelación y 100 %. La vista no modifica el diseño ni el FOV del portal.

**Límite:** verificado en Unity Editor; no ejecutado en HONOR/Android. No se generó una APK en esta corrección.
