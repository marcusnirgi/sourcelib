# SourceLib

![Under Heavy Development](https://img.shields.io/badge/🚧-Under%20Heavy%20Development-yellow)

**SourceLib** is a strongly typed C# library for working with the data formats of the **Source Engine** and its many branches, generations, and variations.

It aims to provide a single, consistent implementation for reading, representing, transforming, validating, and writing Source Engine data.

> **Every format. Every era. Every known variation. One library.**

## Why SourceLib?

Source has been around for decades.

Across those years, Valve and the Source community have produced an enormous amount of content across different engine branches, formats, versions, and generations. Much of that content is tied to the specific tools and assumptions of the engine version it was created for.

Working with that content today often means dealing with fragmented implementations, incomplete format support, undocumented differences, and one-off tools that understand only a particular game or version.

SourceLib is an attempt to solve the foundation of that problem.

The long-term intention is to build a **unified Source Engine data library** capable of understanding the formats and structures used throughout the Source 1 ecosystem. Once that foundation exists, it can do the heavy lifting for higher-level tooling instead of every project having to independently implement and reverse-engineer the same formats.

### The End Goal

One of the primary goals behind SourceLib is to make a future **Source 1 → Source 2 asset conversion tool** practical.

The ambition is to make it possible to take the enormous body of content created over decades for Source 1 and provide a path for bringing that content into the newer generation of the engine.

That requires considerably more than a model converter.

A useful conversion pipeline needs to understand the underlying assets and their relationships: models, meshes, materials, textures, animations, maps, data files, archives, and the many versions and variations of those formats.

SourceLib is intended to provide that foundation.

The conversion tool can then focus on **conversion** rather than spending most of its implementation on understanding the formats it is converting.

> **SourceLib understands Source. Higher-level tools can transform it.**

## Features

- Strongly typed representations of Source Engine data
- Reading **and writing** supported formats
- Explicit handling of format versions and engine variants
- Deterministic serialization
- Consistent parsing and serialization APIs
- Engine-native types such as `Vector2`, `Vector3`, `Color3`, `Angle`, and more
- Focus on correctness and complete format coverage
- No unnecessary abstraction or cleverness

SourceLib is designed to make Source Engine formats predictable to work with, even when the formats themselves are anything but predictable.

## Installation

SourceLib is currently under active development and is not yet considered stable.

Installation instructions will be provided once the public package API is established.

## Supported Formats

| Format           |     Status     | Description                               |
| ---------------- | :------------: | ----------------------------------------- |
| KV1              |  🟢 Supported  | KeyValues1                                |
| KV2              |  🟢 Supported  | KeyValues2                                |
| KV3              |  🟢 Supported  | KeyValues3                                |
| VPK              |  🟢 Supported  | Valve Pak archives (v0-v2)                |
| DMX              |  🟢 Supported  | Data Model eXchange (v1–v5)               |
| MDL              | 🟡 In-progress | StudioModel data                          |
| VVD              |   🔵 Planned   | StudioModel vertices                      |
| VTX              |   🔵 Planned   | StudioModel mesh strips                   |
| VTF              |   🔵 Planned   | Valve Texture Format                      |
| VMT              |   🔵 Planned   | Valve Material Type                       |
| And lots more... |   🔵 Planned   | Additional formats and engine generations |

### Status

- 🟢 **Supported** — Implemented and usable.
- 🟡 **In-progress** — Implementation is underway, but coverage is incomplete.
- 🔵 **Planned** — Intended for a future implementation.

Support is tracked at the format and version level where necessary. A format being listed as supported does not necessarily mean every historical Source branch is identical or interchangeable.

## Scope

The eventual scope of SourceLib is deliberately broad.

It is intended to cover essentially every significant Source Engine data format across the relevant generations of the engine, including the shared structures and engine concepts that connect those formats.

This includes:

- Asset formats
- Archive formats
- Model data
- Mesh data
- Texture data
- Material data
- Map data
- Animation data
- Engine-native types and structures
- Version- and branch-specific variations

The objective is not simply to **read files**.

SourceLib should be capable of **understanding, representing, modifying, and writing Source Engine data** in a consistent way.

That foundation can then be used by:

- Asset conversion tools
- Editors
- Asset pipelines
- Modding tools
- Map and model tooling
- Data analysis tools
- Server infrastructure
- Other Source Engine projects

## Beyond Source 1

SourceLib starts with Source 1 because that is where the problem of fragmented historical content is particularly significant.

The broader vision is to make Source Engine data accessible through a common, strongly typed foundation, allowing higher-level tooling to work across generations rather than being permanently tied to one game's implementation.

The ultimate ambition is simple:

**Preserve decades of Source content by making it possible to move it forward.**

## Project Status

SourceLib is **actively developed**.

The API and internal implementations may change substantially as format coverage expands and the library's conventions mature.

This is a long-term project. Complete coverage of the Source ecosystem is intentionally ambitious, and formats will be implemented incrementally.

The goal is not to be the smallest Source parser.

The goal is to become the **reliable foundation for working with Source Engine data**.
