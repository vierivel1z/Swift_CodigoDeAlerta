# Demostración práctica ante el profesor

**Objetivo de la demo:** que se vea la secuencia **señal → elección humana → estado efectivo → consecuencia**. La versión principal usa HONOR/Google Cardboard y mirada durante 1,5 s. La ejecución en dispositivo con la versión actual está **pendiente**: ensayar y cronometrar antes de clase. No existe botón de salto de expedientes; hay que resolver los seis en cada turno. [Guion oral](SWIFT_GUION_ORAL.md) · [fundamento](SWIFT_EXPOSICION_CIBERSEGURIDAD.md).

## Antes de exponer

1. Generar una APK que incluya los cambios locales actuales; la APK Development documentada del 10/11 es anterior a cambios posteriores. Guardar log y hash. Instalarla en el HONOR y hacer el recorrido completo. No usar una captura del Editor como prueba de VR física.
2. Confirmar que portal, prólogo, asiento, monitor, CONTROLES, REGISTRO, ALERTAS, portapapeles, teléfono `SIM-003`, resultados y salida son legibles y alcanzables con la retícula. Verificar que una fijación de 1,5 s activa una vez y que apartar la mirada rearma.
3. Ensayar la misma secuencia en ambos modos: `SIM-001 Aprobar`, `SIM-002 Devolver`, `SIM-003 Aprobar` **deliberadamente para mostrar el control**, `SIM-004 Retener`, `SIM-005 Retener`, `SIM-006 Retener`. La tercera elección es un error didáctico, no una recomendación.
4. Mantener la aplicación abierta entre turnos; la comparación de pérdidas está en memoria. Reservar tiempo para seis decisiones por turno, prólogo, consulta y movimientos de mirada. El cierre ocurre tras la sexta decisión. Una pulsación prematura de CERRAR TURNO solo informa que faltan expedientes.
5. Tener una grabación de **la propia APK** como contingencia, etiquetada con versión/fecha y «video de ensayo». No afirmar que existe hasta grabarla y revisarla. Preparar capturas del Editor solo como apoyo, identificadas como tales.

## Recorrido completo: primero ATAQUE

| Paso | Acción real en el juego | Qué mostrar y decir |
| --- | --- | --- |
| A1 | En el portal, mirar **ATAQUE** 1,5 s; dejar terminar u omitir el prólogo. Entrar al puesto mirando el monitor principal. | «Soy el analista de un banco corresponsal ficticio. Este escenario no activa la corrección reforzada». |
| A2 | Abrir `SIM-001`; consultar expediente y REGISTRO; pulsar **APROBAR**. | «No hay anomalías precargadas; no queremos bloquear todo». |
| A3 | En `SIM-002` señalar banco intermediario ausente; pulsar **DEVOLVER**. | «Es un problema de formato; devolver no significa acusar de fraude». |
| A4 | En `SIM-003`, abrir **REGISTRO** y **ALERTAS**. Mostrar `Papell` frente a `Papel`. Opcional: portapapeles hoja 2 y 3. Pulsar **APROBAR** solo para la comparación. | «La señal estaba disponible, pero en ATAQUE una aprobación errónea queda aprobada». La llamada contextual no está disponible aquí. |
| A5 | `SIM-004` **RETENER**, `SIM-005` **RETENER**, `SIM-006` **RETENER**. | Señalar respectivamente cuenta no encontrada, monto inusual y coincidencia parcial. «Una coincidencia no confirma fraude». |
| A6 | Tras la sexta decisión, abrir **RESULTADOS**, **REVISAR DECISIONES Y FUENTES** y, si conviene, **CONTEXTO HISTÓRICO Y CRÉDITOS**. | Mostrar `SIM-003`: Elegiste Aprobar → Aprobada; 5/6 y USD 20 000 ficticios con esta secuencia. Separar el contexto histórico de estas cifras. |

## Segundo recorrido: DEFENSA con las mismas decisiones

