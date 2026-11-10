# Prueba física pendiente — HONOR / Google Cardboard

10/11/2026. Esta ficha describe pruebas por realizar; ninguna casilla constituye un PASS. Utilizar la APK indicada en `Artifacts/ImmersiveGameplay/apk-path.txt` cuando exista un build Succeeded. Las pruebas de Editor no certifican lectura binocular, temperatura ni seguimiento Android.

## Preparación y comprobaciones generales conservadas

Identificar el APK por ruta y SHA-256 del informe actual, no solo por la versión de paquete. Copiarlo al HONOR e instalar desde Archivos; si Android rechaza la instalación, registrar el mensaje exacto. Registrar modelo, Android/MagicOS, visor, perfil QR, fecha y participante.

Comprobar imagen horizontal estereoscópica, ambos ojos, orientación de cabeza y recentrado. Recorrer los ocho destinos existentes (Entry, Inspection, Evidence, Security, Cubicles West/East, Reception y Rear Gallery), sin aparecer dentro del mobiliario. Revisar aliasing de retícula/textos, reflejos y fade binocular, manteniendo los ajustes existentes.

Resultados de referencia de la simulación: aprobar los seis casos en ATAQUE da pérdida ficticia 965000; aprobar todos con controles en DEFENSA da pérdida 0, con 1/6 decisiones coherentes en ambas secuencias. La secuencia Aprobar/Devolver/Retener/Retener/Retener/Retener da 6/6 y pérdida 0. No son cifras históricas. Una alerta o coincidencia no confirma fraude.

El [checklist anterior](../Archive/2026-11-10/Context/HONOR_TEST_CHECKLIST.md) conserva el detalle histórico; su APK del 2/11 no es la versión vigente.

## Recorrido mínimo

1. Abrir desde el icono Android: debe cargar BancoCorresponsal y conservar el visor Cardboard. Recentrar mirando al frente antes de sentarse.
2. Practicar el tutorial, omitirlo, volver a abrirlo y completarlo. No debe crear una sesión ni cambiar resultados.
3. En el portal, mirar ATAQUE durante aproximadamente 0,75 s, apartar la mirada y cambiar a DEFENSA: cada progreso debe comenzar en cero, completar en 1,5 s y activar una sola vez. Sostener la mirada tras completar no debe activar otra vez.
4. Comprobar los tres momentos del prólogo, omisión y final automático; texto visible, panel portal detrás y sin acciones bancarias durante la introducción.
5. Entrar por el monitor. Debe haber fade corto, posición estable, seguimiento de cabeza activo y ambos monitores. Sentado no deben funcionar los teletransportes.
6. Mirar el portapapeles físico: el punto y el progreso deben verse. Su inspección aparece al lado izquierdo sin tapar el monitor; pasar páginas, leer nombres/cuentas largos, marcar/desmarcar y cerrar. Las marcas son manuales y no confirman validez ni puntúan.
7. Abrir el documento físico del escritorio: lectura solamente; cerrar sin decidir. Verificar que los cuatro botones de inspección se puedan seleccionar completos y no estén cubiertos por la mesa.
8. En DEFENSA llegar a SIM-003 (por ejemplo, Aprobar SIM-001 y Devolver SIM-002). Solo este expediente habilita el teléfono. Mirarlo, SOLICITAR, apartar y RESPONDER, leer tres intervenciones, REGISTRAR y cerrar. El texto GUION DE PRUEBA debe estar visible. La consulta no da una decisión ni activa defensas.
9. Mantener la mirada tras solicitar: no debe contestar sola. Mantenerla tras registrar o repetir el registro: una única evidencia con prefijo SIM-003. Volver a abrir conserva la consulta; iniciar otro turno la reinicia. Cambiar de expediente cancela cualquier consulta anterior.
10. Consultar Registro, Alertas y Procedimiento; decidir mediante Aprobar/Retener/Devolver. Al final comprobar la elección del jugador, el resultado efectivo y si intervino un control, además de fuentes y llamada registradas.
11. SALIR DE ESTACIÓN debe cerrar la inspección, mostrar PC en reposo, conservar el expediente y restaurar destinos permitidos. Volver a entrar mantiene el progreso.
12. Visitar impresora desde el destino de evidencias. Consultarla dos veces: una evidencia y una confirmación visual, sin duplicados. Registrar una llamada no debe encender la confirmación de impresora.
13. Pausar/reanudar con una inspección o llamada pendiente; perder foco, quitar visor y recentrar. La inspección debe cerrarse de forma segura y el timbre debe detenerse. No extrapolar la simulación de pausa del Editor a Android.

## Comodidad y rendimiento

Registrar modelo de visor, ajuste IPD/perfil QR y gafas del participante; sentado en una silla estable. Confirmar que puede leer subtítulos sin entrecerrar los ojos, que no hay doble imagen, que girar hacia herramientas no exige torsión dolorosa y que el punto no se pierde sobre papel claro.

Medir en dispositivo con Unity Profiler/Android: FPS y tiempos CPU/GPU, picos al abrir herramientas, memoria y calentamiento durante al menos 10 minutos. No fijar PASS por una captura ni por FPS del Editor. Si hay mareo, parar el ensayo y registrar el punto exacto. Anotar por separado resultados técnicos y comentarios de legibilidad.

## Herramientas de manos del salón

Pendiente identificar fabricante/modelo, visor, mandos o sensor y PC disponible. Cardboard en este HONOR no aporta por sí mismo seguimiento articulado de manos. La versión entregada conserva mirada; no simula manos rastreadas. Antes de instalar SDK, comprobar soporte oficial del hardware, compatibilidad con Unity 6000.3 y proveedor XR. Preparar una variante/plataforma separada y un adaptador de comandos a las mismas acciones; conservar el dominio y evitar doble entrada simultánea mirada/mano. No instalar un stack XR sobre la APK Cardboard para probar compatibilidad a ciegas.

## Registro

| Prueba | Resultado real | Dispositivo / fecha | Evidencia / problema |
|---|---|---|---|
| Arranque y Cardboard | NO PROBADO | | |
| Portal y prólogo | NO PROBADO | | |
| Herramientas / lectura binocular | NO PROBADO | | |
| Llamada / evidencia única | NO PROBADO | | |
| Entrada / salida / tracking | NO PROBADO | | |
| Pausa / recentrado | NO PROBADO | | |
| Rendimiento sostenido | NO PROBADO | | |
| Manos del salón | PENDIENTE DE HARDWARE | | |