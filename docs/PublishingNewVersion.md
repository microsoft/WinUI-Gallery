# Publishing a new WinUI Gallery version

This runbook coordinates the Microsoft Store package with the GitHub release so
both represent the same source commit and version.

The public release uses two forms of the same version:

- GitHub release and tag: `vX.Y.Z` (for example, `v2.10.0`)
- MSIX packages and bundle: `X.Y.Z.0` (for example, `2.10.0.0`)

The Store package version must always be greater than the version that is
currently published. Do not use a pipeline run number for the package version.

## Prerequisites

The releaser needs:

- Write access to this GitHub repository.
- Permission to run the Azure DevOps pipeline named
  `WinUI-Gallery-Store-Release`.
- Access to the WinUI 3 Gallery product in Partner Center.

## 1. Prepare the release commit

Choose the release version `X.Y.Z`. Update all three checked-in version values to
`X.Y.Z.0`:

- `WinUIGallery/WinUIGallery.csproj`
- `WinUIGallery/Package.appxmanifest`
- `WinUIGallery/Package.Dev.appxmanifest`

Open and merge a version-bump pull request into `main`. Include any final
dependency updates or release-only changes in that pull request so the merged
commit is the exact source to release.

Wait for the required GitHub checks on `main` to pass. Record the merged commit
SHA; the Store build and GitHub release must both use that commit.

## 2. Validate the packages

Manually run `WinUI-Gallery-Store-Release` in Azure DevOps with:

- Branch: `main`
- `releaseVersion`: `X.Y.Z`
- `publishToStore`: `false`

This verifies that `releaseVersion` matches the version in
`WinUIGallery.csproj` and produces x64 and ARM64 packages without creating a
Store submission. Download and smoke-test the packages before continuing.

This step can be repeated without consuming a Store package version.

## 3. Submit the Store package for certification

Run the same pipeline again from the same `main` commit with:

- `releaseVersion`: `X.Y.Z`
- `publishToStore`: `true`

This setting is not a dry run. It builds the `X.Y.Z.0` Store package and submits
the update for certification.

The pipeline uses manual Store publishing, so passing certification does not
make the update public. Someone must still select **Publish now** in Partner
Center.

In Partner Center, confirm:

- The submission version is `X.Y.Z.0`.
- The bundle contains both x64 and ARM64 packages.
- Publishing is held for manual release.
- Certification starts without package validation errors.

If the submission is wrong, select **Cancel certification**. Wait until the
product returns to **Update in draft** before replacing or deleting it. Never
select **Publish now** for a test submission.

## 4. Prepare the GitHub release

While Store certification is running, create a draft GitHub release:

- Tag: `vX.Y.Z`
- Target: the exact commit SHA used for the Store update
- Title: `WinUI 3 Gallery vX.Y.Z`
- Release notes: summarize the release and include the generated comparison
  from the previous release tag

Leave the GitHub release as a draft. Publishing it creates the tag and announces
the release before the Store package is necessarily available.

WinUI Gallery releases historically do not attach MSIX files to GitHub; the
Microsoft Store is the package distribution channel.

## 5. Publish

After Store certification passes:

1. Review the certified package and release notes one final time.
2. Select **Publish now** in Partner Center.
3. Wait until the Microsoft Store listing offers version `X.Y.Z.0`.
4. Publish the draft GitHub release.
5. Verify the GitHub tag targets the recorded release commit.

This order avoids announcing a GitHub release while Store users can still only
install the previous version.

## Hotfixes

For a hotfix, increment the patch component (for example, `2.10.0` to
`2.10.1`) and repeat the complete process. Microsoft Store does not support
downgrading to an older package version; a corrective release must use a higher
version.
