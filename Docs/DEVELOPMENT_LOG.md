# AkkaSync Development Log

This log tracks AkkaSync’s development progress, including completed features, ongoing work, and planned improvements. It is intended to keep contributors and users informed.

---
| Status | Feature / Task | Description | Start Date | End Date|
|--------|----------------|------------|------|------
| ✅ Completed | Feasibility & Value Research | Research to assess technical feasibility and project value | 2025-11-03 |2025-11-17|
| ✅ Completed | Actor-based ETL pipeline | Implemented concurrent, modular pipelines using Akka.NET | 2025-11-18 | 2025-11-2 |
| ✅ Completed | Plugin system | Supports multiple Source/Sink plugins and custom Transformer components | 2025-11-26 | 2025-11-29 |
| ✅ Completed | Core Sink & Source Implementation | Implemented SqliteSink, CsvSource, and synchronization cursor mechanism to enable data ingestion and incremental sync | 2025-11-30 | 2025-12-03 |
| ✅ Completed | Project Restructure, Plugin Modularization & DI Support | Refactored project structure to isolate core runtime and plugins, enabling each plugin to be developed, versioned, and published independently; added dependency injection support for AkkaSync modules | 2025-12-04 | 2025-12-05 |
| 🔄 In Progress | AkkaSync.Host & Plugin Auto-Loading | Developing the AkkaSync.Host console application and implementing plugin scanning and automatic loading for dynamic module integration | 2025-12-06 | TBD |
| 📝 Planned | Dashboard ↔ Host Communication | Plan and implement communication mechanism between AkkaSync.Dashboard and AkkaSync.Host for monitoring and controlling pipelines | TBD | TBD |

---

*Note: Items marked in 🔄 are actively being worked on, while 📝 are planned features under consideration.*
