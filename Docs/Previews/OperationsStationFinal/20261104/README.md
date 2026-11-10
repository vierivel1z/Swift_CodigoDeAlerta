# Estación VR final — capturas reales de Unity

Fecha: 2026-11-04 Lima. Unity 6000.3.24f1 / URP 17.3.0, Play Mode, Android como destino; no prueba en dispositivo.

| Vista | PNG |
|---|---|
| Puesto de pie, reposo activo y sin foco | [01](01-standing-desktop.png) |
| Monitor enfocado / entrada | [02](02-monitor-focused.png) |
| Entrada aproximadamente al 50 % real | [03](03-entry-half.png) |
| Sentado / EXPEDIENTE | [04](04-seated-expediente.png) |
| REGISTRO | [05](05-registro.png) |
| ALERTAS | [06](06-alertas.png) |
| PROCEDIMIENTO | [07](07-procedimiento.png) |
| Observaciones ampliadas | [08](08-observaciones.png) |
| SALIR DE ESTACIÓN enfocado | [09](09-exit-focused.png) |
| Puesto después de salir | [10](10-after-exit.png) |
| Consulta secundaria | [11](11-secondary-monitor.png) |
| Resultados | [12](12-results.png) |
| Consulta de cuenta ausente | [13](13-registro-cuenta-no-encontrada.png) |
| Luz del monitor desactivada | [14](14-monitor-spill-off.png) |
| Luz del monitor activada | [15](15-monitor-spill-on.png) |
| Exploración: pose y orientación reales, monitor sin foco | [16](16-standing-actual-exploration.png) |
| Foco del monitor: pose y orientación reales | [17](17-monitor-focus-actual-gaze.png) |

Los encuadres 01–15 y 04b usan una cámara temporal situada en el ojo real de la prueba y dirigida al monitor para comparar la UI. El jugador de prueba puede mirar a otro lado mientras esa cámara documenta el estado sin foco. Las capturas 16/17 conservan también la orientación real del jugador. No se simuló el porcentaje visual: 03 registra Image.fillAmount frente al dwell real. `receipt.txt` conserva pose, orientación de origen, modo, dwell, fecha UTC y contadores agregados del Editor. `hashes.json` y `validation.json` identifican los archivos finales; las capturas de iteraciones previas se regeneraron al finalizar y el recibo mantiene su historial.

Se revisaron visualmente los PNG y la referencia [REF_Operations_Expediente_Final.png](../../../../Assets/_Project/Art/References/REF_Operations_Expediente_Final.png): documento marfil, monto destacado, observaciones laterales, navegación y decisiones con la misma familia cromática. La geometría y cabecera son más simples; no se afirma coincidencia literal ni aprobación del usuario. No hay posproducción ni cambios de URP para las capturas.

[Informe de implementación, archivos, pruebas y HONOR](../../../Archive/2026-11-10/Context/OPERATIONS_STATION_FINAL.md).
