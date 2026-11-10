# Matriz de implementación inmersiva

Fecha: 10 de noviembre de 2026. Fuentes: los cuatro documentos completos de Docs/Research y auditoría del 8/11. Actualizada tras integración y pruebas reales de Editor; Android físico sigue pendiente. Existencia de scripts no equivale a prueba.

Estados: V = implementada y verificada (con plataforma); IV = implementada pendiente de validación; C = pendiente viable con Cardboard; A = pendiente de arte/audio; H = pendiente de hardware; O = opcional/descartada por riesgo.

| ID | Prioridad | Tarea y aceptación | Archivos afectados | Estado | Evidencia / próximo paso |
|---|---|---|---|---|---|
| B01 | P0 | Git, concurrencia y respaldo recuperable | Escena/scripts/ProjectSettings/Packages | V | 41 archivos respaldados; ruta en Artifacts/ImmersiveGameplay/backup-path.txt |
| B02 | P0 | Dominio sin regresión | BankingDomain, Tools/DomainTests | V .NET | 27/27 ejecutadas hoy; dominio debe permanecer idéntico |
| B03 | P0 | Flujo, oficina, smoke, tutorial y prólogo | Sondas/runners existentes | V Editor | flow/office/smoke PASS en arranques limpios 10/11; tutorial real 43 comprobaciones. Recibos en Artifacts/ImmersiveGameplay. |
| B04 | P0 | Estado real de escena/destinos/ediciones sin guardar | BancoCorresponsal y Editor | V Editor | Principal guardada y perfil integrado; diferencias y SHA conservados. Copia aislada con GUID propio. |
| C01 | P1 | Reutilizar madera, clip y papel | Analyst review clipboard | V Editor | Madera/clip/papel existentes conservados; prefab de revisión independiente, capturas antes/después. |
| C02 | P1 | Inspeccionar sentado por dwell único 1,5 s | Coordinador/vista/BankInteractable | V Editor | clipboard-083536: selección real 1,5 s y vista world-space; sin padre en cámara. |
| C03 | P1 | Hojas: formato, expediente, observaciones/fuentes | TMP y proyección de solo lectura | V Editor | Tres hojas TMP, datos existentes y observaciones sin decisión recomendada; 109 comprobaciones de portapapeles. |
| C04 | P1 | Marcas manuales separadas por sesión e ID | Diario auxiliar | V Editor/.NET | Marcas manuales por sesión/ID, reset y ausencia de efectos bancarios comprobados. |
| C05 | P1 | Paginación/cierre y conservación de secundario | UIManager/OperationsStation | V Editor | Paginación real, cierre y conservación de ambos monitores comprobados. Perfil ausente restaura Notebook→Procedure. |
| C06 | P0 | Dwell 0/50/100, cancelar/cambiar, latch, PC | VRInteractionManager/DocumentFocus/prueba | V Editor | 0/50/100 real, mirar fuera y latch; PC 14 comprobaciones con hover/cancelación/hold. Android físico pendiente. |
| C07 | P0 | Modal bloquea botones ocultos, no vista principal | Elegibilidad y collider | V Editor | CanActivate y Activate directo bloquean controles ocultos; raycast único. |
| C08 | P1 | Datos largos, sin overflow y lectura cómoda | TMP/capturas/HONOR | V Editor / H visor | TMP 36 y datos muy largos paginados; Game View revisado. Lectura binocular y comodidad pendientes. |
| T01 | P1 | Teléfono aislado DEFENSA/SIM-003 | Prefab/catálogo/controlador nuevo | V Editor | Prefab de tres meshes con materiales existentes; elegibilidad exclusiva DEFENSA/SIM-003. |
| T02 | P1 | Solicitar/señal/responder/subtítulos/fin | Estados y panel TMP | V Editor | phone-083603: 88 comprobaciones; solicitar, pendiente, responder, subtítulos, registro, cierre/reapertura. |
| T03 | P1 | Contenido fiel a Astra y dominio | Catálogo SIM-003 | IV editorial | Catálogo separado y visible GUION DE PRUEBA; registro exacto del dominio. Falta aprobar diálogo final. |
| T04 | P0 | Registro correcto y único | RegisterEvidence/diario | V Editor/.NET | Una evidencia SIM-003, sin duplicados; cambio de sesión/registro cancela contexto anterior. |
| T05 | P0 | Pérdidas/aciertos/DefenseOverride intactos | Dominio y regresión | V Editor/.NET | Ataque 965000 y 1/6; defensa automática 0 y 1/6; secuencia correcta 6/6 y 0. Dominio idéntico. |
| T06 | P2 | NoAnswer/inconclusa y cancelación distintos | Fixtures de prueba | V .NET / IV variantes Editor | NoAnswer/inconclusa validados en DeskToolsTests; catálogo de entrega solo contiene la propuesta SIM-003. |
| T07 | P2 | Auricular a pose predeterminada | Transform/animación | O | No necesario para teléfono A |
| T08 | P3 | Agarre real manos/mandos | Stack/proveedor XR separado | H | Identificar hardware del salón; no instalar SDK |
| O01 | P1 | Foco/confirmación/inspección/retorno coherentes | Portapapeles/teléfono/documento | V Editor | Portapapeles, teléfono y documento usan foco común, inspección, controles y retorno; teléfono separado del rayo al papel. |
| O02 | P1 | Impresora perceptible, evidencia sin duplicados | Impresora/evidencia | V Editor / A sonido | Dos consultas por dwell real: una evidencia/confirmación. Registro telefónico no confirma impresora. Placa/tinte locales. |
| O03 | P3 | Carpeta/archivador/sello extra | Props | O | Posponer redundantes y pasos obligatorios |
| S01 | P1 | Fade, tracking y destino seguro | AnalystSeat/HeadPoseAdapter | V Editor / H tracking físico | AnalystSeat idéntico al respaldo; HeadPoseAdapter sin intervención. Entrada/salida/reentrada reales; componente activo, tracking físico pendiente. |
| S02 | P1 | Señal/animación de silla y sonido si existe | Presentación asiento | V Editor / A clip | Respuesta de 3,5 cm solo en mesh sin collider, dentro del fade original; disparador Chair preparado. |
| S03 | P1 | SALIR conserva expediente, restaura reposo y pads | OperationsStation/AnalystSeat | V Editor | objects-083641: SALIR cierra inspección, conserva expediente, restaura reposo y destinos; reentrada mantiene secundario. |
| S04 | P2 | Calibración/altura/comodidad | Anclajes/visor | H | Altura virtual no equivale a medición del usuario |
| A01 | P1 | Cinco categorías/buses, sin segundo gestor | FeedbackAudio/AudioCueLibrary | V Editor | FeedbackAudio y cinco categorías existentes; sin segundo gestor ni mixer nuevo. |
| A02 | P1 | Timbre/responder/fin/papel/silla/impresora | Referencias y eventos | V disparadores / A clips | Referencias PhoneRing/Answer/End/PageTurn/Chair/Printer preparadas; sin audio final inventado. Ver IMMERSIVE_AUDIO_ASSET_REQUEST. |
| A03 | P0 | Sin spam por Changed; pausa/cancelación | Audio y pruebas | V Editor / A clips / H pausa | Diez Notify sin spam comprobados; corte de timbre preparado. Mezcla audible y pausa Android pendientes. |
| A04 | P2 | Voz/ambiente/música | Clips licenciados | A | Subtítulos esenciales, voz opcional; no descargas dudosas |
| E01 | P1 | Elección original frente a estado efectivo | GameManager/UI/diario | V Editor/.NET | Evento descriptivo DecisionCommitted; resultados y revisión distinguen elección y estado efectivo/DefenseOverride. |
| E02 | P1 | Fuentes consultadas y evidencias distintas | Diario/vista educativa | V Editor/.NET | Diario auxiliar de fuentes abiertas y llamada registrada; abrir no equivale a validar ni leer. |
| E03 | P1 | Explicación posterior sin anticipar solución | Resultado/revisión | V Editor | Revisión solo posterior al turno, sin revelar soluciones durante inspección; capturas y navegación paginada PASS. |
| V01 | P1 | Paleta/materiales/legibilidad/señalética | UI y objetos locales | V Editor / H aceptación | Panel institucional gris/petróleo/cian, tipografía conservada y materiales compartidos. Galería inspeccionada; aprobación del usuario pendiente. |
| V02 | P2 | Luz del puesto/ruido/contacto | Escena/capturas | C / H medición | Oficina e intensidades conservadas; prioridad futura es medir iluminación/contraste en HONOR antes de tocar calidad. |
| V03 | P3 | Arte/texturas adicionales IA | Assets opcionales | O/A | Sin recurso nuevo indispensable; conservar originales |
| Q01 | P0 | Pruebas catálogo/contexto/deduplicación | Tests nuevos | V .NET | DeskToolsTests 37/37 y DomainTests 27/27, logs separados; sin dependencias nuevas. |
| Q02 | P0 | Raycast/PC/modal/pausa/ciclo completo | Runner Editor | V Editor / IV pausa Android | clipboard/phone/objects/PC/tutorial/regresión PASS. Pausa real Android y pérdida de tracking no probadas. |
| Q03 | P1 | Capturas nuevas e inspección visual | Docs/Previews/ImmersiveGameplay | V Editor | Capturas reales Play/Game View y cuatro vistas de edición, hashes/recibos. Herramienta de ventanas falló; APIs de Editor funcionaron. |
| Q04 | P0 | APK estable nueva/log/hash/banco inicial | Build Android existente | V build / H dispositivo | Succeeded, 0 errores/1 aviso; APK nueva Build-20261110-083825, hash independiente y logs. HONOR no probado. |
| Q05 | P0 | HONOR óptica/mirada/pausa/calor/rendimiento | Equipo real | H | No declarar probado sin ensayo físico |
| Q06 | P2 | Piloto 3–5 estudiantes/comprensión | Ficha anónima | H | No constituye evidencia estadística |
| D01 | P1 | Informe, Kanban, créditos y reversión | Docs/Context/VisualReview | V documentación | Informe, matriz, galería, audio, checklist, Kanban/changelog y créditos actualizados; historial conservado. |

