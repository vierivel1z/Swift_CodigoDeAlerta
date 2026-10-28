# SWIFT: Código de Alerta — Contexto del proyecto

## Identidad
- **Equipo:** 05 — A Toda Olla.
- **NRC / grupo:** escenario 5, «Banco Corresponsal Internacional».
- **Proyecto académico general:** «Caso Banco Andino: La transferencia fantasma».
- **Nombre propuesto de la experiencia del equipo:** «SWIFT: Código de Alerta».
- **Tipo asignado:** **VR (realidad virtual)**, NO AR.
- **Motor:** Unity 6; versión vista en Unity Hub: **6000.3.24f1 LTS**.
- **Destino:** APK para teléfono Android real, con Google Cardboard XR Plugin.
- **Pipeline gráfico verificado (2026-10-27):** URP 17.3.0. PC_RPAsset en Graphics y calidad PC; Mobile_RPAsset para Android. Pendiente confirmar criterio docente; conservar URP. Ver INITIAL_AUDIT.md.

## Objetivo
Construir un videojuego educativo VR, situado en un banco corresponsal ficticio, en el que el jugador inspecciona órdenes bancarias fraudulentas, registra evidencias y aprende a diferenciar lo que sucedió en 2016 de una defensa hipotética que podría haber cambiado el resultado.

## Base narrativa obligatoria
El material docente se basa en el atraco al Banco de Bangladesh del 4–5 de febrero de 2016. En la narración se enviaron **36 órdenes por US$951 millones**. Treinta y cinco fueron inicialmente rechazadas por formato; la mayoría se reenviaron corregidas. Finalmente, **30 órdenes (~US$850 millones) quedaron bloqueadas**, **4 transferencias (US$81 millones) se pagaron a Manila**, **1 por US$20 millones llegó a una fundación en Sri Lanka y fue detenida/devuelta**, y **1 orden no se reenvió**. La combinación de filtros, revisiones y discrepancias debe explicarse sin presentar coincidencias o errores como pruebas automáticas de fraude.

En el capítulo «La impresora apagada» se cuenta que un empleado del banco de origen descubre una impresora SWIFT sin funcionar al día siguiente. **Ese empleado no pertenece al Banco Corresponsal Internacional**. Si se muestra, que sea un prólogo breve claramente separado del puesto del jugador.

La inspección manual propuesta por el juego es una **simulación contrafactual/educativa**, no la afirmación de que un analista real tuvo todas las oportunidades de detener las órdenes.

## Fantasía jugable (propuesta, no requisito textual del escenario)
El jugador es un analista ficticio del Banco Corresponsal Internacional. En una sala bancaria compacta, consulta órdenes sobre un monitor o un panel flotante legible y decide entre:
1. **Aprobar** (sin anomalía identificada en la simulación).
2. **Retener para revisar** (alertas, coincidencias, monto atípico, cuenta no verificada).
3. **Devolver por formato** (campos obligatorios incompletos).

Inspiración de interacción: inspección de documentos al estilo *Papers, Please*, **sin copiar arte, música, marcas ni recursos de ese juego**.

### Modos
- **Ataque 2016:** explicar y visualizar el resultado histórico. Puede incluir interacción guiada, pero el resumen histórico no debe cambiar falsamente.
- **Defensa simulada:** el jugador activa controles reforzados y las órdenes fraudulentas quedan retenidas; mostrar diferencia visual y narrativa frente al modo ataque.

### Flujo propuesto
Menú/portal → prólogo muy corto (opcional, impresora en el banco de origen) → sala de operaciones → tutorial de mirada → inspección de un conjunto pequeño de órdenes ficticias → resumen del modo ataque → activar defensa → segunda revisión con nuevos controles → resultado y reflexión final → créditos.

Un MVP puede usar **3 órdenes ficticias**, luego crecer a **6–10 órdenes representativas**. El total histórico de 36 debe mostrarse en el resumen; no asumir que el jugador debe inspeccionar literalmente las 36 si no lo exigen los requisitos particulares.

