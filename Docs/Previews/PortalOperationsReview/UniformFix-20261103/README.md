# Carga uniforme — corrección del 3 de noviembre de 2026

Capturas reales de Unity 6000.3.24f1, Play Mode, URP existente, FOV 60°. Un ratón virtual del Input System envía posición y pulsación al interactor real. La prueba no escribe el progreso ni el `fillAmount`.

- [Hover sin pulsar: 0 %](PC-0.png).
- [Pulsación mantenida: 49,204 %](PC-50.png).
- [Soltar a mitad: vuelve a 0 %](PC-release-0.png).
- [DEFENSA completa: 100 % y confirmación](PC-100.png).

La barra y la retícula leen el mismo contador. La tarjeta contraria permanece vacía. Las capturas anteriores del portal se conservaron; las nuevas tienen sus propios hashes en `hashes.json` y recibos en el `receipt.txt` de la carpeta superior.

Regresión PC, regresión de mirada, flujo, oficina y smoke: PASS Editor. HONOR/Android no probado.

[Informe de la corrección](../../../Archive/2026-11-10/Context/PORTAL_UNIFORM_FILL_FIX.md).
