# Demostración académica: qué falló, qué cambió y por qué

Guion verificable para `BancoCorresponsal`, 11/11/2026. Versión principal: HONOR/Google Cardboard con mirada de 1,5 s. **No hay ensayo físico ni duración medida**; hacer una práctica cronometrada en el salón antes de presentar. No mostrar la variante Quest como funcional. Las seis órdenes y todos los importes de juego son ficticios; el contexto histórico fijo se identifica por separado.

## Mensaje de una frase

«En ATAQUE el analista dispone de datos y alertas, pero no de la corrección automática reforzada; en DEFENSA puede activarla. Cuando aprueba una orden con anomalía, el sistema cambia el estado efectivo. La llamada añade información y evidencia, pero no es la causa de ese cambio automático».

La llamada es una **simulación de guion**, no una llamada real ni una comprobación técnica de un número independiente; el destino y la identidad del interlocutor no se verifican en el prototipo.

## Preparación

- Usar la misma escena y el mismo conjunto de seis expedientes en ambos modos; iniciar un turno limpio para cada uno. Confirmar que se ve el portal, el asiento, el expediente, REGISTRO, ALERTAS y los botones.
- Probar antes de clase que el visor enfoca durante 1,5 s, la cámara no oculta el prólogo, la estación se abre, los textos se leen en ambos ojos, el teléfono SIM-003 está disponible en DEFENSA, la salida funciona y los resultados se pueden comparar. [Checklist del HONOR](../Context/IMMERSIVE_DEVICE_CHECKLIST.md).
- Mantener las mismas seis **elecciones del analista** en los dos turnos para aislar el efecto de la revisión reforzada. Explicar que una aprobación incorrecta en SIM-003 es deliberada para mostrar el control, no una recomendación bancaria.
- El segundo turno conserva `LastAttackLoss` mientras la app siga abierta; no cerrar/reiniciar la aplicación entre ATAQUE y DEFENSA. No se ha implementado persistencia entre arranques.

## Recorrido sugerido

| Momento | Acción en la versión real | Qué decir / qué observar | Evidencia visible |
| --- | --- | --- | --- |
| 1. ATAQUE | Seleccionar ATAQUE, pasar/omitir prólogo y entrar a la estación | «Somos analistas, no el atacante. La deficiencia simulada es depender de la decisión manual sin la corrección reforzada» | Portal, modo en cabecera, ausencia de activación de CONTROLES |
| 2. Orden comparable | Avanzar hasta SIM-003. En SIM-001 elegir **Aprobar**; en SIM-002 **Devolver** | «El defecto de formato se devuelve; no lo llamaré fraude» | Estado y observación de SIM-002 |
| 3. Riesgo | En SIM-003 abrir REGISTRO y ALERTAS, comparar `Fundacion Horizonte de Papell` con `Fundacion Horizonte de Papel`. Elegir deliberadamente **Aprobar** | «Había una discrepancia visible; la decisión manual deja aprobada esta orden ficticia» | Consulta de registro, alerta y estado final de SIM-003 |
| 4. Terminar ATAQUE | SIM-004 **Retener**, SIM-005 **Retener**, SIM-006 **Retener** | «Son decisiones de demostración; una coincidencia parcial no prueba fraude» | Resultados ATAQUE: 5/6 decisiones coherentes y USD 20 000 de pérdida ficticia, si datos/orden siguen como hoy |
| 5. DEFENSA | Seleccionar DEFENSA; activar CONTROLES antes de decidir SIM-003. Repetir SIM-001 **Aprobar**, SIM-002 **Devolver** | «La protección exige activación; no está encendida solo por entrar al modo» | Mensaje de controles activos |
| 6. Procedimiento | En SIM-003 abrir portapapeles y pasar sus tres hojas; consultar REGISTRO/ALERTAS; opcionalmente marcar manualmente la revisión | «Abrir una fuente, marcar una hoja y validar un dato son hechos distintos» | Hoja de datos, fuentes y nota «No valida datos» |
| 7. Teléfono de prueba | En SIM-003 abrir teléfono, SOLICITAR → RESPONDER → leer transcripción → REGISTRAR | «La fuente ficticia comunica el nombre registrado. No confirma autorización ni fraude; guardamos exactamente lo consultado» | `GUION DE PRUEBA`, estado `CONSULTA REGISTRADA`, evidencia única |
| 8. Comparación causal | Elegir de nuevo **Aprobar** en SIM-003; SIM-004/005/006 **Retener** | «El analista volvió a equivocarse en SIM-003. El control activo, no la llamada, cambió Aprobada por Retenida» | Resultado DEFENSA: elección original Aprobar, estado efectivo Retenida, `Control: intervino` |
| 9. Cierre | Abrir revisión de decisiones/fuentes y comparar ATAQUE/DEFENSA | «Mismas elecciones: 5/6 en ambos; pérdida ficticia 20 000 frente a 0. La diferencia de ese caso se debe a la intervención simulada» | Dos importes, fila SIM-003, evidencia de llamada y explicación posterior |

