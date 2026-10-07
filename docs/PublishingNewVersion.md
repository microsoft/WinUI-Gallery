# Publishing a new WinUI Gallery version

This runbook coordinates the Microsoft Store package with the GitHub release so
both represent the same source commit and version.

The public release uses two forms of the same version:

- GitHub release and tag: `vX.Y.Z` (for example, `v2.10.0`)
- MSIX packages and bundle: `X.Y.Z.0` (for example, `2.10.0.0`)

The Store package version must always be greater than the version that is
currently published. Do not use a pipeline run number for the package version.

Each minor version is released from its own branch, `release/X.Y` (for example,
`release/3.0`), created from `main`. The release branch freezes the source for
flight testing and the final release, while feature work continues on `main`.
Patch releases for that version (`X.Y.1`, `X.Y.2`) ship from the same branch.

## Prerequisites

The releaser needs:

- Write access to this GitHub repository.
- Permission to run the Azure DevOps pipeline named
  `WinUI-Gallery-Store-Release`.
- Access to the WinUI Gallery product in Partner Center.

To test an update with a small group before releasing it to everyone, submit it
to a [package flight](https://learn.microsoft.com/windows/apps/publish/package-flights)
first:

1. The flight group is the known user group `WinUI 3 Gallery testers` (under
   **Customers** > **Customer groups** in Partner Center). Add testers'
   Microsoft accounts there.
2. For each release, open the product's **Application overview** page, go to
   **Manage package flights**, and select **Create new package flight**. Name
   it `WinUI Gallery X.Y Flight`, select the testers group, and make sure the new
   flight has the highest rank. A flight's name and groups cannot be changed
   later. The pipeline looks the flight up by this exact name.

The pipeline cannot make the first submission to a new flight. It replaces any
pending flight submission (`force: true`), and the Store API rejects removing a
new flight's initial draft submission. In WinUI Gallery 3.0, that failed run
was followed by the flight disappearing from Partner Center. Make the first
submission of each new flight manually:

1. Run the pipeline with `publishToStore: false` (step 3) to build the packages.
2. Download the `MSIX-x64` and `MSIX-ARM64` artifacts from the run and bundle
   them with the Windows SDK:
   `makeappx bundle /d <folder with both .msix files> /p WinUIGallery_X.Y.Z.0.msixbundle /bv X.Y.Z.0`
3. Upload the `.msixbundle` (not the individual `.msix` files) on the flight
   submission's **Packages** page and submit it.

Later submissions to the same flight (for example, `X.Y.1`) can use the
pipeline.

Members of a flight group only receive packages from the highest-ranked flight
they belong to. They do not receive non-flighted updates, so delete old flights
when they are no longer needed.

## 1. Prepare the release commit

Choose the release version `X.Y.Z`. Update all three checked-in version values to
`X.Y.Z.0`:

- `WinUIGallery/WinUIGallery.csproj`
- `WinUIGallery/Package.appxmanifest`
- `WinUIGallery/Package.Dev.appxmanifest`

Open and merge a version-bump pull request into `main`. Include any final
dependency updates or release-only changes in that pull request so the merged
commit is the exact source to release.

If the release changes the app's display name (`DisplayName` in
`Package.appxmanifest`), first reserve the new name in Partner Center under
**Product management** > **Manage app names**. Store package validation rejects
a package whose display name does not exactly match a reserved name. Select the
new name in the Store listing before the update is released to all customers.

Wait for the required GitHub checks on `main` to pass.

## 2. Create the release branch

Create `release/X.Y` from the merged version-bump commit:

```powershell
git fetch origin
git push origin <commit-sha>:refs/heads/release/X.Y
```

From now on, the release branch is the source for the Store build and the GitHub
release. Only fixes needed for the release go into it (see
[Fixing issues during the release](#fixing-issues-during-the-release)).

`release/*` branches should be protected in the repository settings: require a
pull request and passing checks, and block force pushes and deletion.

## 3. Validate the packages

Manually run `WinUI-Gallery-Store-Release` in Azure DevOps with:

- Branch: `release/X.Y`
- `releaseVersion`: `X.Y.Z`
- `publishToStore`: `false`

This verifies that `releaseVersion` matches the version in
`WinUIGallery.csproj` and produces x64 and ARM64 packages without creating a
Store submission. Download and smoke-test the packages before continuing.

This step can be repeated without consuming a Store package version.

## 4. Submit the Store package for certification

Run the same pipeline again from `release/X.Y` with:

- `releaseVersion`: `X.Y.Z`
- `publishToStore`: `true`
- `storeReleaseTrack`: `Flight` (default) or `Production`
- `storeFlightName`: the Partner Center flight name, for example `WinUI Gallery 3.0 Flight`
  (required for `Flight`, ignored for `Production`)

This setting is not a dry run. It builds the `X.Y.Z.0` Store package and submits
the update for certification. Record the commit SHA of the run (shown in the
Azure DevOps run summary); the GitHub release must use that commit.

With `storeReleaseTrack: Flight`, the pipeline creates a submission for the
package flight only. If this is the flight's first submission, submit it
manually instead (see [Prerequisites](#prerequisites)). Store listing metadata is not changed, and customers outside
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

## 5. Prepare the GitHub release

While Store certification is running, create a draft GitHub release:

- Tag: `vX.Y.Z`
- Target: the exact commit SHA on `release/X.Y` used for the Store update
- Title: `WinUI Gallery vX.Y.Z`
- Release notes: summarize the release and include the generated comparison
  from the previous release tag

Leave the GitHub release as a draft. Publishing it creates the tag and announces
the release before the Store package is necessarily available.

WinUI Gallery releases historically do not attach MSIX files to GitHub; the
Microsoft Store is the package distribution channel.

## 6. Publish

### Flight track: test, then promote

After the flight submission passes certification:

1. Select **Publish now** for the flight submission in Partner Center.
2. Ask the flight group to install or update WinUI Gallery from the Microsoft
   Store and to verify version `X.Y.Z.0` (**Settings** > **About**).
3. Collect feedback. For blocking issues, follow
   [Fixing issues during the release](#fixing-issues-during-the-release) and
   submit the new patch version to the same flight.

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

## Fixing issues during the release

Fix issues on `main` first, then bring the fix into the release branch, so
`main` never loses a fix that shipped:

1. Merge the fix into `main` with a normal pull request.
2. Create a branch from `release/X.Y`, cherry-pick the fix
   (`git cherry-pick -x <commit>`), and bump the patch version (for example,
   `X.Y.Z` to `X.Y.(Z+1)`) in the three version files listed in step 1.
3. Open a pull request into `release/X.Y` and merge it after checks pass.
4. Run the pipeline from `release/X.Y` with the new `releaseVersion`.

A new package version is required for every Store submission that changes
binaries, including resubmissions to the same flight.

## Hotfixes

For a hotfix after the release is public, use the same steps on the existing
`release/X.Y` branch, then follow this runbook from step 3. Microsoft Store does
not support downgrading to an older package version; a corrective release must
use a higher version.

The next feature release starts again from `main` with a new `release/X.Y`
branch. Its version must be higher than any patch release already shipped.
