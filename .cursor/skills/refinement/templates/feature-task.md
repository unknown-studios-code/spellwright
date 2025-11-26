# Feature Task Template

Use for new gameplay/user-facing functionality. Replace all placeholder content with real implementation details.

---

```markdown
# [Feature Title - Specific and Implementation-Focused]

### 📝 Description

[Describe what needs to be implemented from a gameplay/user perspective. Be specific about the visible behavior.]

---

### ✅ Definition of Done

1. [Create specific file/component with exact path]
2. [Implement specific functionality]
3. [Add specific fields/properties]
4. [Write tests for specific scenarios]
5. [Profile and verify performance target]
6. [Verify no errors or warnings in Unity Console]
7. [Add XML documentation comments to all public APIs]
8. Branch created following naming convention and link added to Notion task property "Branch"
9. PR created following PR template and link added to Notion task property "Pull Request"
10. Pull Request reviewed, approved, and merged

---

### ⚙️ Technical Refinement

**Core Scripts:**

- `Assets/Scripts/[Category]/[FileName].cs` - [Script responsibility]
- `Assets/Scripts/[Category]/[FileName].cs` - [Script responsibility]

**Key Prefabs/Assets:**

- `Assets/Resources/[Path]/[Asset].asset` - [Asset purpose]
- `Assets/Prefabs/[Path]/[Prefab].prefab` - [Prefab purpose]

**Inspector Values:**

- In `[Component].cs`, expose `[type] [FieldName] = [default]` (description)
- In `[Config]`, add `[field]` field (range: [min]-[max])

**Implementation Notes:**

- Use `[specific API/pattern]` for [specific purpose]
- Apply `[BurstCompile]` to [specific targets]
- Use `EntityCommandBuffer.ParallelWriter` if creating/destroying entities in job
- [Additional implementation guidance]

**Dependencies:**

- Requires `[Component/System]` from **SPWT-X**
- Requires `[Asset/Data]` to have `[specific property]`

---

### ⚠️ Potential Risks

- 🔴 **Performance Risk:** [Risk description]
    - Mitigation: [How to address]
- 🟡 **Edge Case:** [Risk description]
    - Mitigation: [How to address]
- 🟠 **Integration Risk:** [Risk description]
    - Mitigation: [How to address]
- 🔵 **Testing Challenge:** [Risk description]
    - Mitigation: [How to address]

---

### 🔗 References

- **🎨 Design Files:**
    - [Behavior Flow Diagram](URL)
    - [Reference Video](URL)
- **📚 Documentation:**
    - [Unity DOTS Documentation](URL)
    - [Project Rules](path/to/rules)
- **🌐 External Resources:**
    - [Reference Implementation](URL)
```

---

## Feature Task Quality Checks

- [ ] Description focuses on user-facing behavior
- [ ] Definition of Done includes code, tests, docs, and PR workflow (10+ items typical)
- [ ] File paths are exact and follow project structure (`Assets/Scripts/[Category]/[FileName].cs`)
- [ ] Implementation Notes include algorithm choices and performance considerations
- [ ] Dependencies list specific tasks/systems (with SPWT-X or system names)
- [ ] Risks are implementation-specific and categorized
- [ ] Performance targets included ("<0.5ms for 100 entities")
