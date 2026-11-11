# Tasks: US-001

## Ordered Implementation Tasks
- [ ] T001 — Implement new endpoint contract and wiring
  - Implement changes in grounded service SVC-004.
  - Expose POST /api/v1/auth/register/email as an unauthenticated HTTPS-only endpoint that accepts EmailRegistrationRequest.
  - Wire POST /api/v1/auth/register/email internally to the existing POST /api/v1/ums/registration capabilities without duplicating business logic. [service:SVC-004, FR-US-001-001, US-001-ac-1]

- [ ] T002 — Implement unique-email user creation path
  - In SVC-004, implement data access logic against the PostgreSQL ums schema to check for an existing User record keyed by the email in EmailRegistrationRequest.
  - When no such record exists, create a new User (and any minimally required related entities such as UserStatus) associated with that email.
  - Defer all other validation and identity verification to other epics. [service:SVC-004, dependency:PostgreSQL ums schema, FR-US-001-001, US-001-ac-1]

- [ ] T003 — Implement existing-email conflict behavior
  - In SVC-004, implement control flow so that when the email in EmailRegistrationRequest already exists in the ums schema, the POST /api/v1/auth/register/email handler does not create a new User.
  - Return HTTP 409 (Conflict) with a response body structurally conforming to ErrorResponse (fields and localization left unspecified by this story). [service:SVC-004, dependency:PostgreSQL ums schema, FR-US-001-002, US-001-ac-2]

- [ ] T004 — Enforce security constraints
  - Configure Spring Security within SVC-004 so that POST /api/v1/auth/register/email is accessible without authentication but only over HTTPS/TLS.
  - Ensure no sensitive data from EmailRegistrationRequest is ever placed into URL parameters (path or query). [service:SVC-004, technology:Spring Security (OAuth2 Resource Server), US-001]
  - Validate credential and personal-data handling against the story's acceptance criteria.

## Verification Tasks
- [ ] T005 — Verify US-001-ac-1
  - Design and implement tests in the CI environment that assert: Implement the /api/v1/auth/register/email POST endpoint accepts a valid EmailRegistrationRequest and creates a new user account when the email is unique.
  - Link tests to obligation ID 0753a206-d975-40e7-bc4f-dca56f67c3fc:test:acceptance:US-001-ac-1.
  - Capture and store evidence: test result for US-001-ac-1.

- [ ] T006 — Verify US-001-ac-2
  - Design and implement tests in the CI environment that assert: Ensure the /api/v1/auth/register/email POST endpoint returns a 409 error with an ErrorResponse when an email already exists in the database.
  - Link tests to obligation ID 0753a206-d975-40e7-bc4f-dca56f67c3fc:test:acceptance:US-001-ac-2.
  - Capture and store evidence: test result for US-001-ac-2.

## Notes on Constraints and Gaps
- The following implementation constraints are present and currently unresolved; tasks may need refinement once these references are clarified:
  - Ground implementation decisions in unresolved reference (service:SVC-004).
  - Ground implementation decisions in unresolved reference (FR-US-001-001).
  - Ground implementation decisions in unresolved reference (FR-US-001-002).
  - Ground implementation decisions in unresolved reference (US-001).
  - Ground implementation decisions in unresolved reference (US-001-ac-1).
  - Ground implementation decisions in unresolved reference (US-001-ac-2).
- ErrorResponse structure and localization are explicitly out of scope and remain ambiguous for this story; do not over-specify them in implementation or tests.

## Provenance
- Derived from implementation_specification for story US-001, including functional_behavior, clauses, api_event_data_contracts, test_obligations, and spec_technical_design.design_steps.
- Accepted impact revision: none (accepted_impact_hash = null).
- Policy version: ace-spec-format/v1.
- Repository baseline commit: b69a7ffae0f5d721e163bc9ab9d4ea7b7e062a86.