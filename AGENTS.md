# Agent guidance

Follow the repository architecture, build, accessibility, and coding guidance in `.github/copilot-instructions.md`.

## Designing control samples

- Structure a control page as a learning progression that explains the control and its capabilities, not as one deep, application-specific scenario.
- Begin with the simplest useful example, then add focused examples for distinct concepts such as configuration, states, input, events, data binding, selection, customization, and accessibility when they apply.
- Make each `ControlExample` teach one clear idea. Its heading, UI, and code snippet should make that idea understandable without requiring context from another example.
- Keep sample data, labels, and supporting models small, neutral, and reusable. Do not introduce business workflows, large domain models, or unrelated application infrastructure unless the control requires them.
- Prefer several concise, logically separated examples over one comprehensive scenario that combines many features.
- Add a scenario-based example only after the foundational concepts are covered and only when the scenario is the clearest way to demonstrate how multiple control capabilities work together.
