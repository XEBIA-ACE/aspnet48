# Implementation Plan: US-001

## Technical Context
- unresolved reference (service:SVC-004)
- unresolved reference (FR-US-001-001)
- unresolved reference (FR-US-001-002)
- unresolved reference (US-001)
- unresolved reference (US-001-ac-1)
- unresolved reference (US-001-ac-2)

## Contract Changes
- change required: A new external API endpoint POST /api/v1/auth/register/email is introduced for service SVC-004, which is a material API contract addition.
- change required: New unauthenticated HTTPS-only registration endpoint that accepts EmailRegistrationRequest and returns 201 on successful user creation or 409 with ErrorResponse when the email already exists.
- no change: Existing UMS registration endpoint POST /api/v1/ums/registration is reused internally by the new /api/v1/auth/register/email endpoint without changing its external contract.

## Security and Quality
- Validate credential and personal-data handling against the story's acceptance criteria

## Verification Plan
- **US-001-ac-1** (acceptance_criterion, tier B, trust HYPOTHESIS): Implement the /api/v1/auth/register/email POST endpoint to accept a valid EmailRegistrationRequest and create a new user account when the email is unique.
  - Test obligation: 0753a206-d975-40e7-bc4f-dca56f67c3fc:test:acceptance:US-001-ac-1 (required_before=release, required_environment=ci)
  - Expected evidence: test result for US-001-ac-1
- **US-001-ac-2** (acceptance_criterion, tier B, trust HYPOTHESIS): Ensure the /api/v1/auth/register/email POST endpoint returns a 409 error with an ErrorResponse when an email already exists in the database.
  - Test obligation: 0753a206-d975-40e7-bc4f-dca56f67c3fc:test:acceptance:US-001-ac-2 (required_before=release, required_environment=ci)
  - Expected evidence: test result for US-001-ac-2

## Design and Implementation Notes
- Implement changes in grounded service SVC-004.
- Within SVC-004, introduce a new unauthenticated HTTPS-only endpoint POST /api/v1/auth/register/email that accepts an EmailRegistrationRequest payload, ensuring that this endpoint is internally wired to the existing UMS registration capabilities exposed via POST /api/v1/ums/registration without duplicating business logic. [service:SVC-004, FR-US-001-001, US-001-ac-1]
- In SVC-004, implement data access logic against the PostgreSQL ums schema to check for an existing User record keyed by the email in EmailRegistrationRequest; when no such record exists, create a new User (and any minimally required related entities such as UserStatus) associated with that email, deferring all other validation and identity verification to other epics. [service:SVC-004, dependency:PostgreSQL ums schema, FR-US-001-001, US-001-ac-1]
- In SVC-004, implement control flow so that when the email in EmailRegistrationRequest already exists in the ums schema, the POST /api/v1/auth/register/email handler does not create a new User and instead returns HTTP 409 with a response body structurally conforming to ErrorResponse (fields and localization left unspecified by this story). [service:SVC-004, dependency:PostgreSQL ums schema, FR-US-001-002, US-001-ac-2]
- Configure Spring Security within SVC-004 so that POST /api/v1/auth/register/email is accessible without authentication but only over HTTPS/TLS, aligning with the story description that discovery and access are unauthenticated yet secure; ensure no sensitive data from EmailRegistrationRequest is ever placed into URL parameters (path or query). [service:SVC-004, technology:Spring Security (OAuth2 Resource Server), US-001]

## Gaps and Unresolved Items
- Implementation constraints reference unresolved items and must be clarified before coding:
  - Ground implementation decisions in unresolved reference (service:SVC-004)
  - Ground implementation decisions in unresolved reference (FR-US-001-001)
  - Ground implementation decisions in unresolved reference (FR-US-001-002)
  - Ground implementation decisions in unresolved reference (US-001)
  - Ground implementation decisions in unresolved reference (US-001-ac-1)
  - Ground implementation decisions in unresolved reference (US-001-ac-2)
- No additional unresolved API impact items are recorded for this story.

## Provenance
- Functional behavior source: spec_functional (gate 3, model gpt-5.1, prompt SPEC_FUNCTIONAL_GENERATE/v4)
- Technical design source: spec_technical_design (gate 4, degraded=true, model gpt-5.1, prompt SPEC_TECHNICAL_DESIGN_GENERATE/v6)
- API impact source: spec_api_impact (gate 5, model gpt-5.1, prompt SPEC_API_IMPACT_ANALYZE/v6)
- Implementation specification story_id: US-001
- Evidence references used in design: service:SVC-004, FR-US-001-001, FR-US-001-002, US-001, US-001-ac-1, US-001-ac-2
- Accepted impact revision: none (accepted_impact_hash = null)
- Policy version: ace-spec-format/v1
- Repository baseline commit: b69a7ffae0f5d721e163bc9ab9d4ea7b7e062a86