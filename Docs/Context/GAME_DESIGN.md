# Diseño de juego — versión 0.1 (propuesta)

## Núcleo: revisar, contrastar, decidir
El jugador ve un expediente en un `Canvas` de Unity configurado en **World Space**, delante del monitor o flotante a una distancia cómoda. Usa la retícula para seleccionar botones grandes: `Aprobar`, `Retener`, `Devolver`. Debe consultar un registro y un panel de alertas en lugar de adivinar.

## Escenas sugeridas
- `Bootstrap`: inicialización, carga de datos.
- `MainMenu`: título, portal de entrada, selección de modo.
- `BankOffice`: sala principal, terminal, archivador, panel de alertas, consola de seguridad, salida.
- `Results`: resumen/decisiones/créditos (también puede ser panel en la misma sala).

**Primera iteración:** una sola escena `BankOffice` con cubos para sala y escritorio, un expediente y una interacción. No construir todas las escenas de inmediato.

## Cinco interacciones mínimas
| Objeto | Acción | Resultado observable |
| --- | --- | --- |
| Monitor | Inspeccionar expediente | Se abre ficha bancaria |
| Archivador | Consultar cuenta ficticia | Se muestra registro o advertencia |
| Panel de alertas | Revisar coincidencias | Aparece contexto de riesgo |
| Consola de defensa | Activar controles | Cambia estado/resultado del escenario |
| Terminal final | Finalizar turno | Se ejecuta `EndScenario` y muestra resultados |

## Expedientes de entrenamiento: 3 ejemplos inventados
1. Operación correcta de bajo monto: **Aprobar**.
2. Orden con banco intermediario ausente: **Devolver**.
3. Cuenta inventada no registrada o coincidencia que requiere revisión: **Retener**.

Estas órdenes son ficticias; no afirmar que ocurrieron individualmente en 2016.

## Lógica deseada
- Decisiones con consecuencias: aprobar algo sospechoso penaliza; retener legítimos sin causa también puede penalizar.
- Modo ataque: reproduce los hechos históricos en un tablero resumen, claramente identificados como hechos.
- Modo defensa: comparación contrafactual con controles activados; no presentar el resultado como histórico.
- Feedback inmediato accesible: sonido, color y texto, sin depender solo del color.

## Interacción VR y pruebas
- En el **Editor de Unity**: cámara de prueba con mouse + teclado y selección por clic o retícula simulada.
- En **Android VR**: seguimiento de cabeza + retícula; seleccionar mirando fijo (~1–2 segundos, ajustable).
- **Un solo servicio de interacción**; cambiar únicamente el proveedor de entrada.
- Teletransporte por mirada hacia puntos fijos, sin locomoción libre forzada.
- No intentar seguimiento de manos con Cardboard.

## Arte y escena
Estilo low-poly, una oficina compacta, mobiliario repetible, una pantalla legible y poca iluminación dinámica. Modelos sugeridos: escritorio, monitor/teclado, silla, archivador, panel de pared, reloj, impresora y mobiliario secundario.

## Cinemática opcional
Un prólogo breve con impresora apagada puede usarse como puente narrativo. Es en el BANCO ANDINO (origen) y no implica que el personaje de la impresora trabaje en el Banco Corresponsal. Priorizar gameplay antes de cinemáticas.

## Métricas para un MVP
- Una orden mostrada correctamente.
- Una interacción seleccionable con mouse y, luego, retícula Cardboard.
- Una decisión almacenada.
- Cambio visible entre modos ataque y defensa.
- Compilar e instalar APK de prueba tempranamente.