Los importes de este **caso de prueba** están comprobados con una prueba nueva en `Tools/DomainTests/Program.cs`: mismas seis elecciones, 5/6 aciertos en ambos modos, USD 20 000 en ATAQUE y USD 0 en DEFENSA activa. Si se elige otra secuencia, no prometer estos números. La llamada no modifica `Defense.Active`, `Correct`, `DefenseOverride` ni `SimulatedLoss`; su aporte es información y registro. La defensa no identifica si hubo fraude real: actúa sobre flags de anomalía. El caso SIM-006 carece de etiqueta interna de pérdida y, aun así, una aprobación con defensa activa sería retenida; usarlo para explicar falsos positivos y necesidad de contexto.

## Preguntas que debe poder contestar el profesor

1. **¿Qué falló en ATAQUE?** El analista aprobó una orden con nombre discrepante; no había corrección automática reforzada que interceptara esa aprobación.
2. **¿Qué existía en ambos modos?** Expediente, registro, alertas, decisiones y registro de evidencias. No afirmar que ATAQUE ocultaba todos los controles.
3. **¿Qué añadió DEFENSA?** Un control activable que cambia el estado efectivo de una aprobación con anomalía. El teléfono contextual SIM-003 añade una fuente de contraste, pero no gobierna ese control.
4. **¿Qué hizo el analista y qué hizo el sistema?** El analista consultó, marcó, registró y eligió Aprobar; el sistema, con el control activo, retuvo. La puntuación sigue evaluando la elección Aprobar.
5. **¿La discrepancia prueba fraude?** No. Es información para revisar; la etiqueta de fraude simulado solo está en los datos de autor para calcular la pérdida del ejercicio.
6. **¿Es una reproducción histórica?** No. Los seis expedientes y pérdidas son inventados. La pantalla de contexto histórico es un resumen fijo del material docente.

## Evidencia y pruebas para ensayar antes de clase

| Nivel | Lo que existe | Lo que falta |
| --- | --- | --- |
| .NET | PASS actual: DomainTests 37/37 y DeskToolsTests 37/37; log en `Artifacts/CybersecurityReview/` | Repetir después de cualquier edición bancaria |
| Unity Editor | PASS actual: regression, flow, office, smoke, phone y clipboard; capturas nuevas en `Docs/Previews/CybersecurityReview` | Validar óptica del visor y repetir si cambia texto/layout |
| Simulador XR | Ninguna ejecución de dos mandos | Laboratorio aislado, compilación y pruebas L1–L4 |
| HONOR/Cardboard | APK de desarrollo anterior con hash en `IMMERSIVE_AUTONOMOUS_PROGRESS.md`; no equivale a uso | Instalar, hacer recorrido real, registrar lectura, FPS/calor, pausa y duración |
| Quest físico | Modelo y disponibilidad por confirmar | Probar solo después del laboratorio; nunca usar captura PC como certificado físico |

## Plan de reversión y ajustes viables

El recorrido no requiere modificar datos ni mecánicas. Si el tiempo en clase es excesivo, preparar dos demostraciones grabadas **desde la app real** y mostrar en vivo solo SIM-003 y la comparación; etiquetar claramente qué es grabación y qué es ejecución en directo. No saltar expedientes mediante código oculto ni falsificar importes. Si el guion telefónico no recibe aprobación editorial, mantener `GUION DE PRUEBA` y explicitarlo o prescindir de la llamada sin cambiar la regla de defensa. Si una mejora visual dificulta lectura, restaurar solo su archivo desde el respaldo, conservando los demás cambios locales.