## Dependencias y riesgos

Orden: base → portapapeles reversible → teléfono de prueba SIM-003 → asiento/impresora/audio → feedback/regresión → APK. Se conserva el secundario. NoResponse, cancelación y contradicción son estados distintos. Validar contexto antes de registrar evidencia; añadir acciones al final del enum; dimensionar índices de audio; no recrear objetivos durante dwell; progreso cero cuando bloqueado.

Guion telefónico final pendiente: el texto de Astra está rotulado como propuesta. El prototipo usará datos de prueba explícitos, sin cambiar valores de SIM-003. Extender a SIM-004/005, manos, físicas libres, sello obligatorio y sustitución definitiva del secundario queda fuera de alcance.

## Evidencia de base

Git: review/portal-dwell-feedback, numerosos cambios locales conservados. DomainTests: 27/27 PASS hoy. Inspección del Editor: PASS (07:04:41 UTC del 10/11). Herramienta de ventanas y Node: fallo de inicialización del sandbox; no se atribuirán a esas herramientas capturas o acciones que no realizaron.

Resultados consolidados en IMMERSIVE_AUTONOMOUS_PROGRESS.md. Historial de pruebas, incluidos fallos e invalidación del primer recibo del tutorial, en Artifacts/ImmersiveGameplay/result-history.txt. La matriz inicial se conserva en Artifacts/ImmersiveGameplay/tracker-initial.md.