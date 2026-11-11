# Estructura de diapositivas — exposición de ciberseguridad

Propuesta para **10 minutos**, pendiente de ensayo cronometrado. Mantener cada diapositiva con una idea, una imagen legible y pocas palabras; explicar oralmente los matices. Las capturas enlazadas son **de Unity Editor**, no de HONOR. Al preparar el archivo final, sustituirlas o complementarlas con evidencia del teléfono si se obtiene. No usar una imagen que sugiera que los seis casos ficticios pertenecen al incidente real.

| N.º y tiempo orientativo | Título y mensaje visible | Qué explicar oralmente | Evidencia sugerida |
| --- | --- | --- | --- |
| 1 · 0:00–0:35 | **SWIFT: Código de Alerta** — «Una señal no detiene sola una orden» | Objetivo educativo y rol del analista. | [Portal real del Editor](../Previews/SecondAutonomousPolish/AfterIteration4/01-portal.png). |
| 2 · 0:35–1:30 | **Contexto 2016** — «Entorno local comprometido; instrucciones fraudulentas» | SWIFT es mensajería; Bangladesh Bank fue víctima; intento de ~USD 951 M, ~USD 81 M a Filipinas, USD 20 M detenido en Sri Lanka. No equiparar con montos jugables. | Diagrama propio y simple: Bangladesh Bank → cuenta en Nueva York → destinos. Pie con [DOJ](https://www.justice.gov/d9/press-releases/attachments/2018/09/06/2018_09_06_park_complaint_unsealed_0.pdf) y [SWIFT](https://www.swift.com/news-events/press-releases/gottfried-leibbrandt-cyber-security-and-innovation). |
| 3 · 1:30–2:10 | **¿Qué hace un corresponsal?** — «Otro banco ayuda a ejecutar pagos transfronterizos» | Una cuenta/servicio de otro banco facilita la cadena; nuestro puesto es ficticio y simplificado. | Diagrama Banco emisor → Corresponsal → Banco receptor, basado en [BIS](https://www.bis.org/committees/cpmi/payment). |
| 4 · 2:10–3:00 | **El trabajo del analista** — «Leer → contrastar → decidir» | Expediente, REGISTRO, ALERTAS; Aprobar/Retener/Devolver. | [Expediente en el Editor](../Previews/DossierTreatment/20261104/04b-expediente.png), [registro](../Previews/DossierTreatment/20261104/05-registro.png) y [alertas](../Previews/DossierTreatment/20261104/06-alertas.png). |
| 5 · 3:00–3:55 | **ATAQUE = escenario vulnerable** — «La alerta existe; una aprobación errónea puede pasar» | `SIM-003`: Papell/Papel. El jugador es analista, no hacker; no hay corrección reforzada. | Zoom legible de `SIM-003` desde captura actual o recorrido real. Evitar capturas históricas que no muestren ese caso. |
| 6 · 3:55–4:55 | **DEFENSA = control activado** — «Aprobar anomalía → Retener/Devolver» | CONTROLES inicia inactivo; la regla usa anomalías precargadas, no IA ni etiqueta oculta de fraude. | Pantalla con CONTROLES activo y [resultado de elección frente a estado](../Previews/CybersecurityReview/results-choice-and-control.png). |
| 7 · 4:55–5:55 | **Consulta y procedimiento** — «Fuente abierta ≠ dato validado» | Portapapeles de tres hojas y teléfono de prueba en DEFENSA/`SIM-003`. La llamada no bloquea; no hay autenticación de interlocutor. | [Hoja de fuentes](../Previews/CybersecurityReview/clipboard/clipboard-sources.png), [diálogo telefónico](../Previews/CybersecurityReview/phone/phone-dialogue.png). |
| 8 · 5:55–7:10 | **Comparación justa** — «Mismas seis elecciones, distinto estado efectivo» | Secuencia `Aprobar, Devolver, Aprobar, Retener, Retener, Retener`; 5/6 en ambos; USD 20 000 ficticios en ATAQUE y USD 0 en DEFENSA activa. | Dos filas o captura de [resultados](../Previews/CybersecurityReview/results-choice-and-control.png), con rótulo **cifras ficticias**. |
| 9 · 7:10–8:20 | **Qué enseña cada caso** — «Formato, identidad, cuenta, monto y falso positivo» | `SIM-002` se devuelve por formato; `SIM-006` se retiene para revisar, sin llamarlo fraude. | Tabla propia de seis filas con solo señal y acción recomendada, derivada de [catálogo verificado](../Context/ATAQUE_DEFENSA_FUNCIONALIDADES.md). |
| 10 · 8:20–9:20 | **Alcance y siguiente prueba** — «Prototipo educativo, no banca conectada» | Código y Editor probados; APK actual, HONOR, comprensión con usuarios y guion telefónico pendientes. | Tres rótulos: Implementado / Simulado / Pendiente. Evitar iconos de IA, candados MFA o manos que sugieran funciones ausentes. |
| 11 · 9:20–10:00 | **Conclusión** — «Detectar, decidir e intervenir son etapas diferentes» | Resumir `SIM-003`: misma mala decisión, defensa activa cambia el estado, no la puntuación. Abrir preguntas. | Fórmula visual: **Señal → APROBAR → Aprobada / Retenida**; pie «resultado ficticio». |

## Diapositivas de reserva para preguntas

- **Regla exacta:** sin anomalía → Aprobar; sin intermediario → Devolver; otras señales → Retener. La intervención solo sustituye **Aprobar** cuando CONTROLES está activo en DEFENSA.
- **Seis expedientes:** [tabla detallada](SWIFT_EXPOSICION_CIBERSEGURIDAD.md#7-los-seis-expedientes-uno-por-uno).
- **Resultados:** elección original, estado efectivo, `DefenseOverride`, aciertos y pérdida ficticia. [Explicación](SWIFT_EXPOSICION_CIBERSEGURIDAD.md#8-cómo-se-calculan-puntuación-y-resultados).
- **Fuentes históricas:** [SWIFT](https://www.swift.com/about-us/who-we-are/what-swift), [BIS](https://www.bis.org/committees/cpmi/payment), [DOJ](https://www.justice.gov/usao-cdca/press-release/file/1367721/dl?inline=). El desglose de «36 órdenes» del material docente debe tratarse como resumen docente hasta localizar una fuente primaria adecuada para esa cifra exacta.

## Reglas de montaje

1. No colocar párrafos del documento maestro en pantalla. Mostrar palabras clave grandes y contar el ejemplo de `SIM-003` en voz alta.
2. Etiquetar **HISTÓRICO** y **SIMULACIÓN** cuando aparezcan cifras. USD 81 millones históricos y USD 20 000 ficticios nunca deben quedar juntos sin esa etiqueta.
3. Toda captura debe tener una leyenda «Unity Editor» o «HONOR/Cardboard» según su procedencia. No usar capturas del Editor como prueba de óptica binocular.
4. Si el recorrido completo tarda más que la exposición, usar el plan reducido del [documento de demo](SWIFT_DEMO_PRACTICA.md); un video grabado debe identificarse como grabación.