| Paso | Acción real en el juego | Qué mostrar y decir |
| --- | --- | --- |
| D1 | Salir de la estación si corresponde, volver al portal y seleccionar **DEFENSA**. Entrar al puesto. | «El control comienza sin activar; entrar a DEFENSA no basta». |
| D2 | Activar **CONTROLES** antes de las decisiones relevantes. Confirmar el mensaje de controles activos. | «Esta es la nueva barrera. Solo sustituye un Aprobar cuando hay una anomalía precargada». |
| D3 | Repetir `SIM-001` **APROBAR** y `SIM-002` **DEVOLVER**. | Mantener iguales las elecciones para que la comparación sea justa. |
| D4 | En `SIM-003`, volver a consultar REGISTRO y ALERTAS. Abrir el **portapapeles** de madera: hoja 1, procedimiento; hoja 2, datos; hoja 3, observaciones y fuentes. Si se usa `MARCAR REVISIÓN`, leer el aviso de que no valida. | «Consultar, marcar y validar son acciones distintas». El portapapeles no aplica CONTROLES. |
| D5 | Abrir el **teléfono** disponible para `SIM-003`. Pulsar **SOLICITAR**, esperar `LLAMADA PENDIENTE`, pulsar **RESPONDER**, recorrer las líneas con **CONTINUAR/SIGUIENTE** hasta la advertencia final y pulsar **REGISTRAR**. | «Guion de prueba. La fuente ficticia comunica el nombre registrado, pero no confirma autorización; registrar deja evidencia y no bloquea». Si no hay botón habilitado, revisar que sea DEFENSA, `SIM-003` y que la llamada haya llegado al estado correcto. |
| D6 | Pulsar otra vez **APROBAR** en `SIM-003` deliberadamente. Después `SIM-004`, `SIM-005`, `SIM-006`: **RETENER** cada una. | «Repetí el error. La defensa activa cambia la aprobación de `SIM-003` a Retenida». |
| D7 | Abrir resultados y revisión posterior. | Señalar `SIM-003`: Elegiste Aprobar → Retenida → Control intervino. Comparar 5/6 en ambos, pérdida ATAQUE USD 20 000 frente a DEFENSA USD 0. La llamada no causó la sustitución. |

**No confundir el cierre:** no es necesario pulsar CERRAR TURNO después del sexto expediente; el turno se cierra automáticamente. Si se presenta otro conjunto de decisiones, no prometer las cifras del ejemplo.

## Recorrido reducido para una exposición corta

El juego **no permite ir directamente a `SIM-003` mediante un botón público**. Una demo en vivo desde cero sigue requiriendo `SIM-001` y `SIM-002` antes de ese caso, y completar los seis para ver resultados. La reducción honesta es **reducir el tiempo de explicación, no alterar la app**:

1. Mostrar en vivo el portal, `SIM-001` y `SIM-002` con una frase cada uno. En `SIM-003`, enseñar la discrepancia y la aprobación en ATAQUE. Completar `SIM-004`–`SIM-006` sin comentar cada pantalla y mostrar el resultado.
2. Si el tiempo medido no alcanza para repetir todo DEFENSA en vivo, presentar un **video previamente grabado desde la versión real de la app** con CONTROLES, teléfono, `SIM-003` y resultado final. Decir explícitamente «este fragmento es una grabación». Mantener una captura fija de la fila elección/estado para las preguntas.
3. Alternativa aún más breve si no hay video: usar capturas reales de Editor, **etiquetadas Editor**, para explicar la comparación mientras se demuestra en vivo solo una interacción de mirada y una consulta. Esto explica el diseño, pero no sustituye la prueba del resultado Android.

## Capturas ya disponibles como apoyo, no como prueba del HONOR

- [Resultado con elección y control](../Previews/CybersecurityReview/results-choice-and-control.png).
- [Portapapeles, hoja de observaciones](../Previews/CybersecurityReview/clipboard/clipboard-sources.png).
- [Teléfono, diálogo](../Previews/CybersecurityReview/phone/phone-dialogue.png) y [consulta registrada](../Previews/CybersecurityReview/phone/phone-recorded.png).

Estas imágenes se generaron en Unity Editor. Antes de exponer, comprobar que muestran exactamente la versión que se demostrará y no presentarlas como una grabación del móvil.