## Requisitos generales confirmados por el profesor
- Unity 6 y APK instalable en un Android real.
- Modo **ataque** y modo **defensa** en **la misma aplicación**, con resultados diferentes.
- Al ser VR Cardboard: **portal de entrada**, **teletransporte por mirada**, **mínimo 5 objetos interactivos mediante retícula**, **sonido** y **pantalla final**.
- Arquitectura común: `GameManager`, `ScenarioData` y métodos `RegisterEvidence`, `ApplyDefense`, `EndScenario`.
- GitHub con commits de todos los integrantes.
- Recursos de licencia libre/permitida y créditos dentro de la aplicación; declarar el uso de IA.
- Entregas: APK, enlace GitHub, video de 2–3 minutos desde celular mostrando ambos modos, ficha de máximo 4 páginas (escenario, vulnerabilidad, defensa, conceptos, capturas, roles y créditos).
- Presentación por equipo: 10 minutos (7 de demostración y 3 de análisis); hipótesis argumentada sobre el cómplice interno al final de la actividad.
- **Los PDF de Image Targets, Vuforia y requisitos AR no aplican a este equipo VR.**

## Objetos VR propuestos (al menos 5)
1. Computadora: abrir órdenes y tomar decisiones.
2. Archivador: consultar registro ficticio de beneficiarios/cuentas.
3. Panel de alertas: comparar alertas y coincidencias.
4. Consola de seguridad: activar controles del modo defensa.
5. Terminal de cierre: consultar resultados y terminar escenario.

Una impresora puede aparecer en el prólogo (banco de origen) o como decoración, pero no debe desplazar la actividad principal al escenario de otro equipo.

## Límites de alcance
- No implementar intrusión real, malware funcional, interceptación SWIFT ni acceso a redes/cuentas reales.
- No convertir el juego en «eliminar virus»: el análisis de malware y la respuesta a incidentes corresponden a otros escenarios.
- No simular detección real de manos: Cardboard funciona principalmente con mirada/retícula y sensores del teléfono.
- Desarrollar un modo de prueba de PC (mouse/teclado) separado de la entrada VR; compartir la lógica de juego.
- Evitar cámara que se mueva bruscamente, textos pequeños, sombras pesadas y escenas complejas que afecten Android VR.

## Reglas éticas
- Solo simulación defensiva.
- Bancos, personajes, cuentas, DNI, correos, documentos y beneficiarios ficticios.
- Nombres institucionales del juego: BANCO ANDINO, Banco Corresponsal Internacional y Banco Pacífico Insular; no utilizar marcas, logotipos o colores distintivos de entidades reales.
- No basar personajes visuales en docentes ni personas reales implicadas.
- Referirse a Lazarus con rigor, sin estereotipos nacionales.
- Registrar autor, licencia, URL de recursos y herramientas de IA usadas.

## Pendientes cruciales
**NO se han compartido las pestañas particulares `Funcional`, `Técnico`, `Roles`, `Defensa` y `Vulnerabilidad` completas del escenario 5.** Cuando se obtengan, actualizar este documento y priorizar las exigencias específicas del docente frente a las propuestas de diseño.


## Implementación autónoma — 2026-10-27
Ya existe un prototipo con seis órdenes ficticias, escena BancoCorresponsal, entrada
PC/mirada, UI TMP y modos ataque/defensa. Cardboard 1.36.0 fijado a commit oficial y
XR Management 4.5.4; loader Android configurado, mínimo API 26 y target 35, ARM64/IL2CPP.
Las pruebas Unity se ejecutaron en copia aislada. Esto no acredita funcionamiento en
un teléfono real. Consultar SESSION_REPORT.md, PROTOTYPE_GUIDE.md y el estado actual
al final del KANBAN; INITIAL_AUDIT describe el estado anterior a la implementación.
