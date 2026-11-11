# Guion oral — SWIFT: Código de Alerta

Uso sugerido: exposición de **10 minutos**, con demostración grabada o en vivo según el tiempo real medido. Las frases son una guía para hablar, no texto para leer literalmente. Practicar con cronómetro: no existe una duración física certificada del recorrido en HONOR. [Documento maestro](SWIFT_EXPOSICION_CIBERSEGURIDAD.md) · [pasos de demo](SWIFT_DEMO_PRACTICA.md).

## Versión para exponer

### 0:00–0:50 — El problema

«Nuestro juego pregunta algo simple: ¿qué pasa si una orden de transferencia trae señales de riesgo, el analista puede verlas, pero la aprueba de todas formas? El jugador no es un hacker. Es quien debe revisar la orden en un banco corresponsal ficticio. En ATAQUE veremos una situación vulnerable; en DEFENSA activaremos una barrera adicional y compararemos el resultado».

**Señalar:** portada, portal de modos y objetivo de aprendizaje. No prometer que la escena representa una oficina real de 2016.

### 0:50–1:50 — Contexto histórico sin mezclar cifras

«SWIFT permite que instituciones financieras intercambien mensajes estandarizados; no es el lugar donde el videojuego mueve dinero. Un banco corresponsal puede mantener cuentas y procesar pagos para otro banco, facilitando una transferencia internacional. En 2016, atacantes comprometieron sistemas de Bangladesh Bank desde los que salían instrucciones que parecían auténticas. El intento rondó los 951 millones de dólares; alrededor de 81 millones llegaron a Filipinas y una orden de 20 millones dirigida a Sri Lanka fue detenida. Nuestro juego toma la pregunta de seguridad de ese caso, pero sus seis expedientes y pérdidas son totalmente inventados». [SWIFT](https://www.swift.com/about-us/who-we-are/what-swift), [BIS](https://www.bis.org/committees/cpmi/payment), [Departamento de Justicia](https://www.justice.gov/d9/press-releases/attachments/2018/09/06/2018_09_06_park_complaint_unsealed_0.pdf).

### 1:50–2:30 — Qué revisa el jugador

«Cada turno tiene seis órdenes. Leo beneficiario, cuenta, monto e intermediario. Puedo abrir REGISTRO, ALERTAS y el portapapeles. Después elijo Aprobar, Retener o Devolver. Retener significa pedir revisión; Devolver responde aquí a un dato obligatorio ausente. Una alerta es un indicio, no una prueba de fraude».

**Señalar:** expediente y las tres decisiones. Si hay poco tiempo, usar captura de `SIM-003`.

### 2:30–3:50 — ATAQUE y el caso SIM-003

«Aquí tenemos `SIM-003`. En el expediente dice “Papell”; en el registro, “Papel”. Esa diferencia me permite sospechar y revisar. En ATAQUE tengo las alertas y el registro, pero no una corrección automática reforzada. Para demostrar la debilidad, elijo deliberadamente Aprobar. No estoy recomendando hacerlo: estoy provocando el error para observar la consecuencia. La orden queda Aprobada y el juego suma 20 000 dólares de pérdida ficticia para este caso».

**Señalar:** ambas grafías, botón Aprobar y luego el estado de resultado. Aclarar que el modo ATAQUE no desactiva todas las herramientas de consulta.

### 3:50–5:20 — DEFENSA, teléfono y procedimiento

«Repito las mismas seis elecciones, pero ahora activo CONTROLES. Ese botón enciende una regla del juego: si intento aprobar una orden con una anomalía, el sistema la retiene; si falta el intermediario, la devuelve. No usa inteligencia artificial ni sabe si hubo fraude real. Solo ve las anomalías precargadas. Antes de decidir `SIM-003`, también puedo usar el portapapeles para revisar tres hojas y un teléfono con un guion de prueba para consultar el nombre del registro. Ni marcar una hoja ni registrar la llamada activa el bloqueo. La llamada tampoco verifica un número real o la identidad de quien responde».

**Señalar:** estado CONTROLES activo, hojas 1–3, transcripción etiquetada `GUION DE PRUEBA`, botón REGISTRAR. No llamar a esto verificación autenticada.

### 5:20–6:40 — Intervención observable

«Vuelvo a elegir Aprobar en `SIM-003`. Esta vez el juego conserva mi elección, pero cambia el estado final a Retenida y marca que intervino el control. Mi decisión sigue siendo incorrecta. El control evitó la aprobación efectiva en la simulación; no me regaló el acierto».

**Señalar:** una fila que muestre «Elegiste APROBAR», «RETENIDA», «Control: intervino». Esta es la diferencia bancaria más importante entre los dos modos.

### 6:40–7:40 — Comparación y cifras

«Para comparar justamente repetimos las mismas seis decisiones: Aprobar, Devolver, Aprobar, Retener, Retener, Retener. El resultado comprobado es 5 de 6 elecciones coherentes en ambos turnos. ATAQUE acumula 20 000 dólares ficticios; DEFENSA con el control activo, cero. La llamada agregó información y evidencia, pero no alteró el cálculo. Si yo hubiera decidido bien desde el inicio, los dos modos podrían acabar sin pérdida. Por eso no decimos que DEFENSA siempre produce una cifra diferente».

### 7:40–8:40 — Qué se aprende

«El juego separa un control detectivo —mostrar una discrepancia—, una acción humana —decidir— y una barrera correctiva —cambiar el estado efectivo de una mala aprobación—. También muestra un posible falso positivo: `SIM-006` tiene coincidencia parcial de alerta, pero el juego no lo etiqueta como fraude para la pérdida. Retener es revisar, no acusar».

### 8:40–9:30 — Límites honestos

«Esto no se conecta a SWIFT ni reproduce la intrusión. El teléfono es ficticio y el control usa casos precargados. Las pruebas de código y del Editor pasaron; la versión actual necesita una APK nueva y ensayo con Cardboard en el HONOR. Los requisitos particulares del profesor que no se han entregado completos quedan pendientes de cotejo».

### 9:30–10:00 — Cierre

«La conclusión es que una alerta visible no basta si la decisión final la ignora. Una barrera adicional puede cambiar el resultado, pero también hay que medir la calidad de la decisión humana. Nuestro prototipo convierte esa diferencia en algo que el estudiante puede observar y explicar».

## Versión para estudiar: mapa de once ideas

1. **Problema:** una aprobación incorrecta puede convertir una señal de riesgo en una pérdida simulada.
2. **Historia:** Bangladesh Bank sufrió una intrusión en su entorno local; SWIFT declaró que su red central no fue comprometida. El juego no reproduce las órdenes reales. [Fuente](https://www.swift.com/news-events/press-releases/gottfried-leibbrandt-cyber-security-and-innovation).
3. **SWIFT:** mensajes entre instituciones; las cuentas y bancos gestionan el dinero.
4. **Corresponsal:** banco que presta servicios de pago/cuenta a otro banco en una cadena internacional. [BIS](https://www.bis.org/committees/cpmi/payment).
5. **Jugador:** analista en ambos modos, nunca atacante informático.
6. **ATAQUE:** expediente, registro y alertas disponibles; falta la corrección automática reforzada.
7. **DEFENSA:** CONTROLES comienza inactivo; al activarlo, un Aprobar anómalo se retiene o devuelve.
8. **Teléfono:** consulta ficticia solo `SIM-003` en DEFENSA; añade evidencia, no bloquea.
9. **Portapapeles:** procedimiento y marcas manuales; marcar no valida.
10. **Resultado:** elección original y estado efectivo pueden diferir; la puntuación valora la elección.
11. **Límite y valor:** no hay IA antifraude ni integración bancaria; sí una comparación observable de detectar, decidir e intervenir.

**Fórmula de memoria para `SIM-003`:** «Vi dos nombres → aprobé por error → en ATAQUE pasó → en DEFENSA activa se retuvo → mi puntuación no mejoró». Para el detalle de cifras y otras órdenes, consultar el [documento maestro](SWIFT_EXPOSICION_CIBERSEGURIDAD.md).
