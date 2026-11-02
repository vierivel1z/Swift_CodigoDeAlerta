# Visual improvements — VR Banking Office

Descripción preparada para revisión; esta tarea no abre ni fusiona una Pull Request automáticamente.

## Resumen

Conserva y versiona la oficina bancaria actual de Unity 6000.3.24f1: iluminación horneada, materiales, escena BancoCorresponsal, dependencias de modelos, UI modular con TMP y recursos artísticos. Completa feedback de mirada, tutorial opcional sin puntuación y preparación de audio por eventos.

## Escena y recursos

- BancoCorresponsal y sus referencias, prefabs, materiales, lightmaps/LightingData y ajustes de iluminación.
- Metadatos de modelos utilizados con UV secundaria para recuperar el bake.
- Cinco documentos PNG conservados como referencias; interfaz reconstruida con UI/TMP, sin usar las imágenes verticales como fondos de monitor.
- Un cuadro institucional mate y los dos momentos ilustrados de la impresora. El tercer momento usa un render real de BancoCorresponsal; storyboard y tercera ilustración conceptual se conservan como referencias, sin afirmar que esa oficina conceptual esté integrada.
- `.meta`, GUID, biblioteca de cues y configuración de Build Settings con BancoCorresponsal primero/habilitado; escenas de prueba conservadas deshabilitadas.

## Interfaz, interacción y audio

- Expediente por campos, registro, alertas existentes, procedimiento y comparación de resultados. Se mantienen datos/cálculos bancarios y tres decisiones originales.
- Se conserva la corrección que mantiene el panel delante del collider de computadora.
- Retícula de contraste blanco/negro, progreso cian a 1,5 s, confirmación y aviso de apartar la mirada para rearmar.
- Tutorial opcional: mirar, esperar, monitor de práctica, demostración de expediente y decisión sin puntuación ni creación de sesión bancaria.
- Instrucciones de Android separadas de PC; buses UI/Objetos/Alertas/Ambiente/Música, ocho disparadores y tonos TEMP explícitos. Ambiente/música/sonidos finales pendientes; no hay descargas ni AudioMixer nuevo.

## Iluminación, materiales y Cardboard

Incluye el bake y los materiales de la oficina que ya estaban localmente, con sus referencias. El cuadro añade dos materiales mate URP Lit sin emisión intensa. No se migró URP 17.3.0 ni se cambió el paquete/configuración de Cardboard. Se conservan cuatro destinos, portales y cinco funciones.

## Validación ejecutada

- DomainTests: 27 PASS.
- GameplayFlowProbe: PASS, prólogo omitido y 12 s reales, ambos modos, comparación y tres decisiones 6/6.
- BankOfficeProbe: PASS, nueve roles alcanzables por raycast, cinco funciones y cuatro teletransportes.
- PrototypeSmokeProbe: PASS, TMP, decisiones, resultados, teletransportes y dwell/latch/rearme.
- Revisión UX: PASS, tutorial mediante mirada, sin activación temprana/doble ni efectos en puntuación; texto sin desbordes y audio sin disparos por simples refrescos.
- Escena guardada desde Unity, dependencias nativas existentes, ausencia de scripts MonoBehaviour faltantes y lightmaps recuperables comprobados en el Editor.

## Capturas e informes

- [UX, retícula y tutorial](../Previews/UXGameplayReview/20261102T205325756Z/README.md).
- [Documentos bancarios](../Previews/DocumentUIReview/20261102T143810058Z/README.md).
- [Cuadro y prólogo](../Previews/ArtIntegrationReview/20261102T145831568Z/README.md).
- [Avance UX](UX_GAMEPLAY_PROGRESS.md), [integración artística](ART_INTEGRATION_REPORT.md), [interfaz documental](DOCUMENT_UI_IMPLEMENTATION.md), [estado funcional](FUNCTIONAL_PROTOTYPE_VALIDATION.md).

## Limitaciones pendientes

Las pruebas son PC/Unity Editor. No certifican Android, render estéreo, selección física, giroscopio, confort, audio final, FPS o temperatura en HONOR. Hace falta una APK nueva con los cambios actuales. No se realizó una importación limpia en otro equipo ni se incluyen APK, Library, caches, respaldos o logs de sesión.

## Checklist de revisión

- [ ] Clonar la rama y abrir con Unity 6000.3.24f1, esperar la resolución de Packages.
- [ ] Abrir BancoCorresponsal y comprobar materiales, bake, referencias y Console.
- [ ] Ejecutar Play: portales, prólogo, tutorial/omisión y tres decisiones.
- [ ] Revisar cuadros, datos dinámicos, contraste y estados de mirada.
- [ ] Generar APK nueva y ejecutar el checklist HONOR/Cardboard.
- [ ] Sustituir tonos TEMP por clips finales con procedencia/licencia documentada.
- [ ] Aprobar visualmente antes de fusionar; no se ha hecho merge automático.
