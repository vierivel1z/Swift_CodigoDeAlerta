# Glosario explicado con ejemplos del juego

Las definiciones bancarias describen conceptos generales; los ejemplos de `SIM-001`–`SIM-006` son ficticios. Para el desarrollo completo, ver [SWIFT_EXPOSICION_CIBERSEGURIDAD.md](SWIFT_EXPOSICION_CIBERSEGURIDAD.md).

## SWIFT

**Definición fácil:** red de mensajería estandarizada entre instituciones financieras, utilizada para comunicar instrucciones y otra información; no es por sí misma el depósito del dinero. **En el juego:** el título sitúa la simulación en el mundo de pagos internacionales, pero la app no envía mensajes a la red. **Importa porque** un mensaje aparentemente auténtico no significa que el entorno que lo originó estuviera seguro. [SWIFT](https://www.swift.com/about-us/who-we-are/what-swift).

## Banco corresponsal

**Definición fácil:** banco que mantiene cuentas o presta servicios de pago para otro banco, a menudo en otra jurisdicción. **En el juego:** el analista trabaja en un corresponsal ficticio y examina órdenes. **Importa porque** una transferencia internacional puede implicar varias instituciones con responsabilidades diferentes; nuestra estación simplifica esa cadena. [BIS](https://www.bis.org/committees/cpmi/payment).

## Orden de transferencia

**Definición fácil:** instrucción para mover dinero bajo ciertas condiciones y hacia una cuenta. **En el juego:** `SIM-003` pide enviar USD 20 000 ficticios a un beneficiario indicado. **Importa porque** una orden con campos inconsistentes debe revisarse antes de aprobarla.

## Beneficiario

**Definición fácil:** persona u organización a la que se dirige el pago. **En el juego:** `SIM-003` nombra a «Fundacion Horizonte de Papell», pero el registro muestra «Papel». **Importa porque** una diferencia de identidad es una señal para contrastar, aunque no pruebe fraude.

## Banco intermediario

**Definición fácil:** institución que puede participar en la cadena para hacer llegar un pago entre bancos. **En el juego:** falta en `SIM-002`. **Importa porque** la regla educativa devuelve esa orden por formato; la ausencia de campo no equivale automáticamente a un delito. [BIS, cadenas de pagos](https://www.bis.org/committees/cpmi/payment).

## Vulnerabilidad

**Definición fácil:** debilidad que puede permitir un resultado no deseado. **En el juego:** en ATAQUE, una aprobación manual errónea con una anomalía visible no recibe corrección reforzada. **Importa porque** define la lección que se compara, sin fingir que la app recrea una intrusión técnica.

## Control preventivo

**Definición fácil:** medida que intenta impedir una acción no deseada antes de que produzca su efecto. **En el juego:** la revisión reforzada activa impide que ciertos Aprobar anómalos terminen Aprobados. **Importa porque** reduce la pérdida ficticia de ese error; no significa que el juego implemente MFA o doble firma.

## Control detectivo

**Definición fácil:** medida que señala un dato problemático para revisarlo. **En el juego:** REGISTRO y ALERTAS muestran la discordancia de `SIM-003`. **Importa porque** detectar no es resolver: el jugador todavía puede aprobar equivocadamente.

## Verificación independiente

**Definición fácil:** comprobar una instrucción usando una fuente o canal confiable que no dependa de la propia instrucción sospechosa. **En el juego:** el teléfono *representa la idea* de una segunda consulta, pero no comprueba número ni identidad. **Importa porque** llamar a un número recibido en un mensaje potencialmente comprometido no garantiza independencia.

## Evidencia

**Definición fácil:** constancia de lo que se observó o hizo; su calidad depende de la fuente y del proceso. **En el juego:** EVIDENCIA registra una observación y la llamada puede dejar una entrada asociada a `SIM-003`. **Importa porque** ayuda a explicar el camino de una decisión, pero no es prueba forense o legal.

## Trazabilidad

**Definición fácil:** posibilidad de reconstruir qué fuente se consultó, qué se eligió y qué ocurrió. **En el juego:** el diario del turno distingue fuentes abiertas, marcas, llamada, elección y estado efectivo. **Importa porque** permite revisar un error humano y una intervención automática. El diario no persiste entre ejecuciones ni constituye una auditoría bancaria formal.

## Anomalía

**Definición fácil:** dato que difiere de lo esperado y merece atención. **En el juego:** intermediario faltante, nombre distinto, cuenta no hallada, monto inusual o coincidencia parcial. **Importa porque** activa revisión; el flag precargado no certifica fraude.

## Fraude

**Definición fácil:** engaño intencional para obtener un beneficio ilegítimo. **En el juego:** una etiqueta interna marca tres órdenes para *calcular* pérdida ficticia si se aprueban. **Importa porque** esa etiqueta de autor no debe mostrarse como prueba al analista ni usarse para que el control «adivine» la respuesta.

## Retención

**Definición fácil:** dejar una orden pendiente de mayor revisión, sin ejecutarla como aprobada. **En el juego:** `SIM-003` debería retenerse por discrepancia de nombre. **Importa porque** permite investigar una señal sin declarar automáticamente fraude.

## Devolución

**Definición fácil:** retornar una orden por un dato o requisito faltante en el ejercicio. **En el juego:** `SIM-002` se devuelve porque no incluye banco intermediario. **Importa porque** distingue un defecto de formato de una sospecha que exige investigación.

## Defensa en profundidad

**Definición fácil:** usar varias capas que cumplan funciones distintas, de manera que el fallo de una no determine por sí solo el resultado. **En el juego:** expediente/alerta informan, el analista decide y CONTROLES puede corregir una aprobación anómala. **Importa porque** el ejemplo `SIM-003` muestra qué capa intervino. La simulación tiene pocas capas y no representa la arquitectura completa de un banco.

## Falso positivo

**Definición fácil:** señal que sugiere un problema aunque el caso no sea el fraude que se sospechaba. **En el juego:** `SIM-006` tiene coincidencia parcial, pero no etiqueta interna de pérdida por fraude. **Importa porque** retener para revisar no debe confundirse con condenar una operación.

## `DefenseOverride`

**Definición fácil:** marca interna de que el control activo sustituyó la aprobación elegida por el analista. **En el juego:** `SIM-003` puede mostrar «Elegiste APROBAR», «RETENIDA» y «Control intervino». **Importa porque** permite distinguir el error de la persona del resultado protegido; la puntuación continúa evaluando la elección original.

## Decisión original y estado efectivo

**Definición fácil:** la decisión original es el botón que pulsó la persona; el estado efectivo es lo que finalmente quedó en el expediente. **En el juego:** con defensa activa, Aprobar `SIM-003` puede terminar Retenida. **Importa porque** una pérdida cero no demuestra que el analista eligiera bien.

## Pérdida ficticia

**Definición fácil:** suma didáctica de montos de casos etiquetados internamente para el ejercicio que terminaron Aprobados. **En el juego:** aprobar por error `SIM-003` en ATAQUE añade USD 20 000 ficticios. **Importa porque** permite comparar consecuencias sin atribuir esos montos a víctimas reales.
