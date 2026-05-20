# AGENTS

## Purpose

Guidance for AI coding agents working in this repository.

## Read First

- Project overview and setup: [README.md](README.md)
- Generated registry warning: [src/__registry__/README.md](src/__registry__/README.md)
- Scripts and toolchain versions: [package.json](package.json)

## Tech Stack

- Next.js 16 App Router
- React 19 + TypeScript (strict)
- Tailwind CSS v4 + shadcn/ui

## Working Agreement

- Prefer small, focused edits and preserve existing style.
- Use the path alias pattern from TypeScript config, for example imports rooted at @/.
- Keep UI component usage consistent with the existing registry and component patterns.
- Update only files relevant to the task.

## Key Directories

- App entry and global layout: [src/app](src/app)
- Reusable demos/components: [src/components](src/components)
- Source-of-truth shadcn registry UI primitives: [src/registry/new-york-v4/ui](src/registry/new-york-v4/ui)
- Auto-generated registry artifacts: [src/__registry__](src/__registry__)

## Do Not Edit

- Files in [src/__registry__](src/__registry__) are generated. See [src/__registry__/README.md](src/__registry__/README.md).

## Common Commands

- Install dependencies: pnpm install
- Start dev server: pnpm dev
- Build production bundle: pnpm build
- Run lint: pnpm lint
- Auto-fix lint issues: pnpm lint:fix
- Check formatting: pnpm format
- Type check: pnpm type-check

## Validation Before Finishing

- Run pnpm lint
- Run pnpm type-check
- If behavior changed, run pnpm build

## Project-Specific Notes

- The default starter page imports components from [src/app/(delete-this-and-modify-page.tsx)](src/app/(delete-this-and-modify-page.tsx)).
- For new product work, replace that area with domain-specific app routes/components.
- Theme handling is wired in [src/app/layout.tsx](src/app/layout.tsx) via next-themes.
