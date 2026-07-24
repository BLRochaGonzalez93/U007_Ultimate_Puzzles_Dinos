# Increment 036 — Fullscreen Ads Safe Display

## Purpose

This package does not resize Google rewarded/interstitial creatives. Those ads
are native full-screen content controlled by Google Mobile Ads.

Instead, it prevents the game from requesting a full-screen ad while Unity is
still reporting an invalid or transitional portrait-sized surface.

## Permanent Android orientation

All Android build profiles now apply:

- Default Orientation: Auto Rotation
- Portrait: disabled
- Portrait Upside Down: disabled
- Landscape Left: enabled
- Landscape Right: enabled

The same configuration is also applied at runtime before Bootstrap loads.

## Full-screen guard

Before RewardedAd.Show() or InterstitialAd.Show():

1. Screen.width must be greater than Screen.height.
2. Screen.safeArea must be valid and contained inside the current surface.
3. Resolution, safe area and orientation must remain unchanged for four frames.
4. If the state does not stabilize within 2.5 seconds, the ad is not shown.

Rewarded:
- A blocked/failed presentation returns false.
- No level reward is granted.

Interstitial:
- A blocked presentation is skipped so gameplay is never trapped.

## Diagnostics

When a full-screen ad opens, Console logs:

- Screen width/height
- Screen.orientation
- Screen.safeArea

This makes it possible to distinguish an app orientation problem from a native
Google Mobile Ads / Device Simulator rendering issue.

## Editor tools

VRM Games > Ultimate Puzzles Dinos > Monetization >
- Apply Landscape-Only Display
- Validate Fullscreen Display

## Important testing note

Unity Device Simulator emulates the game screen and safe area, but Google
Mobile Ads full-screen content is native platform UI. Final close-button and
system-inset verification must therefore also be performed on a physical
Android device before release.
