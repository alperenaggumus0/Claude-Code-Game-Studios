# Sprint 1 -- 2026-03-26 to 2026-04-01

## Sprint Goal
Create functional VR archaeological excavation scene with XRI integration, flashlight, tablet, and haptic feedback.

## Capacity
- Total days: 5
- Buffer (20%): 1 day reserved for unplanned work
- Available: 4 days

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-01 | Create excavation scene layout with cave environment | level-designer | 0.5 | None | Scene exists with cave geometry, lighting, and work area |
| S1-02 | Install & configure Unity XR Interaction Toolkit (XRI) | gameplay-programmer | 0.5 | None | XR Rig imported, grab/interact system functional |
| S1-03 | Create shovel prefab with XR Grabbable component | gameplay-programmer | 0.5 | S1-02 | Shovel grabbable, can be picked up, properly scaled |
| S1-04 | Implement digging mechanic with soil removal | gameplay-programmer | 1.0 | S1-03 | Shovel contact removes soil, particle effects spawn |
| S1-05 | Create dinosaur bone prefabs with XR Socket interactable | gameplay-programmer | 0.5 | S1-02 | Bones socket-compatible, can be placed in hands |
| S1-06 | Implement bone excavation trigger (reveal when cleared) | gameplay-programmer | 0.5 | S1-04 | Bones hidden until soil above is cleared |
| S1-07 | Add haptic feedback on shovel-soil contact | gameplay-programmer | 0.5 | S1-04 | Controller vibrates on each dig contact |

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-08 | Create flashlight prefab with XR Grabbable | gameplay-programmer | 0.5 | S1-02 | Flashlight grabbable, toggle on/off with button |
| S1-09 | Set up cave lighting (dark environment for flashlight) | technical-artist | 0.5 | S1-01 | Scene is dark, flashlight effectively illuminates |
| S1-10 | Create virtual tablet prefab (UI canvas on world object) | ui-programmer | 0.5 | None | Tablet grabbable, displays UI in VR |
| S1-11 | Implement tablet content system (3D hologram + info) | ui-programmer | 1.0 | S1-10 | Showing 3D model, name, age, weight when bone held |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|-------------|-------------------|
| S1-12 | Add dig sound effects with spatial audio | sound-designer | 0.5 | S1-04 | Dig sounds play at shovel position |
| S1-13 | Create soil particle system | technical-artist | 0.5 | S1-04 | Realistic dirt particles spawn when digging |
| S1-14 | Add camera system for bone photos on tablet | gameplay-programmer | 0.5 | S1-10 | Can "photo" bones to add to collection |
| S1-15 | Implement completion checkpoint (all bones found) | gameplay-programmer | 0.5 | S1-06 | Screen shows progress and completion message |

## Carryover from Previous Sprint
| Task | Reason | New Estimate |
|------|--------|-------------|
| None | First sprint | N/A |

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| XRI compatibility issues with VR headset | Medium | High | Test on target hardware early; have fallback grab system ready |
| Performance issues with particle effects | Medium | Medium | Use simple particles, optimize mesh count |
| Tablet UI readability in VR | Medium | Medium | Test font sizes, adjust canvas scale |
| Haptic feedback varies by controller | Low | Low | Use generic haptic API, test on major controllers |

## Dependencies on External Factors
- Unity XR Interaction Toolkit package installation (via Unity Package Manager)
- VR headset for testing (at least one target device)

## Definition of Done for this Sprint
- [ ] All Must Have tasks completed
- [ ] All tasks pass acceptance criteria
- [ ] No S1 or S2 bugs in delivered features
- [ ] Design documents updated for any deviations
- [ ] Code reviewed and merged
- [ ] All assets located in `Assets/alperen/` only
- [ ] No modification to other workers' files

## Folder Structure

```
Assets/alperen/
├── Scripts/
│   ├── Interaction/
│   │   ├── DiggingController.cs
│   │   ├── BoneExcavation.cs
│   │   ├── HapticFeedback.cs
│   │   └── FlashlightController.cs
│   └── Tablet/
│       └── TabletDisplay.cs
├── Prefabs/
│   ├── Tools/
│   │   ├── Shovel.prefab
│   │   └── Flashlight.prefab
│   ├── Collectibles/
│   │   ├── DinosaurBone_1.prefab
│   │   ├── DinosaurBone_2.prefab
│   │   └── DinosaurBone_3.prefab
│   └── UI/
│       └── Tablet.prefab
├── Scenes/
│   └── ExcavationCave.unity
├── Materials/
│   ├── Soil.mat
│   ├── Bone.mat
│   └── CaveInterior.mat
├── Models/
│   ├── Shovel.fbx
│   ├── Flashlight.fbx
│   ├── Tablet.fbx
│   └── Bones/
│       ├── Femur.fbx
│       ├── Skull.fbx
│       └── Ribcage.fbx
├── Textures/
│   ├── Soil_Albedo.png
│   ├── Bone_Albedo.png
│   └── CaveWall_Albedo.png
├── Animations/
│   └── ShovelAnim.anim
└── Audio/
    ├── Dig_Sound.wav
    └── Flashlight_On.wav
```

## Notes
- **Critical Rule**: All work must be contained within `Assets/alperen/`. Do NOT modify any files in other team members' directories.
- **XR Setup**: XR Interaction Toolkit handles most interaction logic; focus on custom mechanics.
- **Testing**: Requires VR hardware for full validation.
