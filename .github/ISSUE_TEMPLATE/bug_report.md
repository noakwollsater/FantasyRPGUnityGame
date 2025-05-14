name: 🐛 Bug Report
description: Rapportera ett fel eller oväntat beteende i spelet
title: "[BUG] "
labels: [bug]
assignees: []

body:
  - type: input
    id: version
    attributes:
      label: Spelversion
      description: Vilken version av spelet gäller buggen?
      placeholder: t.ex. v1.2.3 eller Beta Build 5
    validations:
      required: true

  - type: input
    id: environment
    attributes:
      label: Miljö / Plattform
      description: Vilket operativsystem, plattform eller hårdvara?
      placeholder: t.ex. Windows 11, Steam Deck, Xbox Series S
    validations:
      required: true

  - type: textarea
    id: description
    attributes:
      label: Vad hände?
      description: En tydlig och kortfattad beskrivning av buggen.
      placeholder: Spelet kraschar direkt när jag öppnar kartan.
    validations:
      required: true

  - type: textarea
    id: expected
    attributes:
      label: Vad förväntade du dig skulle hända?
      description: Beskriv vad som borde ha hänt istället.
      placeholder: Kartan skulle ha öppnats normalt.
    validations:
      required: false

  - type: textarea
    id: steps
    attributes:
      label: Hur man återskapar buggen
      description: Lista steg-för-steg hur man får buggen att hända igen.
      placeholder: |
        1. Starta spelet
        2. Ladda ett sparat spel
        3. Öppna kartan
        4. Spelet kraschar
    validations:
      required: true

  - type: textarea
    id: logs
    attributes:
      label: Felmeddelanden / loggar (om tillgängligt)
      description: Klistra in loggar eller kraschmeddelanden här, om du har några.
      placeholder: t.ex. NullReferenceException i PlayerManager.cs:28
    validations:
      required: false

  - type: dropdown
    id: severity
    attributes:
      label: Allvarlighetsgrad
      description: Hur allvarlig är buggen?
      options:
        - 🔹 Mindre (påverkar inte spelbarhet)
        - ⚠️ Medel (kan störa spelupplevelsen)
        - ❌ Kritisk (gör spelet ospelbart)
    validations:
      required: true
