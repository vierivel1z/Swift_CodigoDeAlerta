> **Histórico archivado el 10/11/2026.** Conserva el estado de su sesión; no describe necesariamente la versión actual. Consulta el [índice vigente](../../../README.md).

# Ampliación arquitectónica real — BancoCorresponsal

Fecha de trabajo: 2026-11-03, America/Lima. Rama `review/environment-immersive`, base Git `1074fdd`. Los cambios locales anteriores se conservan. Sin commit, merge ni push.

## Referencias y criterios

Confirmadas por el usuario: `Assets/_Project/Art/References/DiseñoVisual_Completo.png` y `DiseñoVisual_PC.png`. No se renombran, duplican ni usan como pantallas funcionales. Se toma de la primera la profundidad por planos, grupos de cubículos, consola sólida baja e inclinada y fondo institucional. De la segunda, monitor principal horizontal, consulta a la derecha, libreta y separación de madera/tela/plástico/metal/papel. Textos y decisiones permanecen en TMP/UI.

La ampliación propuesta es 20 × 18 m; las dimensiones anteriores y finales se registrarán desde los bounds reales del suelo en Unity. No se escala toda la escena ni se cambia el FOV del jugador.

## Protección y trabajo por etapas

Respaldo completo de Assets, Packages, ProjectSettings y Docs antes de esta intervención: consultar `Artifacts/ExpandedBankOffice/backup-path.txt`. Checkpoints posteriores conservan escena, `.meta` y lightmaps. Restauración selectiva desde copia externa, sin reset/clean y preservando cualquier trabajo posterior.

1. Pruebas del estado anterior, medición y planta antes.
2. Ampliar suelo, paredes, techo y vano de entrada; columnas, ventanas simplificadas y supervisión con profundidad. Reorganizar grupos existentes y pasillos.
3. Inclinar/bajar la consola conservando acciones originales. Adaptar navegación y comprobar los cuatro destinos originales por identidad/coordenadas.
4. Puesto horizontal y consulta a la derecha; comprobar silla, botones, libreta y oclusiones.
5. Ajustar luminarias baked y hornear con arquitectura definitiva.
6. Capturas nuevas equivalentes y pruebas finales. Registrar resultados ejecutados, no dar por aprobado Android.

## Método de circulación útil

Muestreo conservador en retícula de 0,25 m sobre el suelo, usando cápsula de radio 0,25 m desde Y=0,30 a Y=1,60. Se ignoran marcadores de suelo/silla; se conservan colliders de muebles y arquitectura. Se calcula superficie libre y su componente conectado con el destino de entrada original. Los colliders envolventes de prefabs cuentan como volumen ocupado, por lo que esto es una aproximación geométrica de circulación, no una certificación arquitectónica o de accesibilidad. Se usa exactamente el mismo método antes y después.

Métricas, tabla final, resultados y diferencias visuales pendientes de completar tras ejecutar las etapas.

El ancho angular geom?trico nominal del panel desde el asiento es aproximadamente 66? (2,10 m a 1,61 m de profundidad). Es un c?lculo del plano, no una medici?n ?ptica Cardboard; probar giro y comodidad en HONOR. El acabado final a?n es m?s oscuro y plano que el arte aprobado. La iluminaci?n final usa un atlas activo; no se deduce ahorro de memoria sin medir resoluci?n y carga en dispositivo.

Checkpoint final completo (Assets, Packages, ProjectSettings y Docs): `D:\UnityGames\SWIFT_Backups\ExpandedBankOffice-20261103-004324\FinalCheckpoint-20261103T174726Z`. El estado inicial permanece en la ra?z del respaldo y permite restauraci?n selectiva. Log Editor final saneado en Artifacts/ExpandedBankOffice/Editor-final-redacted.log.
