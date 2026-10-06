---
name: winui-gallery-blog-post
description: Draft the "Announcing WinUI Gallery X.Y" release blog post for the #ifdef Windows DevBlogs site. Use when asked to write, draft, or outline a WinUI Gallery release blog post, release announcement, or DevBlogs post for a new Gallery version.
---

# WinUI Gallery release blog post

Draft the release announcement for a new WinUI Gallery version in the voice and
structure of previous posts on [#ifdef Windows](https://devblogs.microsoft.com/ifdef-windows/tag/winui-gallery/).

Reference posts (read at least one before drafting if you have web access):

- [Announcing WinUI 3 Gallery 2.9](https://devblogs.microsoft.com/ifdef-windows/announcing-winui-3-gallery-2-9/) (May 2026)
- [Announcing WinUI Gallery 2.8](https://devblogs.microsoft.com/ifdef-windows/announcing-winui-gallery-2-8/) (March 2026)
- [Announcing WinUI Gallery 2.7](https://devblogs.microsoft.com/ifdef-windows/announcing-winui-gallery-2-7/) (September 2025)

## Workflow

1. **Identify the release range.** The previous release is the latest GitHub
   release tag (`gh release view --repo microsoft/WinUI-Gallery --json tagName`).
   The new version is the `<Version>` in `WinUIGallery/WinUIGallery.csproj`.
2. **Gather the data.** Run the helper script from the repository root:

   ```powershell
   .\.github\skills\winui-gallery-blog-post\scripts\Get-ReleaseData.ps1 -OutFile release-data.md
   ```

   It lists every merged PR since the previous tag, grouped by area, plus the
   contributors outside the core team. The grouping is a heuristic; re-check it.
3. **Read the important PRs.** For each candidate headline, run
   `gh pr view <number> --repo microsoft/WinUI-Gallery` and read the description
   and screenshots. Do not invent behavior; describe what the PR actually ships.
4. **Pick 3 to 6 headlines.** Prefer, in order: new controls or sample pages,
   platform or Windows App SDK changes that developers care about, visible app
   experience changes (search, navigation, home page, settings), and tooling.
   Everything else goes into the improvement lists.
5. **Draft the post** using the template below. Save it as Markdown outside the
   repository (or wherever the user asks); blog drafts are not committed.
6. **List the assets to capture.** For every headline, add an image placeholder
   and a short shot list so the author knows which screenshots or GIFs to record.
7. **Report open questions** to the author: names to verify, features to confirm,
   and anything whose status (experimental, preview) affects the wording.

## Voice and style

- Friendly, upbeat, and developer-focused. Address the reader as "you". Use "we"
  for the team.
- Lead each feature with the developer benefit, then explain what the sample
  shows. Keep each feature to one to three short paragraphs.
- Wrap API, type, property, and file names in inline code: `TitleBar`,
  `ListView.ScrollIntoView`, `ControlInfoData.json`.
- Link the control or API to Microsoft Learn when a docs page exists.
- Credit community contributions inline with "— thanks [@user](https://github.com/user)!"
- Clearly label experimental or preview APIs, and say they can change.
- Use emoji sparingly: at most one per H2 heading, and the sign-off.
- Avoid marketing superlatives and AI-sounding filler ("delve", "seamless",
  "game-changer", "in today's fast-paced world").

## Naming

- The app is **WinUI Gallery** starting with version 3.0. Earlier posts and
  Store listings used "WinUI 3 Gallery".
- Title: `Announcing WinUI Gallery X.Y`. Slug: `announcing-winui-gallery-x-y`.
- Use the major and minor version in the title (`3.0`), and the full tag
  (`v3.0.0`) in links to GitHub release notes.

## Template

```markdown
# Announcing WinUI Gallery X.Y

![WinUI Gallery X.Y hero image](<hero image, 1024x536>)

Hey WinUI developers! If you're new around here, WinUI Gallery is the go-to app
for exploring WinUI 3 controls, samples, design guidance, and handy tools — all
in one place. Today, we're excited to announce **WinUI Gallery X.Y**, <one
sentence summarizing the theme of the release>.

## **<emoji> <Headline 1>**

<Why it matters to developers, in one or two sentences.> <What the Gallery
now shows.>

Some of the things this unlocks:

- <Concrete capability>
- <Concrete capability>

![<alt text describing the screenshot>](<image>)

## **<emoji> New samples**

### <Control or API name>

<What the sample demonstrates and which APIs it uses.> — thanks [@user](https://github.com/user)!

![<alt text>](<image>)

### <Control or API name>

<...>

## **A few more improvements**

- <Improvement> ([#1234](https://github.com/microsoft/WinUI-Gallery/pull/1234)) — thanks [@user](https://github.com/user)!
- <Improvement> ([#1235](https://github.com/microsoft/WinUI-Gallery/pull/1235))

## **Additional improvements and bugfixes**

- **Accessibility**: <summary of accessibility fixes>
- **Platform**: <Windows App SDK / .NET updates>
- <Other notable fixes>

See the [full release notes on GitHub](https://github.com/microsoft/WinUI-Gallery/releases/tag/vX.Y.Z).

## **Thanks!**

A big thank you to everyone who contributed to this release:
[@user1](https://github.com/user1), [@user2](https://github.com/user2), ...

All in all, this new version is packed with polish, new samples, and community
love! Grab it [from the Microsoft Store](https://apps.microsoft.com/detail/9P3JFPWWDZRC?hl=en-us&gl=US&ocid=pdpshare)
or check out [the GitHub repo](https://github.com/microsoft/WinUI-Gallery) if you
want to contribute.

Happy coding! 💻✨
```

## Checklist before handing off

- [ ] Every claim maps to a merged PR in the release range.
- [ ] Every PR link and contributor handle is correct.
- [ ] The thank-you list excludes the core Gallery team and bots; check with the
      author whether Microsoft contributors from other teams belong in it.
- [ ] Experimental and preview APIs are labeled as such.
- [ ] Every image has alt text and a shot-list entry.
- [ ] The Store link and GitHub release tag match the version being released.
- [ ] The post is published only after the Store update is live (see
      `docs/PublishingNewVersion.md`).
