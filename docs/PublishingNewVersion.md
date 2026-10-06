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
- Access to the WinUI Gallery product in Partner Center.

To test an update with a small group before releasing it to everyone, also set
up a [package flight](https://learn.microsoft.com/windows/apps/publish/package-flights)
once in Partner Center:

1. Under **Customers** > **Customer groups**, create a known user group (for
   example, `WinUI Gallery testers`) that contains the Microsoft accounts of
   the testers.
2. In the WinUI Gallery product, open **Package flights** and create a flight
   (for example, `Insiders`) that targets that group. Record the flight name
   exactly; the pipeline looks it up by name.

Members of a flight group only receive packages from that flight. They do not
receive non-flighted updates, so keep the flight current or delete it when it is
no longer needed.

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
- `storeReleaseTrack`: `Flight` (default) or `Production`
- `storeFlightName`: the Partner Center flight name, for example `Insiders`
  (required for `Flight`, ignored for `Production`)

This setting is not a dry run. It builds the `X.Y.Z.0` Store package and submits
the update for certification.

With `storeReleaseTrack: Flight`, the pipeline creates a submission for the
package flight only. Store listing metadata is not changed, and customers outside
the flight group are not affected. Use `Production` only to skip flight testing
(for example, for an urgent hotfix).

The pipeline does not change the product's Store visibility; the submission
keeps the visibility of the previous submission.

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
- Title: `WinUI Gallery vX.Y.Z`
- Release notes: summarize the release and include the generated comparison
  from the previous release tag

Leave the GitHub release as a draft. Publishing it creates the tag and announces
the release before the Store package is necessarily available.

WinUI Gallery releases historically do not attach MSIX files to GitHub; the
Microsoft Store is the package distribution channel.

## 5. Publish

### Flight track: test, then promote

After the flight submission passes certification:

1. Select **Publish now** for the flight submission in Partner Center.
2. Ask the flight group to install or update WinUI Gallery from the Microsoft
   Store and to verify version `X.Y.Z.0` (**Settings** > **About**).
3. Collect feedback. For blocking issues, fix them on `main`, bump the version
   (for example, `X.Y.Z` to `X.Y.(Z+1)`), and submit a new flight.

When the flight is approved, promote the exact tested packages to everyone:

1. In Partner Center, start an update for the non-flighted submission.
2. On the **Packages** page, use the option to add packages from a package
   flight and choose the certified packages from the flight.
3. Review the Store listing (name, description, screenshots, what's new) for
   the release, then submit the update for certification.
4. Continue with the publication steps below.

Promoting the flight packages avoids rebuilding and resubmitting the same
version through the pipeline, so customers get the exact binaries that were
tested.

### Publication

After the non-flighted Store certification passes:

1. Review the certified package and release notes one final time.
2. Select **Publish now** in Partner Center.
3. Wait until the Microsoft Store listing offers version `X.Y.Z.0`.
4. Publish the draft GitHub release.
5. Verify the GitHub tag targets the recorded release commit.

This order avoids announcing a GitHub release while Store users can still only
install the previous version.

## Alternative: Microsoft Store Developer CLI

The governed pipeline is the supported release path. For local experiments, the
[Microsoft Store Developer CLI](https://learn.microsoft.com/windows/apps/publish/msstore-dev-cli/overview)
(`msstore`) can also submit to a package flight. It requires an Entra app
registered in Partner Center (`msstore reconfigure`).

```powershell
# Find the flight ID
msstore flights list 9P3JFPWWDZRC

# Submit a bundle to the flight without committing it for certification
msstore publish . -i .\WinUIGallery_X.Y.Z.0.msixbundle -id 9P3JFPWWDZRC -f <flightId> -nc

# Check the flight submission
msstore flights submission status 9P3JFPWWDZRC <flightId>
```

## Hotfixes

For a hotfix, increment the patch component (for example, `2.10.0` to
`2.10.1`) and repeat the complete process. Microsoft Store does not support
downgrading to an older package version; a corrective release must use a higher
version.
