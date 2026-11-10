# Prólogo cercano — capturas reales de Unity

Unity 6000.3.24f1, Play Mode, URP existente. Vista desde la posición y rotación reales de la cámara del jugador, con su FOV de 60° y aspecto actual. Las cámaras temporales de captura no se guardan en la escena.

1. [Prólogo delante del portal](01-portal-near-prologue.png).
2. [Seguimiento tras girar 55° y desplazarse](02-follow-turn-and-move.png).
3. [Distancia adaptada muy cerca de la consola](03-close-console-clearance.png).
4. [Botón Omitir alcanzable con la mirada](04-gaze-skip-stays-reachable.png).

El Canvas de los modos se oculta temporalmente para que sus textos no compitan con el prólogo. El marco físico permanece. La prueba seleccionó «Omitir» con el raycast real y el dwell de 1,5 s y comprobó la restauración del portal.

`receipt.txt` registra fechas UTC, posición, rotación, FOV y aspecto. `hashes.json` contiene SHA-256 de los cuatro PNG. Se abrieron y revisaron las capturas; no son mockups ni renders de IA.

La distancia máxima prevista es 1,1 m y se acorta ante obstáculos. El tamaño angular se conserva. La comodidad binocular y el rendimiento de Cardboard siguen pendientes de dispositivo.

[Informe de implementación y pruebas](../../Archive/2026-11-10/Context/INTRO_CAMERA_FOLLOW_FIX.md).
